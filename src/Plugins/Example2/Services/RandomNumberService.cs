using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace ORBIT9000.Plugins.Example2.Services
{
    public class RandomNumberService
    {
        private readonly ILogger<RandomNumberService> _logger;

        public RandomNumberService(ILogger<RandomNumberService> logger)
        {
            this._logger = logger;
            this._logger.LogInformation("RandomNumberService created");
        }

        public async Task<IEnumerable<int>> GenerateRandomNumbers()
        {
            IEnumerable<int> numbers = Enumerable.Range(0, 100).Select(_ => RandomNumberGenerator.GetInt32(0, 100));
            Thread.Sleep(RandomNumberGenerator.GetInt32(0, 1000));
            this._logger.LogInformation("Generated random number: {Number}", numbers);

            return await Task.FromResult(numbers);
        }
    }
}