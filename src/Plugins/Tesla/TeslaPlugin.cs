using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Abstractions.Plugin;
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

        public void Run()
        {
            var sum = 1;
            for(var i = 0; i < 100; i++)
            {
                sum += (i * 10);
                _logger.LogInformation($"xxxx {Thread.CurrentThread.ManagedThreadId}_{sum}ms");
                Thread.Sleep(sum);
            }
        }
    }
}
