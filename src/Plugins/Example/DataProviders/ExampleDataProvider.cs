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
        private readonly ILogger<ExampleDataProvider> _logger;

        public ExampleDataProvider(ILogger<ExampleDataProvider> logger)
        {
            ArgumentNullException.ThrowIfNull(logger);
            this._logger = logger;
        }

        public bool AllowAnonymous => true;

        public bool IsAuthenticated => false;

        public IAuthResult Authenticate()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<WeatherResponse>> GetData()
        {
            var url = "https://api.open-meteo.com/v1/forecast".SetQueryParams(new
           {
               latitude = 52.52,
               longitude = 13.41,
               hourly = "temperature_2m",
               timezone = "Europe/Berlin"
           });

            var result = url.GetJsonAsync<WeatherResponse>().GetAwaiter().GetResult();

            return Task.FromResult<IEnumerable<WeatherResponse>>([result]);
        }
    }
}