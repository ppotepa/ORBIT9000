using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Abstractions;
using ORBIT9000.Core.Attributes.Engine;
using ORBIT9000.Plugins.ScheduleExample3.DataProviders;
using ORBIT9000.Plugins.ScheduleExample3.Response;

namespace ORBIT9000.Plugins.ScheduleExample3
{
    [SchedulableService("run every 3 seconds")]
    public class ExampleSchedulePlugin3(ILogger<ExampleSchedulePlugin3> logger, WarsawDataProvider dataProvider) : IOrbitPlugin
    {
        private readonly WarsawDataProvider _dataProvider = dataProvider;
        private readonly ILogger<ExampleSchedulePlugin3> _logger = logger;

        public Task<object> Execute()
        {
            throw new NotImplementedException();
        }

        public Task OnLoad()
        {
            foreach (WeatherResponse response in this._dataProvider.GetData().GetAwaiter().GetResult())
            {
                this._logger.LogInformation("Weather data: {@Response}", response);
            }

            this._logger.LogInformation("Fetched data from weather API: {@Data}", this.GetHashCode());
            return Task.CompletedTask;
        }

        public Task OnUnload()
        {
            this._logger.LogInformation("Unloading plugin {Name}", this.GetType().Name);
            return Task.CompletedTask;
        }

        public void RegisterServices(IServiceCollection collection)
        {
            collection.AddTransient<WarsawDataProvider>();
        }
    }
}
