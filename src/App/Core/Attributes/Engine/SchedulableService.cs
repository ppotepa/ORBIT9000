namespace ORBIT9000.Core.Attributes.Engine
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SchedulableService : Attribute
    {
        public SchedulableService(string scheduleExpression)
        {
            this.ScheduleExpression = scheduleExpression;
        }

        public string ScheduleExpression { get; }
    }
}
