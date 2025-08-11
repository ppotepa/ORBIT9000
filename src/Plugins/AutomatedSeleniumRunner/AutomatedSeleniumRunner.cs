using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Abstractions;
using ORBIT9000.Core.Attributes.Engine;

namespace Runner.AutomatedSeleniumRunner
{
<<<<<<< HEAD:src/Plugins/LinkedIn.JobApplier/LinkedInJobApplier.cs
    [SchedulableService("run every 10 seconds")]
    public class LinkedInJobApplier : IOrbitPlugin
=======
    [SchedulableService("run every 5 seconds")]
<<<<<<< HEAD
    public class AutomatedSeleniumRunner : IOrbitPlugin
>>>>>>> 93105f8 (Adjust Naming):src/Plugins/AutomatedSeleniumRunner/AutomatedSeleniumRunner.cs
=======
    public class AutomatedSeleniumRunner(ILogger<AutomatedSeleniumRunner> logger) : IOrbitPlugin
>>>>>>> bfa6c2d (Try fix pipeline)
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
