<<<<<<< HEAD
﻿using Microsoft.Extensions.Logging;
using ORBIT9000.Abstractions.Scheduling;
using ORBIT9000.Core.Environment;

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
            ThrowIfDisposed();

            lock (_lock)
            {
                _entries.Add(new ScheduleJobWithAction(job, action));
                _logger.LogInformation(
                    "Scheduled job: {JobName}, NextRun: {NextRun}",
                    job.Name,
                    job.NextRun);
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            _logger.LogInformation("SimpleScheduler started.");

            using CancellationTokenSource linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken, _cancellationTokenSource.Token);
            CancellationToken token = linkedTokenSource.Token;

            while (!token.IsCancellationRequested)
            {
                if (await HandleNoJobsAsync(token)) continue;

                List<ScheduleJobWithAction> dueJobs = GetDueJobs();

                if (dueJobs.Count > 0)
                {
                    await HandleDueJobsAsync(dueJobs, token);
                    continue;
                }

                await HandleNextJobDelayAsync(token);
            }

            _logger.LogInformation("SimpleScheduler stopped.");
        }

        private async Task<bool> HandleNoJobsAsync(CancellationToken token)
        {
            if (_entries.Count == 0)
            {
                _logger.LogDebug("No jobs scheduled. Waiting...");
                await Task.Delay(TimeSpan.FromSeconds(1), token);
                return true;
            }
            return false;
        }

        private List<ScheduleJobWithAction> GetDueJobs()
        {
            DateTime now = DateTime.UtcNow;
            lock (_lock)
            {
                List<ScheduleJobWithAction> dueJobs = [.. _entries
                    .Where(job => job.Job.NextRun <= now && !_runningJobs.Contains(job.Job))
                    .OrderBy(job => job.Job.NextRun)];

                dueJobs.ForEach(job =>
                {
                    _runningJobs.Add(job.Job);
                    _logger.LogInformation(
                        "Job due: {JobName}, NextRun: {NextRun}",
                        job.Job.Name,
                        job.Job.NextRun);
                });

                return dueJobs;
            }
        }

        private async Task HandleDueJobsAsync(List<ScheduleJobWithAction> dueJobs, CancellationToken token)
        {
            _logger.LogInformation("Handling {Count} due job(s).", dueJobs.Count);
            List<Task> jobTasks = dueJobs.ConvertAll(dueJob => RunJobAsync(dueJob, token));

            _ = Task.Run(async () =>
            {
                try
                {
                    await Task.WhenAll(jobTasks);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error running concurrent jobs");
                }
            }, token);

            UpdateNextRunForJobs(dueJobs);

            await Task.Delay(10, token);
        }

        private void UpdateNextRunForJobs(List<ScheduleJobWithAction> dueJobs)
        {
            lock (_lock)
            {
                dueJobs.ForEach(dueJob =>
                {
                    DateTime oldNextRun = dueJob.Job.NextRun;
                    dueJob.Job.NextRun = _calculator.GetNextOccurrence(dueJob.Job, dueJob.Job.NextRun);
                    _logger.LogDebug(
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

            lock (_lock)
            {
                if (_entries.Count == 0)
                {
                    _logger.LogDebug("No jobs to wait for. Delaying 1 second.");
                    Task.Delay(TimeSpan.FromSeconds(1), token);
                    return;
                }

                next = _entries.Min(j => j.Job.NextRun);
            }

            TimeSpan delay = next - now;
            if (delay > TimeSpan.Zero)
            {
                _logger.LogDebug(
                    "Waiting {Delay} until next job.",
                    delay);
                await Task.Delay(delay, token);
            }
        }

        private async Task RunJobAsync(ScheduleJobWithAction job, CancellationToken token = default)
        {
            try
            {
                _logger.LogInformation(
                    "Running job: {JobName}",
                    job.Job.Name);

                if (job.Action != null)
                {
                    await Task.Run(job.Action, token);
                }
                _logger.LogInformation(
                    "Job completed: {JobName}",
                    job.Job.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Job error: {JobName}",
                    job.Job.Name);
            }
            finally
            {
                lock (_lock)
                {
                    _runningJobs.Remove(job.Job);
                }
            }
        }

        protected override void DisposeManagedObjects()
        {
            try
            {
                _logger.LogInformation("Disposing SimpleScheduler.");
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disposing SimpleScheduler");
            }

            base.DisposeManagedObjects();
        }

        private void ThrowIfDisposed()
        {
            ObjectDisposedException.ThrowIf(disposed, nameof(SimpleScheduler));
        }

        private sealed class ScheduleJobWithAction(IScheduleJob job, Action? action)
        {
            public IScheduleJob Job { get; } = job;
            public Action? Action { get; } = action;
=======
﻿using ORBIT9000.Core.Abstractions.Scheduling;
using ORBIT9000.Core.Attributes.Engine;

namespace ORBIT9000.Engine.Scheduling
{
    public class SimpleScheduler : IScheduler
       
    {
        public SimpleScheduler()
        {
        }

        public void Schedule(ISchedule schedule, Action job)
        {
            DateTime next = schedule.Start <= DateTime.UtcNow
                       ? DateTime.UtcNow
                       : schedule.Start;

            Task.Run(async () =>
            {
                while (schedule.End == null || next <= schedule.End.Value)
                {
                    var wait = next - DateTime.UtcNow;
                    if (wait > TimeSpan.Zero)
                        await Task.Delay(wait);

                    job();

                    next = next + schedule.Interval;
                }
            });
>>>>>>> a7c6658 (Add Very Basic Job Scheduling)
        }
    }
}
