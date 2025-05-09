using ORBIT9000.Core.Attributes.Engine;

namespace ORBIT9000.Core.Abstractions.Scheduling
{
    public interface IScheduler
    {
        #region Methods

        void Schedule(IScheduleJob schedule);
        Task StartAsync(CancellationToken cancellationToken = default);

        #endregion Methods
    }
}
