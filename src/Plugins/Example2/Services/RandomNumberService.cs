using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace ORBIT9000.Plugins.Example.Services
{
    public class RandomNumberService
    {
        private readonly ILogger<RandomNumberService> _logger;

        public RandomNumberService(ILogger<RandomNumberService> logger)
        {
            _logger = logger;
            _logger.LogInformation("RandomNumberService created");
        }

<<<<<<< HEAD
<<<<<<< HEAD
        public async Task<IEnumerable<int>> GenerateRandomNumbers()
        {
            IEnumerable<int> numbers = Enumerable.Range(0, 100).Select(_ => RandomNumberGenerator.GetInt32(0, 100));
            Thread.Sleep(RandomNumberGenerator.GetInt32(0, 1000));
            _logger.LogInformation("Generated random number: {Number}", numbers);
=======
        public async Task<IEnumerable<int>> GenerateRandomNumber()
=======
        public async Task<IEnumerable<int>> GenerateRandomNumbers()
>>>>>>> 53c6dc2 (Further Remove code smells.)
        {
            var numbers = Enumerable.Range(0, 100).Select(x => RandomNumberGenerator.GetInt32(0, 100));
            Thread.Sleep(RandomNumberGenerator.GetInt32(0, 1000));
            _logger.LogInformation("Generated random number: {Number}", numbers);   
>>>>>>> 83dd439 (Remove Code Smells)

            return await Task.FromResult(numbers);
        }
    }
}