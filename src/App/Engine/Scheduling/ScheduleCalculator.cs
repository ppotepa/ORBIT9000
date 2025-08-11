<<<<<<< HEAD
﻿using ORBIT9000.Abstractions.Scheduling;
=======
﻿using ORBIT9000.Core.Abstractions.Scheduling;
using ORBIT9000.Core.Attributes.Engine;
>>>>>>> 0fcc8d3 (Improve Scheduler Logic)

namespace ORBIT9000.Engine.Scheduling
{
    public class ScheduleCalculator : IScheduleCalculator
    {
        public DateTime GetNextOccurrence(IScheduleJob schedule, DateTime after)
        {
            if (after < schedule.Start)
                return schedule.Start;

            DateTime next = after + schedule.Interval;
<<<<<<< HEAD
<<<<<<< HEAD
            if (schedule.DaysOfWeek?.Count is 0)
=======
            if (schedule.DaysOfWeek != null && schedule.DaysOfWeek.Any())
>>>>>>> 0fcc8d3 (Improve Scheduler Logic)
=======
            if (schedule.DaysOfWeek?.Count is 0)
>>>>>>> bfa6c2d (Try fix pipeline)
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
