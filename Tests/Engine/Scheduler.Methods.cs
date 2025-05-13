using Moq;
using ORBIT9000.Abstractions.Scheduling;
using ORBIT9000.Core.Environment;

namespace ORBIT9000.Engine.Tests
{
    // This test class currently uses Task.Delay or threaded calls to wait for asynchronous or scheduled operations.
    // These delays make the tests slower and potentially flaky, especially as the test suite grows.
    // TODO: Replace Task.Delay and threaded calls with controlled time simulation, mocks, or manual triggering
    // of scheduled behavior to improve reliability and speed of tests.
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
            public string? Name { get; set; }
            public DateTime NextRun { get; set; }
            public string? OriginalExpression { get; set; }
            public DateTime Start { get; set; } = DateTime.UtcNow;
            #endregion Properties
        }

        #endregion Classes
    }
}