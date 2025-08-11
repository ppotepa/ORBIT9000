namespace ORBIT9000.Core.Attributes.Engine
{
    public class Schedule : ISchedule
    {
        public DateTime Start { get; set; }
        public TimeSpan Interval { get; set; }
        public DateTime? End { get; set; }

        /// <summary> If non‐empty, only fire when the current day is in this set. </summary>
        public IReadOnlyCollection<DayOfWeek>? DaysOfWeek { get; set; }
    }

    public interface ISchedule
    {
        DateTime Start { get; }
        TimeSpan Interval { get; }
        DateTime? End { get; }
        IReadOnlyCollection<DayOfWeek>? DaysOfWeek { get; }
    }
}
