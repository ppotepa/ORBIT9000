using ORBIT9000.Core.Attributes.Engine;

namespace ORBIT9000.Core.Parsing
{
    public class SimpleSchedule : Schedule
    {
        public DateTime Start { get; set; }
        public TimeSpan Interval { get; set; }
        public DateTime? End { get; set; }
        public IReadOnlyCollection<DayOfWeek>? DaysOfWeek { get; set; }
    }
}
