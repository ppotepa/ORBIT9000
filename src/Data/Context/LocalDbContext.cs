using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ORBIT9000.ExampleDomain.Entities;

namespace ORBIT9000.Data.Context
{
    public class LocalDbContext : ExtendedDbContext
    {
        #region Fields


        private static readonly SemaphoreSlim _dbCreationSemaphore = new(1, 1);
        private static bool _dbCreated;

        #endregion Fields

        #region Constructors


        public LocalDbContext(IConfiguration configuration, ILogger<LocalDbContext> logger) : base(configuration, logger)
        {
            EnsureDatabaseCreated().GetAwaiter().GetResult();
        }

        private async Task EnsureDatabaseCreated()
        {
            if (!_dbCreated)
            {
                await _dbCreationSemaphore.WaitAsync();
                try
                {
                    if (!_dbCreated)
                    {
                        _logger.LogInformation("Checking if database exists and creating if needed");
                        bool created = await Database.EnsureCreatedAsync();
                        if (created)
                        {
                            _logger.LogInformation("Database was created");
                        }
                        else
                        {
                            _logger.LogInformation("Database already exists");
                        }
                        _dbCreated = true;
                    }
                }
                finally
                {
                    _dbCreationSemaphore.Release();
                }
            }
        }

        public DbSet<WeatherData> WeatherData { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string? connectionString = _configuration.GetSection("OrbitEngine:Database:Debug:ConnectionString").Value;

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                _logger.LogError("Database connection string is missing in appsettings.json.");
                throw new InvalidOperationException("Database connection string is missing in appsettings.json.");
            }

            _logger.LogInformation("Using connection string: {ConnectionString}", connectionString);
            optionsBuilder.UseSqlServer(connectionString);

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        #endregion Methods
    }
}