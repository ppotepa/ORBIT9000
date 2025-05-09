using ORBIT9000.Core.Abstractions.Scheduling;
using ORBIT9000.Core.Attributes.Engine;

namespace ORBIT9000.Engine.Scheduling
{
    public class ScheduleCalculator : IScheduleCalculator
    {
        public DateTime GetNextOccurrence(IScheduleJob schedule, DateTime after)
        {
            if (after < schedule.Start)
                return schedule.Start;

            DateTime next = after + schedule.Interval;
            if (schedule.DaysOfWeek != null && schedule.DaysOfWeek.Any())
            {
                int safety = 0;

                while (!schedule.DaysOfWeek.Contains(next.DayOfWeek))
                {
                    next = next.AddDays(1);
                    safety++;
                    if (safety > 7) break;
                }
            }

            if (schedule.End.HasValue && next > schedule.End.Value)
                return DateTime.MaxValue;

            return next;
        }
    }
}
