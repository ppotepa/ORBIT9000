namespace ORBIT9000.Core.Attributes.Engine
{
    public class Schedule : ISchedule
    {
        public DateTime Start { get; }
        public TimeSpan Interval { get; }
        public DateTime? End { get; }

        /// <summary> If non‐empty, only fire when the current day is in this set. </summary>
        public IReadOnlyCollection<DayOfWeek>? DaysOfWeek { get; }
    }

    public interface ISchedule
    {
        DateTime Start { get; }
        TimeSpan Interval { get; }
        DateTime? End { get; }
        IReadOnlyCollection<DayOfWeek>? DaysOfWeek { get; }
    }
}
