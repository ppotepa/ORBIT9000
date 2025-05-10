using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Abstractions.Scheduling;
using ORBIT9000.Core.Attributes.Engine;

namespace ORBIT9000.Engine.Scheduling
{
    public class SimpleScheduler(IScheduleCalculator calculator,
                           ILogger<SimpleScheduler> logger) : IScheduler
    {
        private readonly List<IScheduleJob> _jobs = [];
        private readonly object _lock = new();
        private readonly IScheduleCalculator _calculator = calculator;
        private readonly ILogger<SimpleScheduler> _logger = logger;

        public void Schedule(IScheduleJob job)
        {
            lock (this._lock) { this._jobs.Add(job); }
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                IScheduleJob? due = null;
                DateTime now = DateTime.UtcNow;

                if (this._jobs.Count == 0)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                    continue;
                }

                lock (this._lock)
                {
                    due = this._jobs.Where(job => job.NextRun <= now).OrderBy(j => j.NextRun).FirstOrDefault();
                }

                if (due != null)
                {
                    _ = this.RunJobAsync(due, cancellationToken);
                    due.NextRun = this._calculator.GetNextOccurrence(due, due.NextRun);
                    continue;
                }

                DateTime next;
                lock (this._lock)
                {
                    next = this._jobs.Min(j => j.NextRun);
                }
                TimeSpan delay = next - now;
                if (delay > TimeSpan.Zero)
                    await Task.Delay(delay, cancellationToken);
            }
        }

        private async Task RunJobAsync(IScheduleJob job, CancellationToken token)
        {
            try { await job.Action(token); }
            catch (Exception ex) { this._logger.LogError(ex, "Job error"); }
        }
    }
}