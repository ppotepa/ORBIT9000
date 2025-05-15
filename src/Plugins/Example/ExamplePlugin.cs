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
            IEnumerable<WeatherResponse> data = await _dataProvider.GetData();

            foreach (WeatherResponse response in data)
            {
                _logger.LogInformation("Weather data: {@Response}", response);
            }

            _logger.LogInformation("Fetched data from weather.");
        }

        public Task OnUnload()
        {
            _logger.LogInformation("Unloading plugin {Name}", GetType().Name);
            return Task.CompletedTask;
        }

        public void RegisterServices(IServiceCollection collection)
        {
            collection.AddTransient<ParisDataProvider>();
        }
    }
}
