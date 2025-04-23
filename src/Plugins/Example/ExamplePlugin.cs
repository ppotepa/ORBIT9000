using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Abstractions.Loaders;
using ORBIT9000.Plugins.Example.DataProviders;

namespace ORBIT9000.Plugins.Example
{
    public class ExamplePlugin : IOrbitPlugin
    {
        private readonly ExampleDataProvider _dataProvider;
        private readonly ILogger<ExamplePlugin> _logger;
        private readonly IServiceProvider _provider;

        public ExamplePlugin(IServiceProvider provider, ILogger<ExamplePlugin> logger, ExampleDataProvider dataProvider)
        {
            this._provider = provider;
            this._logger = logger;
            this._dataProvider = dataProvider;
        }

        public Task OnLoad()
        {
            IEnumerable<WeatherResponse> data = this._dataProvider.GetData().GetAwaiter().GetResult();
            _logger.LogInformation("Fetched data from weather API: {Data}", data);  
            return Task.CompletedTask;
        }

        public Task OnUnload()
        {
            _logger.LogInformation("Unloading plugin {Name}", this.GetType().Name);
            return Task.CompletedTask;
        }

        public void RegisterServices(IServiceCollection services)
        {
            services.AddTransient<ExampleDataProvider>();
        }
    }
}
