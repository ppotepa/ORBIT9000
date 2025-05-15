using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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

            _logger.LogInformation("ExamplePlugin2 created");
        }

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
        }

        public Task OnUnload()
        {
            return Task.CompletedTask;
        }

        public void RegisterServices(IServiceCollection collection)
        {
            collection.AddTransient<RandomNumberService>();
        }

        #endregion Methods
    }
}