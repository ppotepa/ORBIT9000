using ORBIT9000.Core.Abstractions.Scheduling;
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
        }
    }
}
