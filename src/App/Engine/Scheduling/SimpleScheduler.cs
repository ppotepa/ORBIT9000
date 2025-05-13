using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Abstractions.Scheduling;
using ORBIT9000.Core.Environment;
using ORBIT9000.Core.Models;

namespace ORBIT9000.Engine.Scheduling
{
    public class SimpleScheduler(IScheduleCalculator calculator, ILogger<SimpleScheduler> logger)
                    : Disposable, IScheduler
    {
        private readonly List<ScheduleJobWithAction> _entries = [];
        private readonly HashSet<IScheduleJob> _runningJobs = [];
        private readonly object _lock = new();
        private readonly IScheduleCalculator _calculator = calculator;
        private readonly ILogger<SimpleScheduler> _logger = logger;
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        public void Schedule(IScheduleJob job, Action action)
        {
            this.ThrowIfDisposed();

            lock (this._lock)
            {
                this._entries.Add(new ScheduleJobWithAction(job, action));
                this._logger.LogInformation(
                    "Scheduled job: {JobName}, NextRun: {NextRun}",
                    job.Name,
                    job.NextRun);
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            this.ThrowIfDisposed();

            this._logger.LogInformation("SimpleScheduler started.");

            using CancellationTokenSource linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken, this._cancellationTokenSource.Token);
            CancellationToken token = linkedTokenSource.Token;

            while (!token.IsCancellationRequested)
            {
                if (await this.HandleNoJobsAsync(token)) continue;

                List<ScheduleJobWithAction> dueJobs = this.GetDueJobs();

                if (dueJobs.Count > 0)
                {
                    await this.HandleDueJobsAsync(dueJobs, token);
                    continue;
                }

                await this.HandleNextJobDelayAsync(token);
            }

            this._logger.LogInformation("SimpleScheduler stopped.");
        }

        private async Task<bool> HandleNoJobsAsync(CancellationToken token)
        {
            if (this._entries.Count == 0)
            {
                this._logger.LogDebug("No jobs scheduled. Waiting...");
                await Task.Delay(TimeSpan.FromSeconds(1), token);
                return true;
            }
            return false;
        }

        private List<ScheduleJobWithAction> GetDueJobs()
        {
            DateTime now = DateTime.UtcNow;
            lock (this._lock)
            {
                List<ScheduleJobWithAction> dueJobs = [.. this._entries
                    .Where(job => job.Job.NextRun <= now && !this._runningJobs.Contains(job.Job))
                    .OrderBy(job => job.Job.NextRun)];

                dueJobs.ForEach(job =>
                {
                    this._runningJobs.Add(job.Job);
                    this._logger.LogInformation(
                        "Job due: {JobName}, NextRun: {NextRun}",
                        job.Job.Name,
                        job.Job.NextRun);
                });

                return dueJobs;
            }
        }

        private async Task HandleDueJobsAsync(List<ScheduleJobWithAction> dueJobs, CancellationToken token)
        {
            this._logger.LogInformation("Handling {Count} due job(s).", dueJobs.Count);
            List<Task> jobTasks = dueJobs.ConvertAll(dueJob => this.RunJobAsync(dueJob, token));

            _ = Task.Run(async () =>
            {
                try
                {
                    await Task.WhenAll(jobTasks);
                }
                catch (Exception ex)
                {
                    this._logger.LogError(ex, "Error running concurrent jobs");
                }
            }, token);

            this.UpdateNextRunForJobs(dueJobs);

            await Task.Delay(10, token);
        }

        private void UpdateNextRunForJobs(List<ScheduleJobWithAction> dueJobs)
        {
            lock (this._lock)
            {
                dueJobs.ForEach(dueJob =>
                {
                    DateTime oldNextRun = dueJob.Job.NextRun;
                    dueJob.Job.NextRun = this._calculator.GetNextOccurrence(dueJob.Job, dueJob.Job.NextRun);
                    this._logger.LogDebug(
                        "Updated job {JobName} NextRun from {OldNextRun} to {NewNextRun}",
                        dueJob.Job.Name,
                        oldNextRun,
                        dueJob.Job.NextRun);
                });
            }
        }

        private async Task HandleNextJobDelayAsync(CancellationToken token)
        {
            DateTime now = DateTime.UtcNow;
            DateTime next;

            lock (this._lock)
            {
                if (this._entries.Count == 0)
                {
                    this._logger.LogDebug("No jobs to wait for. Delaying 1 second.");
                    Task.Delay(TimeSpan.FromSeconds(1), token);
                    return;
                }

                next = this._entries.Min(j => j.Job.NextRun);
            }

            TimeSpan delay = next - now;
            if (delay > TimeSpan.Zero)
            {
                this._logger.LogDebug(
                    "Waiting {Delay} until next job.",
                    delay);
                await Task.Delay(delay, token);
            }
        }

        private async Task RunJobAsync(ScheduleJobWithAction job, CancellationToken token = default)
        {
            try
            {
                this._logger.LogInformation(
                    "Running job: {JobName}",
                    job.Job.Name);

                if (job.Action != null)
                {
                    await Task.Run(job.Action, token);
                }
                this._logger.LogInformation(
                    "Job completed: {JobName}",
                    job.Job.Name);
            }
            catch (Exception ex)
            {
                this._logger.LogError(
                    ex,
                    "Job error: {JobName}",
                    job.Job.Name);
            }
            finally
            {
                lock (this._lock)
                {
                    this._runningJobs.Remove(job.Job);
                }
            }
        }

        protected override void DisposeManagedObjects()
        {
            try
            {
                this._logger.LogInformation("Disposing SimpleScheduler.");
                this._cancellationTokenSource.Cancel();
                this._cancellationTokenSource.Dispose();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error disposing SimpleScheduler");
            }

            base.DisposeManagedObjects();
        }

        private void ThrowIfDisposed()
        {
            ObjectDisposedException.ThrowIf(this.disposed, nameof(SimpleScheduler));
        }

        private sealed class ScheduleJobWithAction(IScheduleJob job, Action? action)
        {
            public IScheduleJob Job { get; } = job;
            public Action? Action { get; } = action;
        }
    }
}
