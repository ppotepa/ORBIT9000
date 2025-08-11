
namespace ORBIT9000.Core.Attributes.Engine
{
    public class Schedule(Func<CancellationToken, Task> action) : IScheduleJob
    {
        public DateTime Start { get; set; }
        public TimeSpan Interval { get; set; }
        public DateTime? End { get; set; }
        public DateTime NextRun { get; set; }

        /// <summary> If non‐empty, only fire when the current day is in this set. </summary>
        public IReadOnlyCollection<DayOfWeek>? DaysOfWeek { get; set; }

        public Func<CancellationToken, Task> Action { get; } = action;
    }

    public interface IScheduleJob
    {
        DateTime Start { get; }
        TimeSpan Interval { get; }
        DateTime? End { get; }
        DateTime NextRun { get; set; }
        IReadOnlyCollection<DayOfWeek>? DaysOfWeek { get; }
        Func<CancellationToken, Task> Action { get; }
    }
}
