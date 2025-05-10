using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Abstractions.Scheduling;
using ORBIT9000.Core.Models;

namespace ORBIT9000.Engine.Scheduling
{
    public class SimpleScheduler(IScheduleCalculator calculator,
                           ILogger<SimpleScheduler> logger) : IScheduler
    {
        private readonly List<ScheduleJobWithAction> _jobs = [];
        private readonly object _lock = new();
        private readonly IScheduleCalculator _calculator = calculator;
        private readonly ILogger<SimpleScheduler> _logger = logger;

        public void Schedule(IScheduleJob job)
        {
            lock (this._lock)
            {
                this._jobs.Add(new ScheduleJobWithAction(job, null));
            }
        }

        public void Schedule(IScheduleJob job, Action action)
        {
            lock (this._lock)
            {
                this._jobs.Add(new ScheduleJobWithAction(job, action));
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                ScheduleJobWithAction? due = null;
                DateTime now = DateTime.UtcNow;

                if (this._jobs.Count == 0)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                    continue;
                }

                lock (this._lock)
                {
                    due = this._jobs.Where(job => job.Job.NextRun <= now).OrderBy(j => j.Job.NextRun).FirstOrDefault();
                }

                if (due != null)
                {
                    _ = this.RunJobAsync(due, cancellationToken);
                    due.Job.NextRun = this._calculator.GetNextOccurrence(due.Job, due.Job.NextRun);
                    continue;
                }

                DateTime next;
                lock (this._lock)
                {
                    next = this._jobs.Min(j => j.Job.NextRun);
                }
                TimeSpan delay = next - now;
                if (delay > TimeSpan.Zero)
                    await Task.Delay(delay, cancellationToken);
            }
        }

        private async Task RunJobAsync(ScheduleJobWithAction job, CancellationToken token = default)
        {
            try
            {
                if (job.Job is IScheduleJob scheduleJob)
                {
                    await Task.Run(job.Action!, token);
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Job error");
            }
        }

        private class ScheduleJobWithAction
        {
            public ScheduleJobWithAction(IScheduleJob job, Action? action)
            {
                this.Job = job;
                this.Action = action;
            }

            public IScheduleJob Job { get; }
            public Action? Action { get; }
        }
    }
}
