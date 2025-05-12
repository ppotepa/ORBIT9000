using Microsoft.Extensions.Logging;
using Moq;
using ORBIT9000.Core.Abstractions.Scheduling;
using ORBIT9000.Core.Environment;
using ORBIT9000.Core.Models;
using ORBIT9000.Core.TempTools;
using ORBIT9000.Engine.Scheduling;

namespace ORBIT9000.Engine.Tests
{
    [TestFixture]
    public partial class Scheduler : Disposable
    {
        #region Fields

        private Mock<ILogger<SimpleScheduler>> _loggerMock;
        private Mock<IScheduleCalculator> _scheduleCalculatorMock;
        private SimpleScheduler _simpleScheduler;

        #endregion Fields

        #region Methods

        [SetUp]
        public void Setup()
        {
            this._scheduleCalculatorMock = new Mock<IScheduleCalculator>();
            this._loggerMock = new Mock<ILogger<SimpleScheduler>>();
            this._simpleScheduler = new SimpleScheduler(this._scheduleCalculatorMock.Object, this._loggerMock.Object);
        }

        [Test]
        public async Task StartAsync_CancellationTokenRespected()
        {
            bool jobExecuted = false;
            MockScheduleJob scheduleJob = CreateMockJob(DateTime.UtcNow.AddSeconds(10));

            this._simpleScheduler.Schedule(scheduleJob, () => jobExecuted = true);

            using CancellationTokenSource cancellationTokenSource = new();
            Task schedulerTask = this._simpleScheduler.StartAsync(cancellationTokenSource.Token);

            await cancellationTokenSource.CancelAsync();

            Task completedTask = await Task.WhenAny(schedulerTask, Task.Delay(1000));

            Assert.Multiple(() =>
            {
                Assert.That(schedulerTask, Is.EqualTo(completedTask));
                Assert.That(jobExecuted, Is.False);
            });
        }

        [Test]
        public async Task StartAsync_ConcurrentJobsExecution()
        {
            int job1ExecutionCount = 0;
            int job2ExecutionCount = 0;

            DateTime currentTime = DateTime.UtcNow;

            MockScheduleJob job1 = CreateMockJob(currentTime.AddMilliseconds(100));
            MockScheduleJob job2 = CreateMockJob(currentTime.AddMilliseconds(100));

            this.SetupScheduleCalculator(currentTime.AddMilliseconds(100));

            this._simpleScheduler.Schedule(job1, () => Interlocked.Increment(ref job1ExecutionCount));
            this._simpleScheduler.Schedule(job2, () => Interlocked.Increment(ref job2ExecutionCount));

            await this.RunSchedulerAndCancelAfterDelay(1000);

            Assert.Multiple(() =>
            {
                Assert.That(job1ExecutionCount, Is.GreaterThan(0), "Job 1 did not execute as expected.");
                Assert.That(job2ExecutionCount, Is.GreaterThan(0), "Job 2 did not execute as expected.");
            });
        }

        [Test]
        public async Task StartAsync_ExecutesJobFromTextSchedule()
        {
            bool jobExecuted = false;

            TextScheduleParser parser = new();
            IScheduleJob scheduleJob = parser.Parse("run every 1 second");

            scheduleJob.NextRun = DateTime.UtcNow.AddMilliseconds(-10);

            this.SetupScheduleCalculator(DateTime.UtcNow.AddSeconds(1));
            this._simpleScheduler.Schedule(scheduleJob, () => jobExecuted = true);

            await this.RunSchedulerAndCancelAfterDelay(100);

            Assert.That(jobExecuted, Is.True, "Job from text schedule was not executed");
        }

        [Test]
        public async Task StartAsync_HandlesExceptions()
        {
            MockScheduleJob scheduleJob = CreateMockJob(DateTime.UtcNow.AddMilliseconds(-10));

            this.SetupScheduleCalculator(DateTime.UtcNow.AddHours(1));

            this._simpleScheduler.Schedule(scheduleJob, () => throw new Exception("Test exception"));

            await this.RunSchedulerAndCancelAfterDelay(100);

            this._loggerMock.Verify(
                logger => logger.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((_, _) => true),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Test]
        public async Task StartAsync_JobNotExecutedDueToParsingError()
        {
            bool jobExecuted = false;

            TextScheduleParser parser = new();
            IScheduleJob? scheduleJob = null;

            try
            {
                scheduleJob = parser.Parse("invalid schedule format");
            }
            catch
            {
                // failed
            }

            if (scheduleJob != null)
            {
                this._simpleScheduler.Schedule(scheduleJob, () => jobExecuted = true);
                await this.RunSchedulerAndCancelAfterDelay(100);
            }

            Assert.That(jobExecuted, Is.False, "Job was executed despite parsing error.");
        }

        [Test]
        public async Task StartAsync_JobsRunInCorrectOrder()
        {
            List<int> executionOrder = [];
            DateTime currentTime = DateTime.UtcNow;

            MockScheduleJob firstJob = CreateMockJob(currentTime.AddMilliseconds(50));
            MockScheduleJob secondJob = CreateMockJob(currentTime.AddMilliseconds(10));

            this.SetupScheduleCalculator(DateTime.UtcNow.AddHours(1));

            this._simpleScheduler.Schedule(firstJob, () => executionOrder.Add(1));
            this._simpleScheduler.Schedule(secondJob, () => executionOrder.Add(2));

            await this.RunSchedulerAndCancelAfterDelay(200);

            Assert.Multiple(() =>
            {
                Assert.That(executionOrder, Has.Count.EqualTo(2));
                Assert.That(executionOrder[0], Is.EqualTo(2));
                Assert.That(executionOrder[1], Is.EqualTo(1));
            });
        }

        [Test]
        public async Task StartAsync_RunsJobWhenDue()
        {
            bool jobExecuted = false;
            MockScheduleJob scheduleJob = CreateMockJob(DateTime.UtcNow.AddMilliseconds(-10));

            this.SetupScheduleCalculator(DateTime.UtcNow.AddHours(1));
            this._simpleScheduler.Schedule(scheduleJob, () => jobExecuted = true);

            await this.RunSchedulerAndCancelAfterDelay(100);

            // Assert
            Assert.That(jobExecuted, Is.True, "Overdue job was not executed");
        }

        [Test]
        public async Task StartAsync_RunsMultipleJobsWhenOverdue()
        {
            int executedJobCount = 0;
            DateTime currentTime = DateTime.UtcNow;

            MockScheduleJob firstOverdueJob = CreateMockJob(currentTime.AddMilliseconds(-20));
            MockScheduleJob secondOverdueJob = CreateMockJob(currentTime.AddMilliseconds(-10));

            this.SetupScheduleCalculator(currentTime.AddHours(1));

            this._simpleScheduler.Schedule(firstOverdueJob, () => Interlocked.Increment(ref executedJobCount));
            this._simpleScheduler.Schedule(secondOverdueJob, () => Interlocked.Increment(ref executedJobCount));

            await this.RunSchedulerAndCancelAfterDelay(100);

            Assert.That(executedJobCount, Is.EqualTo(2), "Not all overdue jobs were executed");
        }

        [Test]
        public async Task StartAsync_UpdatesNextRunTime()
        {
            MockScheduleJob scheduleJob = CreateMockJob(DateTime.UtcNow.AddMilliseconds(-10));
            DateTime nextRunTime = DateTime.UtcNow.AddHours(1);

            this.SetupScheduleCalculator(nextRunTime);

            this._simpleScheduler.Schedule(scheduleJob, () => { });

            await this.RunSchedulerAndCancelAfterDelay(100);

            Assert.That(nextRunTime, Is.EqualTo(scheduleJob.NextRun));
        }

        [Test]
        public async Task StartAsync_WaitsUntilNextJob()
        {
            bool jobExecuted = false;
            DateTime currentTime = DateTime.UtcNow;
            MockScheduleJob scheduleJob = CreateMockJob(currentTime.AddMilliseconds(100));

            this.SetupScheduleCalculator(currentTime.AddHours(1));

            this._simpleScheduler.Schedule(scheduleJob, () => jobExecuted = true);

            Assert.That(jobExecuted, Is.False);

            await this.RunSchedulerAndCancelAfterDelay(150);

            Assert.That(jobExecuted, Is.True);
        }

        #endregion Methods
    }
}