namespace ORBIT9000.Abstractions.Scheduling;

public interface IScheduleCalculator
{
    DateTime GetNextOccurrence(IScheduleJob schedule, DateTime after);
}