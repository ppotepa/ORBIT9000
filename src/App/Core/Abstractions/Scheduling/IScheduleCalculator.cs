<<<<<<< HEAD
﻿namespace ORBIT9000.Abstractions.Scheduling;
=======
﻿using ORBIT9000.Core.Attributes.Engine;
namespace ORBIT9000.Core.Abstractions.Scheduling;
>>>>>>> 0fcc8d3 (Improve Scheduler Logic)

public interface IScheduleCalculator
{
    DateTime GetNextOccurrence(IScheduleJob schedule, DateTime after);
}