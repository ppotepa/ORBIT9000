using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Abstractions;
using ORBIT9000.Core.Attributes.Engine;

namespace LinkedIn.JobApplier
{
    [SchedulableService("run every 1 second")]
    public class LinkedInJobApplier : IOrbitPlugin
    {
        private readonly ILogger<LinkedInJobApplier> logger;

        public LinkedInJobApplier(ILogger<LinkedInJobApplier> logger)
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
