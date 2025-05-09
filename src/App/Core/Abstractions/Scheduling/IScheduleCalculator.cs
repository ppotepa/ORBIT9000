using ORBIT9000.Core.Attributes.Engine;
namespace ORBIT9000.Core.Abstractions.Scheduling;

public interface IScheduleCalculator
{
    DateTime GetNextOccurrence(IScheduleJob schedule, DateTime after);
}