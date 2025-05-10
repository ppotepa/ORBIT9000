using ORBIT9000.Core.Models;
namespace ORBIT9000.Core.Abstractions.Scheduling;

public interface IScheduleCalculator
{
    DateTime GetNextOccurrence(IScheduleJob schedule, DateTime after);
}