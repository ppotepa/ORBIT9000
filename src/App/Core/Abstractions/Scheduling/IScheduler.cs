using ORBIT9000.Core.Models;

namespace ORBIT9000.Core.Abstractions.Scheduling
{
    public interface IScheduler
    {
        #region Methods

        void Schedule(IScheduleJob job, Action action);
        Task StartAsync(CancellationToken cancellationToken = default);

        #endregion Methods
    }
}
