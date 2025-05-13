namespace ORBIT9000.Abstractions.Scheduling
{
    public interface IScheduler
    {
        #region Methods

        void Schedule(IScheduleJob job, Action action);
        Task StartAsync(CancellationToken cancellationToken = default);

        #endregion Methods
    }
}
