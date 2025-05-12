using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Abstractions.Authentication;
using ORBIT9000.Core.Attributes;
using ORBIT9000.Core.Attributes.Engine;
using ORBIT9000.Plugins.ScheduleExample3.Response;

namespace ORBIT9000.Plugins.ScheduleExample3.DataProviders
{
    [DataProvider]
    [DefaultProject("Example")]
    public class WarsawDataProvider : IAuthenticate
    {
#pragma warning disable S1075 // URIs should not be hardcoded
        private const string ForecastURL = "https://api.open-meteo.com/v1/forecast";
#pragma warning restore S1075 // URIs should not be hardcoded

        private readonly ILogger<WarsawDataProvider> _logger;

        public WarsawDataProvider(ILogger<WarsawDataProvider> logger)
        {
            ArgumentNullException.ThrowIfNull(logger);

            this._logger = logger;
            this._logger.LogInformation("ExampleDataProvider initialized. {Data}", this.GetHashCode());
        }

        public bool AllowAnonymous => true;

        public bool IsAuthenticated => false;

        public IAuthResult Authenticate()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<WeatherResponse>> GetData()
        {
            this._logger.LogInformation("Fetching data from weather API: {@Data}", this.GetHashCode());

            var query = new
            {
                latitude = 52.52,
                longitude = 13.41,
                hourly = "temperature_2m",
                imezone = "Europe/Warsaw"
            };

            Url url = ForecastURL.SetQueryParams(query);

            WeatherResponse result = url.GetJsonAsync<WeatherResponse>().GetAwaiter().GetResult();

            return Task.FromResult<IEnumerable<WeatherResponse>>([result]);
        }
        public Task<IEnumerable<WeatherResponse>> GetAlternativeData()
        {
            this._logger.LogInformation("Fetching alternative data from weather API: {@Data}", this.GetHashCode());

            var query = new
            {
                latitude = 40.71, // New York City coordinates
                longitude = -74.01,
                hourly = "precipitation",
                timezone = "America/New_York"
            };

            Url url = ForecastURL.SetQueryParams(query);

            WeatherResponse result = url.GetJsonAsync<WeatherResponse>().GetAwaiter().GetResult();

            return Task.FromResult<IEnumerable<WeatherResponse>>([result]);
        }
    }
}