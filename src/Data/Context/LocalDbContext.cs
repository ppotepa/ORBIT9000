using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using ORBIT9000.ExampleDomain.Entities;

namespace ORBIT9000.Data.Context
{
    public class LocalDbContext(IConfiguration configuration) : ExtendedDbContext
    {
        private readonly IConfiguration _configuration = configuration;

        #region Properties

        public DbSet<WeatherData> WeatherData { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string? connectionString = _configuration.GetSection("OrbitEngine:Database:Debug:ConnectionString").Value;
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("Database connection string is missing in appsettings.json.");

            optionsBuilder.UseSqlServer(connectionString);

            using Microsoft.Data.SqlClient.SqlConnection connection = new(connectionString);
            connection.Open();

            string scriptPath = Path.Combine(AppContext.BaseDirectory, "Schema", "LocalDbSchema.sql");
            if (!File.Exists(scriptPath))
                throw new FileNotFoundException($"Migration script not found: {scriptPath}");

            string script = File.ReadAllText(scriptPath);

            using Microsoft.Data.SqlClient.SqlCommand command = connection.CreateCommand();

            command.CommandText = script;
            command.CommandType = System.Data.CommandType.Text;

            command.ExecuteNonQuery();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        #endregion Methods
    }

    public class LocalDbContextFactory : IDesignTimeDbContextFactory<LocalDbContext>
    {
        public LocalDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                   .Build();

            return new LocalDbContext(configuration);
        }
    }
}