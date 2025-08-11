using ORBIT9000.Abstractions.Scheduling;

namespace ORBIT9000.Engine.Scheduling
{
    public class ScheduleCalculator : IScheduleCalculator
    {
        public DateTime GetNextOccurrence(IScheduleJob schedule, DateTime after)
        {
            if (after < schedule.Start)
                return schedule.Start;

            DateTime next = after + schedule.Interval;
            if (schedule.DaysOfWeek?.Count is 0)
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
