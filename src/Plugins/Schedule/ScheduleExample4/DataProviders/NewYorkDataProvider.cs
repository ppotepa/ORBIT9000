using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using ORBIT9000.Abstractions.Authentication;
using ORBIT9000.Core.Attributes;
using ORBIT9000.Core.Attributes.Engine;
using ORBIT9000.Plugins.Example.Common;

namespace ORBIT9000.Plugins.ScheduleExample4.DataProviders
{
    [DataProvider]
    [DefaultProject("Example")]
    public class NewYorkDataProvider : IAuthenticate
    {
#pragma warning disable S1075 // URIs should not be hardcoded
        private const string ForecastURL = "https://api.open-meteo.com/v1/forecast";
#pragma warning restore S1075 // URIs should not be hardcoded

        private readonly ILogger<NewYorkDataProvider> _logger;

        public NewYorkDataProvider(ILogger<NewYorkDataProvider> logger)
        {
            ArgumentNullException.ThrowIfNull(logger);

            _logger = logger;
            _logger.LogInformation("ExampleDataProvider initialized. {Data}", GetHashCode());
        }

        public bool AllowAnonymous => true;

        public bool IsAuthenticated => false;

        public IAuthResult Authenticate()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<WeatherResponse>> GetData()
        {
            _logger.LogInformation("Fetching data from weather API: {@Data}", GetHashCode());

            // Changed to fetch weather for New York City
            var query = new
            {
                latitude = 40.7128,
                longitude = -74.0060,
                hourly = "temperature_2m",
                imezone = "America/New_York"
            };

            Url url = ForecastURL.SetQueryParams(query);

            WeatherResponse result = url.GetJsonAsync<WeatherResponse>().GetAwaiter().GetResult();

            return Task.FromResult<IEnumerable<WeatherResponse>>([result]);
        }
    }
}