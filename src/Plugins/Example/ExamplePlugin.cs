using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Abstractions;
using ORBIT9000.Plugins.Example.DataProviders;

namespace ORBIT9000.Plugins.Example
{
    public class ExamplePlugin : IOrbitPlugin
    {
        private readonly ExampleDataProvider _dataProvider;
        private readonly ILogger<ExamplePlugin> _logger;

        public ExamplePlugin(IServiceProvider provider, ILogger<ExamplePlugin> logger, ExampleDataProvider dataProvider)
        {
            this._logger = logger;
            this._dataProvider = dataProvider;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S1481:Unused local variables should be removed", Justification = "<Pending>")]
        public Task OnLoad()
        {
            IEnumerable<WeatherResponse> data = this._dataProvider.GetData().GetAwaiter().GetResult();
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
