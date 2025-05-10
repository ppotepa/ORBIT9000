using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Abstractions;
using ORBIT9000.Core.Attributes.Engine;

namespace Runner.AutomatedSeleniumRunner
{
    [SchedulableService("run every 5 seconds")]
    public class AutomatedSeleniumRunner(ILogger<AutomatedSeleniumRunner> logger) : IOrbitPlugin
    {
        private readonly ILogger<AutomatedSeleniumRunner> logger = logger;

        public Task OnLoad()
        {
            this.logger.LogInformation("LinkedIn Job Applier plugin loaded.");
            return Task.CompletedTask;
        }

        public Task OnUnload()
        {
            return Task.CompletedTask;
        }
    }
}
