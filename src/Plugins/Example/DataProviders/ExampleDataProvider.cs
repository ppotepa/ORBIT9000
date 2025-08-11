using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Abstractions.Authentication;
using ORBIT9000.Core.Abstractions.Providers.Data;
using ORBIT9000.Core.Attributes;
using ORBIT9000.Core.Attributes.Engine;

namespace ORBIT9000.Plugins.Example.DataProviders
{
    [DataProvider]
    [DefaultProject("Example")]
    public class ExampleDataProvider : IDataProvider<WeatherResponse>, IAuthenticate
    {
#pragma warning disable S1075 // URIs should not be hardcoded
        private const string ForecastURL = "https://api.open-meteo.com/v1/forecast";
#pragma warning restore S1075 // URIs should not be hardcoded

        private readonly ILogger<ExampleDataProvider> _logger;

        public ExampleDataProvider(ILogger<ExampleDataProvider> logger)
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
            _logger.LogInformation("Fetching data from weather API: {@Data}", this.GetHashCode());

            var query = new
            {
                latitude = 52.52,
                longitude = 13.41,
                hourly = "temperature_2m",
                imezone = "Europe/Warsaw"
            };

            var url = ForecastURL.SetQueryParams(query);

            var result = url.GetJsonAsync<WeatherResponse>().GetAwaiter().GetResult();

            return Task.FromResult<IEnumerable<WeatherResponse>>([result]);
        }
    }
}