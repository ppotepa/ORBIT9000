using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Abstractions.Plugin;
using ORBIT9000.Plugins.Twitter.DataProviders;

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

        public Task OnLoad()
        {
            ILogger<TwitterDataProvider>? logger = _provider.GetService(typeof(ILogger<TwitterDataProvider>))
                as ILogger<TwitterDataProvider>;

            TwitterDataProvider provider = new TwitterDataProvider(logger);
            provider.GetData();

            return Task.CompletedTask;
        }

        public Task OnUnload()
        {
            _logger.LogInformation("Unloading plugin {Name}", this.GetType().Name);
            return Task.CompletedTask;
        }
    }
}
