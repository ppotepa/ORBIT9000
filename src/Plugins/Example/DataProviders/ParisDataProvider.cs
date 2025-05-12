using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Abstractions.Authentication;
using ORBIT9000.Plugins.Example.Common;

namespace ORBIT9000.Plugins.Example.DataProviders
{
    public class ParisDataProvider : IAuthenticate
    {
#pragma warning disable S1075 // URIs should not be hardcoded
        private const string ForecastURL = "https://api.open-meteo.com/v1/forecast";
#pragma warning restore S1075 // URIs should not be hardcoded

        private readonly ILogger<ParisDataProvider> _logger;

        public ParisDataProvider(ILogger<ParisDataProvider> logger)
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
                latitude = 48.8566,
                longitude = 2.3522,
                hourly = "temperature_2m",
                imezone = "Europe/Paris"
            };

            Url url = ForecastURL.SetQueryParams(query);

            WeatherResponse result = url.GetJsonAsync<WeatherResponse>().GetAwaiter().GetResult();

            return Task.FromResult<IEnumerable<WeatherResponse>>([result]);
        }
        public async Task<IEnumerable<WeatherResponse>> GetDataAsync()
        {
            this._logger.LogInformation("Fetching data from weather API: {@Data}", this.GetHashCode());

            var query = new
            {
                latitude = 48.8566,
                longitude = 2.3522,
                hourly = "temperature_2m",
                timezone = "Europe/Paris"
            };

            try
            {
                Url url = ForecastURL.SetQueryParams(query);

                WeatherResponse result = await url.GetJsonAsync<WeatherResponse>();

                return [result];
            }
            catch (FlurlHttpException ex)
            {
                this._logger.LogError(ex, "Error occurred while fetching data from weather API.");
                return [];
            }
        }
    }
}