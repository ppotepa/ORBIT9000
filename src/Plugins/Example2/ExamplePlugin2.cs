using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Abstractions;
using ORBIT9000.Plugins.Example.DataProviders;
using ORBIT9000.Plugins.Example.Services;

namespace ORBIT9000.Plugins.Example
{
    public class ExamplePlugin2 : IOrbitPlugin
    {
        private readonly ILogger<ExamplePlugin2> _logger;
        private readonly RandomNumberService _numbers;
        private readonly ExampleDataProvider _provider;

        // we should not be able to get data from the ExampleDataProvider here
        // Accessing data from the ExampleDataProvider here is not allowed
        // because it does not have a shared scope

        public ExamplePlugin2(RandomNumberService numbers, ILogger<ExamplePlugin2> _logger, ExampleDataProvider provider)
        {
            this._numbers = numbers;
            this._logger = _logger;
            this._provider = provider;

            _logger.LogInformation("ExamplePlugin2 created");
        }

        public async Task OnLoad()
        {
            IEnumerable<WeatherResponse> data = await _provider.GetData();
            IEnumerable<int> data2 = await _numbers.GenerateRandomNumbers();

            _logger.LogInformation("Fetched data from random number generator: {D1}, {D2}", data, data2);
        }

        public Task OnUnload()
        {
            return Task.CompletedTask;
        }

        public void RegisterServices(IServiceCollection collection)
        {
            collection.AddTransient<RandomNumberService>();
        }
    }

}