using Moq;
using ORBIT9000.Core.Environment;
using ORBIT9000.Core.Models;

namespace ORBIT9000.Engine.Tests
{
    [TestFixture]
    public partial class Scheduler : Disposable
    {
        #region Methods

        protected override void DisposeManagedObjects()
        {
            this._simpleScheduler?.Dispose();
            base.DisposeManagedObjects();
        }

        private static MockScheduleJob CreateMockJob(DateTime nextRun)
        {
            return new() { NextRun = nextRun };
        }

        private async Task RunSchedulerAndCancelAfterDelay(int delay)
        {
            using CancellationTokenSource cancellationTokenSource = new();
            _ = this._simpleScheduler.StartAsync(cancellationTokenSource.Token);

            await Task.Delay(delay);
            await cancellationTokenSource.CancelAsync();
        }

        private void SetupScheduleCalculator(DateTime nextOccurrence)
        {
            this._scheduleCalculatorMock
                .Setup(calculator => calculator.GetNextOccurrence(It.IsAny<IScheduleJob>(), It.IsAny<DateTime>()))
                .Returns(nextOccurrence);
        }

        #endregion Methods

        #region Classes

        private class MockScheduleJob : IScheduleJob
        {
            #region Properties

            public IReadOnlyCollection<DayOfWeek>? DaysOfWeek { get; set; }
            public DateTime? End { get; set; }
            public TimeSpan Interval { get; set; } = TimeSpan.FromHours(1);
            public DateTime NextRun { get; set; }
            public DateTime Start { get; set; } = DateTime.UtcNow;
            public string? Name { get; set; }
            public string? OriginalExpression { get; set; }

            #endregion Properties
        }

        #endregion Classes
    }
}