using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Abstractions;
using ORBIT9000.Core.Attributes.Engine;

namespace LinkedIn.JobApplier
{
<<<<<<< HEAD:src/Plugins/LinkedIn.JobApplier/LinkedInJobApplier.cs
    [SchedulableService("run every 10 seconds")]
    public class LinkedInJobApplier : IOrbitPlugin
=======
    [SchedulableService("run every 5 seconds")]
    public class AutomatedSeleniumRunner : IOrbitPlugin
>>>>>>> 93105f8 (Adjust Naming):src/Plugins/AutomatedSeleniumRunner/AutomatedSeleniumRunner.cs
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
