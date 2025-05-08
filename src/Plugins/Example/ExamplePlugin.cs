using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Abstractions;
using ORBIT9000.Core.Attributes.Engine;
using ORBIT9000.Plugins.Example.DataProviders;

namespace ORBIT9000.Plugins.Example
{
    [Singleton(typeof(ExamplePlugin))]
    public class ExamplePlugin : IOrbitPlugin
    {
        private readonly ExampleDataProvider _dataProvider;
        private readonly ILogger<ExamplePlugin> _logger;

        public ExamplePlugin(IServiceProvider provider, ILogger<ExamplePlugin> logger, ExampleDataProvider dataProvider)
        {
            this._logger = logger;
            this._dataProvider = dataProvider;
        }

        public Task OnLoad()
        {
#pragma warning disable S1481
            IEnumerable<WeatherResponse> data = this._dataProvider.GetData().GetAwaiter().GetResult();
#pragma warning restore S1481

            _logger.LogInformation("Fetched data from weather API: {@Data}", this.GetHashCode());
            return Task.CompletedTask;
        }

        public Task OnUnload()
        {
            _logger.LogInformation("Unloading plugin {Name}", this.GetType().Name);
            return Task.CompletedTask;
        }

        public void RegisterServices(IServiceCollection collection)
        {
            collection.AddTransient<ExampleDataProvider>();
        }
    }
}
