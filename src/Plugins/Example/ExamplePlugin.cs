using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ORBIT9000.Abstractions;
using ORBIT9000.Core.Attributes;
using ORBIT9000.Core.Attributes.Engine;
using ORBIT9000.Plugins.Example.Common;
using ORBIT9000.Plugins.Example.DataProviders;

namespace ORBIT9000.Plugins.Example
{
    [DefaultProject("Example")]
    [Singleton]
    public class ExamplePlugin(ILogger<ExamplePlugin> logger, ParisDataProvider dataProvider) : IOrbitPlugin
    {
        private readonly ParisDataProvider _dataProvider = dataProvider;
        private readonly ILogger<ExamplePlugin> _logger = logger;

        public Task<object> Execute()
        {
            throw new NotImplementedException();
        }

        public async Task OnLoad()
        {
            IEnumerable<WeatherResponse> data = await this._dataProvider.GetData();

            foreach (WeatherResponse response in data)
            {
                this._logger.LogInformation("Weather data: {@Response}", response);
            }

            this._logger.LogInformation("Fetched data from weather.");
        }

        public Task OnUnload()
        {
            this._logger.LogInformation("Unloading plugin {Name}", this.GetType().Name);
            return Task.CompletedTask;
        }

        public void RegisterServices(IServiceCollection collection)
        {
            collection.AddTransient<ParisDataProvider>();
        }
    }
}
