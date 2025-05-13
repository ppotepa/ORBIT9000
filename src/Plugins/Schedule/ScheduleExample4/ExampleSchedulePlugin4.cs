using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ORBIT9000.Core.Attributes;
using ORBIT9000.Core.Attributes.Engine;
using ORBIT9000.Plugins.Example.Common;
using ORBIT9000.Plugins.ScheduleExample4.DataProviders;

namespace ORBIT9000.Abstractions
{
    [DefaultProject("Example")]
    [SchedulableService("run every 4 seconds")]
    public class ExampleSchedulePlugin4(ILogger<ExampleSchedulePlugin4> logger, NewYorkDataProvider dataProvider) : IOrbitPlugin
    {
        #region Fields

        private readonly NewYorkDataProvider _dataProvider = dataProvider;
        private readonly ILogger<ExampleSchedulePlugin4> _logger = logger;

        #endregion Fields

        #region Methods

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
            collection.AddTransient<NewYorkDataProvider>();
        }

        #endregion Methods
    }
}
