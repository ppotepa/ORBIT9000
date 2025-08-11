<<<<<<< HEAD
﻿namespace ORBIT9000.Abstractions.Scheduling
{
    public interface IScheduler
    {
        #region Methods

        void Schedule(IScheduleJob job, Action action);
        Task StartAsync(CancellationToken cancellationToken = default);

        #endregion Methods
=======
﻿using ORBIT9000.Core.Attributes.Engine;

namespace ORBIT9000.Core.Abstractions.Scheduling
{
    public interface IScheduler
    {
        void Schedule(ISchedule schedule, Action job);
    }

    public interface IScheduler<TPlugin> : IScheduler
        where TPlugin : IOrbitPlugin    
    {   
>>>>>>> a7c6658 (Add Very Basic Job Scheduling)
    }
}
