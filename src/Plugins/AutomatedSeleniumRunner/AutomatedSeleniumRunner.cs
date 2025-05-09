using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Abstractions;
using ORBIT9000.Core.Attributes.Engine;

namespace LinkedIn.JobApplier
{
    [SchedulableService("run every 5 seconds")]
    public class AutomatedSeleniumRunner : IOrbitPlugin
    {
        private readonly ILogger<AutomatedSeleniumRunner> logger;

        public AutomatedSeleniumRunner(ILogger<AutomatedSeleniumRunner> logger)
        {
            this.logger = logger;
        }

        public Task OnLoad()
        {
            logger.LogInformation("LinkedIn Job Applier plugin loaded.");
            return Task.CompletedTask;
        }

        public Task OnUnload()
        {
            return Task.CompletedTask;
        }
    }
}
