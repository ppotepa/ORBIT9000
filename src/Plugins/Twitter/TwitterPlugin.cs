using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Abstractions.Plugin;
using ORBIT9000.Plugins.Twitter.DataProviders;

namespace ORBIT9000.Plugins.Twitter
{
    public class TwitterPlugin : IOrbitPlugin
    {
        private readonly TwitterDataProvider _dataProvider;
        private readonly ILogger<TwitterPlugin> _logger;
        private readonly IServiceProvider _provider;

        public TwitterPlugin(IServiceProvider provider, ILogger<TwitterPlugin> logger, TwitterDataProvider dataProvider)
        {
            this._provider = provider;
            this._logger = logger;
            this._dataProvider = dataProvider;
        }

        public Task OnLoad()
        {
            var data = _dataProvider.GetData();
            return Task.CompletedTask;
        }

        public Task OnUnload()
        {
            _logger.LogInformation("Unloading plugin {Name}", this.GetType().Name);
            return Task.CompletedTask;
        }
    }
}
