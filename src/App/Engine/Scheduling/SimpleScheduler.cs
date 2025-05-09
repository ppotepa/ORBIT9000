using ORBIT9000.Core.Abstractions.Scheduling;
using ORBIT9000.Core.Attributes.Engine;

namespace ORBIT9000.Engine.Scheduling;

public class SimpleScheduler : IScheduler
{
    private readonly List<(ISchedule schedule, Func<Task> job)> _jobs = new();

    public void Schedule(ISchedule schedule, Action job)
    {
        _jobs.Add((schedule, () => Task.Run(job)));
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var now = DateTime.UtcNow;

            foreach (var (schedule, job) in _jobs)
            {
                if (ShouldRun(schedule, now))
                {
                    _ = Task.Run(job, cancellationToken);
                }
            }

            await Task.Delay(1000, cancellationToken);
        }
    }

    private bool ShouldRun(ISchedule schedule, DateTime now)
    {
        if (schedule.Start > now || (schedule.End.HasValue && schedule.End < now))
            return false;

        if (schedule.DaysOfWeek != null && !schedule.DaysOfWeek.Contains(now.DayOfWeek))
            return false;

        var elapsed = now - schedule.Start;
        return elapsed.TotalMilliseconds % schedule.Interval.TotalMilliseconds < 1000;
    }
}
