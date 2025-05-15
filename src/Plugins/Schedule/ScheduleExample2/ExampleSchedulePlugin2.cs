using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ORBIT9000.Abstractions;
using ORBIT9000.Abstractions.Data;
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
            await Task.Delay(0);
            throw new NotImplementedException();
        }

        public async Task OnLoad()
        {
            try
            {
                IEnumerable<WeatherResponse> weatherResponses = await dataProvider.GetData();

                logger.LogInformation("Fetched data from weather API LondonDataProvider");

                List<WeatherData> weatherDataList = [.. weatherResponses.Select(response => new WeatherData
                {
                    Temperature = (decimal)new Random().NextDouble() * 10,
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
            logger.LogInformation("Unloading plugin {Name}", GetType().Name);
            return Task.CompletedTask;
        }

        public void RegisterServices(IServiceCollection collection)
        {
            collection.AddTransient<LondonDataProvider>();
        }

        #endregion Methods
    }
}