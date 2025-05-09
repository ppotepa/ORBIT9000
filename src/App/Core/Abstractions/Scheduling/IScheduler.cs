using ORBIT9000.Core.Attributes.Engine;

namespace ORBIT9000.Core.Abstractions.Scheduling
{
    public interface IScheduler
    {
        void Schedule(ISchedule schedule, Action job);
        Task StartAsync(CancellationToken cancellationToken = default);
    }
}
