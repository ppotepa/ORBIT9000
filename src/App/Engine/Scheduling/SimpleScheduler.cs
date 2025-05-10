using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Abstractions.Scheduling;
using ORBIT9000.Core.Environment;
using ORBIT9000.Core.Models;

namespace ORBIT9000.Engine.Scheduling
{
    public class SimpleScheduler(IScheduleCalculator calculator, ILogger<SimpleScheduler> logger)
            : Disposable, IScheduler
    {
        private readonly List<ScheduleJobWithAction> _jobs = [];
        private readonly HashSet<IScheduleJob> _runningJobs = [];
        private readonly object _lock = new();
        private readonly IScheduleCalculator _calculator = calculator;
        private readonly ILogger<SimpleScheduler> _logger = logger;
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        public void Schedule(IScheduleJob job)
        {
            this.ThrowIfDisposed();

            lock (this._lock)
            {
                this._jobs.Add(new ScheduleJobWithAction(job, null));
            }
        }

        public void Schedule(IScheduleJob job, Action action)
        {
            this.ThrowIfDisposed();

            lock (this._lock)
            {
                this._jobs.Add(new ScheduleJobWithAction(job, action));
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            this.ThrowIfDisposed();

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
        }

        private async Task<bool> HandleNoJobsAsync(CancellationToken token)
        {
            if (this._jobs.Count == 0)
            {
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
                List<ScheduleJobWithAction> dueJobs = [.. this._jobs
                    .Where(job => job.Job.NextRun <= now && !this._runningJobs.Contains(job.Job))
                    .OrderBy(j => j.Job.NextRun)];

                foreach (ScheduleJobWithAction? job in dueJobs)
                {
                    this._runningJobs.Add(job.Job);
                }

                return dueJobs;
            }
        }

        private async Task HandleDueJobsAsync(List<ScheduleJobWithAction> dueJobs, CancellationToken token)
        {
            List<Task> jobTasks = dueJobs.Select(dueJob => this.RunJobAsync(dueJob, token)).ToList();

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
                foreach (ScheduleJobWithAction dueJob in dueJobs)
                {
                    dueJob.Job.NextRun = this._calculator.GetNextOccurrence(dueJob.Job, dueJob.Job.NextRun);
                }
            }
        }

        private async Task HandleNextJobDelayAsync(CancellationToken token)
        {
            DateTime now = DateTime.UtcNow;
            DateTime next;

            lock (this._lock)
            {
                if (this._jobs.Count == 0)
                {
                    Task.Delay(TimeSpan.FromSeconds(1), token);
                    return;
                }

                next = this._jobs.Min(j => j.Job.NextRun);
            }

            TimeSpan delay = next - now;
            if (delay > TimeSpan.Zero)
                await Task.Delay(delay, token);
        }

        private async Task RunJobAsync(ScheduleJobWithAction job, CancellationToken token = default)
        {
            try
            {
                if (job.Action != null)
                {
                    await Task.Run(job.Action, token);
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Job error");
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

        private class ScheduleJobWithAction(IScheduleJob job, Action? action)
        {
            public IScheduleJob Job { get; } = job;
            public Action? Action { get; } = action;
        }
    }
}
