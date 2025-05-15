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

        public LocalDbContext(IConfiguration configuration, ILogger<LocalDbContext> logger)
        {
            _logger = logger;
            _configuration = configuration;
            _logger.LogInformation("Created a new instance of dbContext. {Code}", GetHashCode());

            string? connectionString = _configuration.GetSection("OrbitEngine:Database:Debug:ConnectionString").Value;
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                _logger.LogError("Database connection string is missing in appsettings.json.");
                throw new InvalidOperationException("Database connection string is missing in appsettings.json.");
            }

            try
            {
                using Microsoft.Data.SqlClient.SqlConnection connection = new(connectionString);
                connection.Open();
                _logger.LogInformation("Database connection opened successfully.");

                string scriptPath = Path.Combine(AppContext.BaseDirectory, "Schema", "LocalDbSchema.sql");
                if (!File.Exists(scriptPath))
                {
                    _logger.LogError("Migration script not found: {ScriptPath}", scriptPath);
                    throw new FileNotFoundException($"Migration script not found: {scriptPath}");
                }

                string migrationScript = File.ReadAllText(scriptPath);
                _logger.LogInformation("Checking if migration is needed: {ScriptPath}", scriptPath);

                // Get current DB schema as a string
                string currentSchema = string.Empty;
                using (Microsoft.Data.SqlClient.SqlCommand schemaCmd = connection.CreateCommand())
                {
                    schemaCmd.CommandText = @"
                        SELECT TABLE_NAME, COLUMN_NAME, DATA_TYPE, IS_NULLABLE, CHARACTER_MAXIMUM_LENGTH
                        FROM INFORMATION_SCHEMA.COLUMNS
                        ORDER BY TABLE_NAME, COLUMN_NAME";
                    using (Microsoft.Data.SqlClient.SqlDataReader reader = schemaCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            currentSchema += $"{reader["TABLE_NAME"]}|{reader["COLUMN_NAME"]}|{reader["DATA_TYPE"]}|{reader["IS_NULLABLE"]}|{reader["CHARACTER_MAXIMUM_LENGTH"]}\n";
                        }
                    }
                }

                // Extract expected schema from migration script (very basic, just table/column names)
                HashSet<string> expectedSchema = [];
                using (StringReader reader = new(migrationScript))
                {
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        line = line.Trim();
                        if (line.StartsWith("CREATE TABLE", StringComparison.OrdinalIgnoreCase))
                        {
                            string tableName = line.Split(' ', StringSplitOptions.RemoveEmptyEntries)[2].Trim('[', ']');
                            expectedSchema.Add(tableName);
                        }
                    }
                }

                // Compare: if any expected table is missing, run migration
                bool migrationNeeded = false;
                foreach (string table in expectedSchema)
                {
                    if (!currentSchema.Contains(table + "|"))
                    {
                        migrationNeeded = true;
                        break;
                    }
                }

                if (migrationNeeded)
                {
                    _logger.LogInformation("Schema difference detected. Executing migration script: {ScriptPath}", scriptPath);
                    using Microsoft.Data.SqlClient.SqlCommand command = connection.CreateCommand();
                    command.CommandText = migrationScript;
                    command.CommandType = System.Data.CommandType.Text;
                    command.ExecuteNonQuery();
                    _logger.LogInformation("Migration script executed successfully.");
                }
                else
                {
                    _logger.LogInformation("No schema differences detected. Migration skipped.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during database configuration or migration script execution.");
                throw;
            }
        }

        #endregion Constructors

        #region Properties

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