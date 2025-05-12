using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Abstractions;
using ORBIT9000.Core.Attributes.Engine;
using ORBIT9000.Plugins.Example.DataProviders;

namespace ORBIT9000.Plugins.Example
{
    [Singleton]
    public class ExamplePlugin(ILogger<ExamplePlugin> logger, ExampleDataProvider dataProvider) : IOrbitPlugin
    {
        private readonly ExampleDataProvider _dataProvider = dataProvider;
        private readonly ILogger<ExamplePlugin> _logger = logger;

        public Task<object> Execute()
        {
            throw new NotImplementedException();
        }

        public Task OnLoad()
        {
            foreach (Response.WeatherResponse response in this._dataProvider.GetData().GetAwaiter().GetResult())
            {
                this._logger.LogInformation("Weather data: {@Response}", response);
            }

            this._logger.LogInformation("Fetched data from weather API: {@Data}", this.GetHashCode());
            return Task.CompletedTask;
        }

        public Task OnUnload()
        {
            this._logger.LogInformation("Unloading plugin {Name}", this.GetType().Name);
            return Task.CompletedTask;
        }

        public void RegisterServices(IServiceCollection collection)
        {
            collection.AddTransient<ExampleDataProvider>();
        }
    }
}
