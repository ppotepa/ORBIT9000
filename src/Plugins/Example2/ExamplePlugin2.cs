using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
<<<<<<< HEAD
using ORBIT9000.Abstractions;
using ORBIT9000.Core.Attributes;
using ORBIT9000.Plugins.Example2.Services;

namespace ORBIT9000.Plugins.Example2
{
    [DefaultProject("Example")]
    public class ExamplePlugin2 : IOrbitPlugin
    {
        #region Fields

        private readonly ILogger<ExamplePlugin2> _logger;
        private readonly RandomNumberService _numbers;

        #endregion Fields

        // we should not be able to get data from the ExampleDataProvider here
        // Accessing data from the ExampleDataProvider here is not allowed
        // because it does not have a shared scope

        #region Constructors

        public ExamplePlugin2(RandomNumberService numbers, ILogger<ExamplePlugin2> _logger)
        {
            _numbers = numbers;
            this._logger = _logger;
=======
using ORBIT9000.Core.Abstractions;
using ORBIT9000.Plugins.Example.DataProviders;
using ORBIT9000.Plugins.Example.Services;

namespace ORBIT9000.Plugins.Example
{
    public class ExamplePlugin2 : IOrbitPlugin
    {
        private readonly ILogger<ExamplePlugin2> _logger;
        private readonly RandomNumberService _numbers;
        private readonly ExampleDataProvider _provider;

        // we should not be able to get data from the ExampleDataProvider here
        // Accessing data from the ExampleDataProvider here is not allowed
        // because it does not have a shared scope

        public ExamplePlugin2(RandomNumberService numbers, ILogger<ExamplePlugin2> _logger, ExampleDataProvider provider)
        {
            this._numbers = numbers;
            this._logger = _logger;
            this._provider = provider;
>>>>>>> 53c6dc2 (Further Remove code smells.)

            _logger.LogInformation("ExamplePlugin2 created");
        }

<<<<<<< HEAD
        #endregion Constructors

        #region Methods

        public Task<object> Execute()
        {
            throw new NotImplementedException();
        }

        public async Task OnLoad()
        {
            IEnumerable<int> data = await _numbers.GenerateRandomNumbers();
            _logger.LogInformation("Fetched data from random number generator: Count : {D0}", data.Count());
=======
        public async Task OnLoad()
        {
            IEnumerable<WeatherResponse> data = await _provider.GetData();
            IEnumerable<int> data2 = await _numbers.GenerateRandomNumbers();

<<<<<<< HEAD
            _logger.LogInformation("Fetched data from random number generator: {D1}, {D2}", data, data2);
>>>>>>> 53c6dc2 (Further Remove code smells.)
=======
            _logger.LogInformation("Fetched data from random number generator: Count : {D1}, Count : {D2}", data.Count(), data2.Count());
>>>>>>> 53879fa (Add AutoInitialization to PluginProvider)
        }

        public Task OnUnload()
        {
            return Task.CompletedTask;
        }

        public void RegisterServices(IServiceCollection collection)
        {
            collection.AddTransient<RandomNumberService>();
        }
<<<<<<< HEAD

        #endregion Methods
    }
=======
    }

>>>>>>> 53c6dc2 (Further Remove code smells.)
}