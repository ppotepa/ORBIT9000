using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Abstractions;
using ORBIT9000.Core.Attributes.Engine;
using ORBIT9000.Core.Data.Retrieve;
using ORBIT9000.Plugins.ScheduleExample2.DataProviders;
using ORBIT9000.Plugins.ScheduleExample2.Response;

namespace ORBIT9000.Plugins.ScheduleExample2
{
    [SchedulableService("run every 10 seconds")]
    public class ExampleSchedulePlugin2(ILogger<ExampleSchedulePlugin2> logger, ExampleDataProvider dataProvider) :
        IOrbitPlugin,
        IDataRetriever
    {
        private readonly ExampleDataProvider _dataProvider = dataProvider;
        private readonly ILogger<ExampleSchedulePlugin2> _logger = logger;

        public Task OnLoad()
        {
            return Task.CompletedTask;
        }

        public Task OnUnload()
        {
            this._logger.LogInformation("Unloading plugin {Name}", this.GetType().Name);
            return Task.CompletedTask;
        }

        public async Task<object> Execute()
        {
            try
            {
                IEnumerable<WeatherResponse> data = await this._dataProvider.GetData();

                foreach (WeatherResponse response in data)
                {
                    this._logger.LogInformation("Weather data: {@Response}", response);
                }

                this._logger.LogInformation("Fetched data from weather API: {@Data}", this.GetHashCode());
                return await Task.FromResult(data);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "An error occurred while fetching weather data.");
            }

            return await Task.FromResult<object>(new object());
        }

        public void RegisterServices(IServiceCollection collection)
        {
            collection.AddTransient<ExampleDataProvider>();
        }
    }
}
