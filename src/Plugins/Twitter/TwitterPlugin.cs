using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Abstractions.Plugin;
using ORBIT9000.Plugins.Tesla.DataProviders.Twitter;

namespace ORBIT9000.Plugins.Twitter
{
    public class TwitterPlugin : IOrbitPlugin
    {
        private readonly ILogger<TwitterPlugin> _logger;
        private readonly IServiceProvider _provider;

        public TwitterPlugin(IServiceProvider provider, ILogger<TwitterPlugin> logger)
        {
            this._provider = provider;
            this._logger = logger;
        }

        public Task Run()
        {
            var logger = _provider.GetService(typeof(ILogger<TwitterDataProvider>)) as ILogger<TwitterDataProvider>;
           
            var provider = new TwitterDataProvider(logger);
            provider.GetData();

           
            return Task.CompletedTask;
        }
    }
}
