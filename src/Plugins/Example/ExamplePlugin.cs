using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
<<<<<<< HEAD
using ORBIT9000.Abstractions;
using ORBIT9000.Core.Attributes;
using ORBIT9000.Core.Attributes.Engine;
using ORBIT9000.Plugins.Example.Common;
=======
using ORBIT9000.Core.Abstractions.Loaders;
>>>>>>> 6edfcca (refactor: replace Twitter plugin with Example plugin)
using ORBIT9000.Plugins.Example.DataProviders;

namespace ORBIT9000.Plugins.Example
{
<<<<<<< HEAD
    [DefaultProject("Example")]
    [Singleton]
    public class ExamplePlugin(ILogger<ExamplePlugin> logger, ParisDataProvider dataProvider) : IOrbitPlugin
    {
        private readonly ParisDataProvider _dataProvider = dataProvider;
        private readonly ILogger<ExamplePlugin> _logger = logger;

        public Task<object> Execute()
        {
            throw new NotImplementedException();
        }

        public async Task OnLoad()
        {
            IEnumerable<WeatherResponse> data = await _dataProvider.GetData();

            foreach (WeatherResponse response in data)
            {
                _logger.LogInformation("Weather data: {@Response}", response);
            }

            _logger.LogInformation("Fetched data from weather.");
=======
    public class ExamplePlugin : IOrbitPlugin
    {
        private readonly ExampleDataProvider _dataProvider;
        private readonly ILogger<ExamplePlugin> _logger;
        private readonly IServiceProvider _provider;

        public ExamplePlugin(IServiceProvider provider, ILogger<ExamplePlugin> logger, ExampleDataProvider dataProvider)
        {
            this._provider = provider;
            this._logger = logger;
            this._dataProvider = dataProvider;
        }

        public Task OnLoad()
        {
            Task<IEnumerable<DataProviders.ExamplePlugin>> data = this._dataProvider.GetData();
            return Task.CompletedTask;
>>>>>>> 6edfcca (refactor: replace Twitter plugin with Example plugin)
        }

        public Task OnUnload()
        {
<<<<<<< HEAD
            _logger.LogInformation("Unloading plugin {Name}", GetType().Name);
            return Task.CompletedTask;
        }

        public void RegisterServices(IServiceCollection collection)
        {
            collection.AddTransient<ParisDataProvider>();
=======
            _logger.LogInformation("Unloading plugin {Name}", this.GetType().Name);
            return Task.CompletedTask;
        }

        public void RegisterServices(IServiceCollection services)
        {
            services.AddTransient<ExampleDataProvider>();
>>>>>>> 6edfcca (refactor: replace Twitter plugin with Example plugin)
        }
    }
}
