using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Abstractions;
using ORBIT9000.Core.Abstractions.Data;
using ORBIT9000.Core.Attributes;
using ORBIT9000.Core.Attributes.Engine;
using ORBIT9000.ExampleDomain.Entities;
using ORBIT9000.Plugins.Example.Common;
using ORBIT9000.Plugins.ScheduleExample2.DataProviders;

namespace ORBIT9000.Plugins.ScheduleExample2
{
    [DefaultProject("Example")]
    [SchedulableService("run every 5 seconds")]
    public class ExampleSchedulePlugin2(
        ILogger<ExampleSchedulePlugin2> logger,
        LondonDataProvider dataProvider,
        IRepository<WeatherData> weatherRepository) : IOrbitPlugin
    {
        #region Methods

        public async Task<object> Execute()
        {
            throw new NotImplementedException();
        }

        public async Task OnLoad()
        {
            try
            {
                IEnumerable<WeatherResponse> weatherResponses = await dataProvider.GetData();

                foreach (WeatherResponse response in weatherResponses)
                {
                    logger.LogInformation("Weather data: {@Response}", response);
                }

                logger.LogInformation("Fetched data from weather API: {@HashCode}", this.GetHashCode());

                List<WeatherData> weatherDataList = [.. weatherResponses.Select(response => new WeatherData
                {
                    Temperature = (decimal?)response?.Hourly?.Temperature2M?.Average(),
                    City = "London",
                    Longitude = response!.Longitude,
                    Lattitude = response!.Latitude
                })];

                foreach (WeatherData? weatherData in weatherDataList)
                {
                    weatherRepository.Add(weatherData);
                }

                weatherRepository.Save();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while fetching weather data.");
            }
        }

        public Task OnUnload()
        {
            logger.LogInformation("Unloading plugin {Name}", this.GetType().Name);
            return Task.CompletedTask;
        }

        public void RegisterServices(IServiceCollection collection)
        {
            collection.AddTransient<LondonDataProvider>();
        }

        #endregion Methods
    }
}