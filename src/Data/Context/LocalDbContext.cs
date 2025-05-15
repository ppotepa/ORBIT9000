using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ORBIT9000.ExampleDomain.Entities;

namespace ORBIT9000.Data.Context
{
    public class LocalDbContext : ExtendedDbContext
    {
        #region Fields

        private readonly IConfiguration _configuration;
        private readonly ILogger<LocalDbContext> _logger;

        #endregion Fields

        #region Constructors

        private static SchemaValidator _schemaValidator = default;


        public LocalDbContext(IConfiguration configuration, ILogger<LocalDbContext> logger)
        {
            _logger = logger;
            _configuration = configuration;
            _logger.LogInformation("Created a new instance of dbContext. {Code}", GetHashCode());

            if (_schemaValidator == null)
            {
                _schemaValidator = new SchemaValidator(logger, configuration);
            }

            bool validate = _schemaValidator.EnsureMigration();
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