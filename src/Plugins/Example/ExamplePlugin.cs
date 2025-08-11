using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
using ORBIT9000.Abstractions;
using ORBIT9000.Core.Attributes;
using ORBIT9000.Core.Attributes.Engine;
using ORBIT9000.Plugins.Example.Common;
=======
using ORBIT9000.Core.Abstractions.Loaders;
>>>>>>> 6edfcca (refactor: replace Twitter plugin with Example plugin)
=======
using ORBIT9000.Core.Abstractions;
<<<<<<< HEAD
<<<<<<< HEAD
>>>>>>> 83dd439 (Remove Code Smells)
=======
using ORBIT9000.Core.Attributes.Engine;
>>>>>>> 15da848 (Improve Reradibility)
=======
using ORBIT9000.Core.Abstractions;
>>>>>>> 53879fa (Add AutoInitialization to PluginProvider)
=======
using ORBIT9000.Core.Attributes.Engine;
>>>>>>> 1aafd5a (Add Basic Messaging)
using ORBIT9000.Plugins.Example.DataProviders;

namespace ORBIT9000.Plugins.Example
{
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
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
=======
    [Singleton(typeof(ExamplePlugin))]
<<<<<<< HEAD
>>>>>>> 15da848 (Improve Reradibility)
=======
    //[Singleton(typeof(ExamplePlugin))]
>>>>>>> 53879fa (Add AutoInitialization to PluginProvider)
=======
    [Singleton(typeof(ExamplePlugin))]
>>>>>>> 1aafd5a (Add Basic Messaging)
    public class ExamplePlugin : IOrbitPlugin
=======
    public class ExamplePlugin(ILogger<ExamplePlugin> logger, ExampleDataProvider dataProvider) : IOrbitPlugin
>>>>>>> bfa6c2d (Try fix pipeline)
    {
        private readonly ExampleDataProvider _dataProvider = dataProvider;
        private readonly ILogger<ExamplePlugin> _logger = logger;

        public Task OnLoad()
        {
            foreach (Response.WeatherResponse response in this._dataProvider.GetData().GetAwaiter().GetResult())
            {
                this._logger.LogInformation("Weather data: {@Response}", response);
            }

            this._logger.LogInformation("Fetched data from weather API: {@Data}", this.GetHashCode());
            return Task.CompletedTask;
>>>>>>> 6edfcca (refactor: replace Twitter plugin with Example plugin)
        }

        public Task OnUnload()
        {
<<<<<<< HEAD
<<<<<<< HEAD
            _logger.LogInformation("Unloading plugin {Name}", GetType().Name);
            return Task.CompletedTask;
        }

        public void RegisterServices(IServiceCollection collection)
        {
            collection.AddTransient<ParisDataProvider>();
=======
            _logger.LogInformation("Unloading plugin {Name}", this.GetType().Name);
=======
            this._logger.LogInformation("Unloading plugin {Name}", this.GetType().Name);
>>>>>>> bfa6c2d (Try fix pipeline)
            return Task.CompletedTask;
        }

        public void RegisterServices(IServiceCollection collection)
        {
<<<<<<< HEAD
            services.AddTransient<ExampleDataProvider>();
>>>>>>> 6edfcca (refactor: replace Twitter plugin with Example plugin)
=======
            collection.AddTransient<ExampleDataProvider>();
>>>>>>> 83dd439 (Remove Code Smells)
        }
    }
}
