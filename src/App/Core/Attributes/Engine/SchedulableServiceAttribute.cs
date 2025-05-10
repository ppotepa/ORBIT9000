namespace ORBIT9000.Core.Attributes.Engine
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SchedulableServiceAttribute(string scheduleExpression) : Attribute, IEngineAttribute
    {
        public string ScheduleExpression { get; } = scheduleExpression;
    }
}
