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
<<<<<<< HEAD
        void Schedule(ISchedule schedule, Action job);
=======
        #region Methods

        void Schedule(IScheduleJob job);
        Task StartAsync(CancellationToken cancellationToken = default);

        #endregion Methods
>>>>>>> 0fcc8d3 (Improve Scheduler Logic)
    }
<<<<<<< HEAD

    public interface IScheduler<TPlugin> : IScheduler
<<<<<<< HEAD
        where TPlugin : IOrbitPlugin    
    {   
>>>>>>> a7c6658 (Add Very Basic Job Scheduling)
=======
        where TPlugin : IOrbitPlugin
    {
>>>>>>> fd5a59f (Code Cleanup)
    }
=======
>>>>>>> 86e317a (Refactor interfaces and improve null safety)
}
