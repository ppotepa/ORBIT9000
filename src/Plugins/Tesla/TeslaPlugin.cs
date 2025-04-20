using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Abstractions.Plugin;
using ORBIT9000.Core.Abstractions.Providers.Data;
using ORBIT9000.Plugins.Tesla.DataProviders.Twitter;

namespace ORBIT9000.Plugins.Tesla
{
    public class TeslaPlugin : IOrbitPlugin
    {
        private ILogger<TeslaPlugin> _logger;
        private IServiceProvider _provider;

        public TeslaPlugin(IServiceProvider provider, ILogger<TeslaPlugin> logger)
        {
            this._provider = provider;
            this._logger = logger;
        }

        public TeslaPlugin() { }

        public Type[] GetDataProviders()
        {
            return [typeof(TeslaTwitterDataProvider)];
        }

        public void Run()
        {
            _logger.LogInformation("Running Tesla plugin.");                
        }
    }
}
