using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Abstractions.Authentication;
using ORBIT9000.Core.Abstractions.Providers.Data;
using ORBIT9000.Core.Attributes;
using ORBIT9000.Core.Attributes.Engine;
using System.Text.Json;

namespace ORBIT9000.Plugins.Example.DataProviders
{
    [DataProvider]
    [DefaultProject("Example")]
    public class ExampleDataProvider : IDataProvider<ExamplePlugin>, IAuthenticate
    {
        private readonly HttpClient _httpClient = new HttpClient();
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

        public Task<IEnumerable<ExamplePlugin>> GetData()
        {
            string city = "London";
            const string URL = $"https://api.open-meteo.com/v1/forecast?latitude=51.5074&longitude=-0.1278&current_weather=true";

            try
            {
                var response = _httpClient.GetAsync(URL).GetAwaiter().GetResult();
                response.EnsureSuccessStatusCode();

                var json = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                var temperature = root.GetProperty("current_weather").GetProperty("temperature").GetDouble();
                var windSpeed = root.GetProperty("current_weather").GetProperty("windspeed").GetDouble();

                _logger.LogInformation("Weather in {City}: {Temperature}°C, Wind Speed: {WindSpeed} km/h", city, temperature, windSpeed);
                IEnumerable<ExamplePlugin> result = [new ExamplePlugin { Temperature = temperature, WindSpeed = windSpeed }];

                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching weather data: {Exception}", ex.Data);
                return Task.FromResult<IEnumerable<ExamplePlugin>>([]);
            }
        }
    }
}