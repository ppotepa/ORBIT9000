using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Abstractions.Plugin;
using ORBIT9000.Plugins.Tesla.DataProviders.Twitter;

namespace ORBIT9000.Plugins.Twitter
{
    public class TwitterPlugin : IOrbitPlugin
    {
        private ILogger<TwitterPlugin> _logger;
        private IServiceProvider _provider;

        public TwitterPlugin(IServiceProvider provider, ILogger<TwitterPlugin> logger)
        {
            this._provider = provider;
            this._logger = logger;
        }

        public ValueTask DisposeAsync()
        {
            return new ValueTask();
        }

        public Task Run()
        {
            var provider = new TwitterDataProvider(_provider.GetService(typeof(ILogger<TwitterDataProvider>)) as ILogger<TwitterDataProvider>);
            provider.GetData();

           
            return Task.CompletedTask;
        }
    }
}
