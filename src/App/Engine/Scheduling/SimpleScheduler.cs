using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Abstractions.Scheduling;
using ORBIT9000.Core.Attributes.Engine;

namespace ORBIT9000.Engine.Scheduling
{
    public class SimpleScheduler : IScheduler
    {
        private readonly List<IScheduleJob> _jobs = new();
        private readonly object _lock = new();
        private readonly IScheduleCalculator _calculator;
        private readonly ILogger<SimpleScheduler> _logger;

        public SimpleScheduler(IScheduleCalculator calculator,
                               ILogger<SimpleScheduler> logger)
        {
            _calculator = calculator;
            _logger = logger;
        }

        public void Schedule(IScheduleJob job)
        {            
            lock (_lock) { _jobs.Add(job); }
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                IScheduleJob due = null;
                DateTime now = DateTime.UtcNow;

                if(_jobs.Count == 0)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                    continue;
                }

                lock (_lock)
                {
                    due = _jobs.Where(job => job.NextRun <= now).OrderBy(j => j.NextRun).FirstOrDefault();
                }

                if (due != null)
                {
                    _ = RunJobAsync(due, cancellationToken);
                    due.NextRun = _calculator.GetNextOccurrence(due, due.NextRun);
                    continue;
                }

                DateTime next;
                lock (_lock)
                {
                    next = _jobs.Min(j => j.NextRun);
                }
                var delay = next - now;
                if (delay > TimeSpan.Zero)
                    await Task.Delay(delay, cancellationToken);
            }
        }

        private async Task RunJobAsync(IScheduleJob job, CancellationToken token)
        {
            try { await job.Action(token); }
            catch (Exception ex) { _logger.LogError(ex, "Job error"); }
        }
    }
}