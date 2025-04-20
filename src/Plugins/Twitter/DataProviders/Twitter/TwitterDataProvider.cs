using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Abstractions.Authentication;
using ORBIT9000.Core.Abstractions.Providers.Data;
using ORBIT9000.Core.Attributes;
using ORBIT9000.Core.Attributes.Engine;
using ORBIT9000.Plugins.Twitter.DataProviders;
using System.Net.Http;
using System.Text.Json;

namespace ORBIT9000.Plugins.Tesla.DataProviders.Twitter
{
    [DataProvider]
    [DefaultProject("Tesla")]
    internal class TwitterDataProvider : IDataProvider<TwitterResult>, IAuthenticate
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private ILogger<TwitterDataProvider> _logger;

        public TwitterDataProvider(ILogger<TwitterDataProvider> logger)
        {
            this._logger = logger;
        }

        public bool AllowAnonymous => true;

        public bool IsAuthenticated => false;

        public IAuthResult Authenticate()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TwitterResult>> GetData()
        {
            string city = "London";
            string url = $"https://api.open-meteo.com/v1/forecast?latitude=51.5074&longitude=-0.1278&current_weather=true";

            try
            {
                var response = _httpClient.GetAsync(url).GetAwaiter().GetResult();
                response.EnsureSuccessStatusCode();

                var json = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                var temperature = root.GetProperty("current_weather").GetProperty("temperature").GetDouble();
                var windSpeed = root.GetProperty("current_weather").GetProperty("windspeed").GetDouble();

                _logger.LogInformation($"Weather in {city}: {temperature}°C, Wind Speed: {windSpeed} km/h");
                IEnumerable<TwitterResult> result = [new TwitterResult { Temperature = temperature, WindSpeed = windSpeed }];

                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error fetching weather data: " + ex.Message);
                return Task.FromResult<IEnumerable<TwitterResult>>([]);
            }
        }
    }
}