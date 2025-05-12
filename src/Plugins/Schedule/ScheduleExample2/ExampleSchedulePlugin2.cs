using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Abstractions;
using ORBIT9000.Core.Attributes;
using ORBIT9000.Core.Attributes.Engine;
using ORBIT9000.Data;
using ORBIT9000.ExampleDomain.Entities;
using ORBIT9000.Plugins.Example.Common;
using ORBIT9000.Plugins.ScheduleExample2.DataProviders;

namespace ORBIT9000.Plugins.ScheduleExample2
{
    [DefaultProject("Example")]
    [SchedulableService("run every 5 seconds")]
    public class ExampleSchedulePlugin2 : IOrbitPlugin
    {
        #region Fields

        private readonly LondonDataProvider _dataProvider;
        private readonly ILogger<ExampleSchedulePlugin2> _logger;
        private readonly IRepository<WeatherData> _weatherRepository;

        #endregion Fields

        #region Constructor

        public ExampleSchedulePlugin2(
            ILogger<ExampleSchedulePlugin2> logger,
            LondonDataProvider dataProvider,
            IRepository<WeatherData> weatherRepository)
        {
            this._logger = logger;
            this._dataProvider = dataProvider;
            this._weatherRepository = weatherRepository;
        }

        #endregion Constructor

        #region Methods

        public async Task<object> Execute()
        {
            throw new NotImplementedException();
        }

        public async Task OnLoad()
        {
            try
            {
                IEnumerable<WeatherResponse> weatherResponses = await this._dataProvider.GetData();

                foreach (WeatherResponse response in weatherResponses)
                {
                    this._logger.LogInformation("Weather data: {@Response}", response);
                }

                this._logger.LogInformation("Fetched data from weather API: {@HashCode}", this.GetHashCode());

                List<WeatherData> weatherDataList = [.. weatherResponses.Select(response => new WeatherData
                {
                    Temperature = (decimal?)response?.Hourly?.Temperature2M?.Average(),
                    City = "London",
                    Long = response!.Longitude,
                    Lat = response!.Latitude
                })];

                foreach (WeatherData? weatherData in weatherDataList)
                {
                    this._weatherRepository.Add(weatherData);
                }

                this._weatherRepository.Save();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "An error occurred while fetching weather data.");
            }
        }

        public Task OnUnload()
        {
            this._logger.LogInformation("Unloading plugin {Name}", this.GetType().Name);
            return Task.CompletedTask;
        }

        public void RegisterServices(IServiceCollection services)
        {
            services.AddTransient<LondonDataProvider>();
        }

        #endregion Methods
    }
}
