using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ORBIT9000.Data.Context;
using System.Data;

namespace ORBIT9000.Data
{
    /// <summary>
    /// [TEMPORARY] This class is subject to removal. It is used only to speed up some development processes.
    /// </summary>
    public class SchemaValidator
    {
        #region Fields

        private readonly IConfiguration _configuration;
        private readonly ILogger<LocalDbContext> _logger;
        private static readonly object _migrationLock = new();
        private static bool _migrationChecked = false;

        #endregion Fields

        #region Constructors

        public SchemaValidator() { }

        public SchemaValidator(ILogger<LocalDbContext> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        #endregion Constructors

        #region Methods

        public bool EnsureMigration()
        {
            if (_migrationChecked)
                return false;

            lock (_migrationLock)
            {
                if (_migrationChecked)
                    return false;

                string? connectionString = _configuration.GetSection("OrbitEngine:Database:Debug:ConnectionString").Value;
                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    _logger.LogError("Database connection string is missing in appsettings.json.");
                    throw new InvalidOperationException("Database connection string is missing in appsettings.json.");
                }

                try
                {
                    using SqlConnection connection = new(connectionString);
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

                    // Get current DB schema
                    Dictionary<string, HashSet<string>> currentSchema = new(StringComparer.OrdinalIgnoreCase);
                    using (SqlCommand schemaCmd = connection.CreateCommand())
                    {
                        schemaCmd.CommandText = @"
                            SELECT TABLE_NAME, COLUMN_NAME
                            FROM INFORMATION_SCHEMA.COLUMNS
                            ORDER BY TABLE_NAME, COLUMN_NAME";
                        using SqlDataReader reader = schemaCmd.ExecuteReader();
                        while (reader.Read())
                        {
                            string table = reader["TABLE_NAME"].ToString()!;
                            string column = reader["COLUMN_NAME"].ToString()!;
                            if (!currentSchema.TryGetValue(table, out HashSet<string>? columns))
                            {
                                columns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                                currentSchema[table] = columns;
                            }
                            columns.Add(column);
                        }
                    }

                    // Parse expected schema
                    Dictionary<string, HashSet<string>> expectedSchema = new(StringComparer.OrdinalIgnoreCase);
                    string? currentTable = null;
                    using (StringReader reader = new(migrationScript))
                    {
                        string? line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            line = line.Trim();
                            if (line.StartsWith("CREATE TABLE", StringComparison.OrdinalIgnoreCase))
                            {
                                string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                                if (parts.Length >= 3)
                                {
                                    currentTable = parts[2].Trim('[', ']', '`');
                                    expectedSchema[currentTable] = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                                }
                            }
                            else if (currentTable != null && line.StartsWith("[") && line.Contains("]"))
                            {
                                string colName = line.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0].Trim('[', ']', '`', ',');
                                if (!string.IsNullOrWhiteSpace(colName) && !line.StartsWith("CONSTRAINT", StringComparison.OrdinalIgnoreCase))
                                {
                                    expectedSchema[currentTable].Add(colName);
                                }
                            }
                            else if (line.StartsWith(")", StringComparison.OrdinalIgnoreCase))
                            {
                                currentTable = null;
                            }
                        }
                    }

                    // Compare schemas
                    bool migrationNeeded = false;
                    foreach (KeyValuePair<string, HashSet<string>> expectedTable in expectedSchema)
                    {
                        if (!currentSchema.TryGetValue(expectedTable.Key, out HashSet<string>? actualColumns))
                        {
                            migrationNeeded = true;
                            break;
                        }

                        foreach (string expectedColumn in expectedTable.Value)
                        {
                            if (!actualColumns.Contains(expectedColumn))
                            {
                                migrationNeeded = true;
                                break;
                            }
                        }

                        if (migrationNeeded)
                            break;
                    }

                    if (migrationNeeded)
                    {
                        _logger.LogInformation("Schema difference detected. Executing migration script: {ScriptPath}", scriptPath);
                        using SqlCommand command = connection.CreateCommand();
                        command.CommandText = migrationScript;
                        command.CommandType = CommandType.Text;
                        command.ExecuteNonQuery();
                        _logger.LogInformation("Migration script executed successfully.");
                        _migrationChecked = true;
                        return true;
                    }
                    else
                    {
                        _logger.LogInformation("No schema differences detected. Migration skipped.");
                        _migrationChecked = true;
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred during database configuration or migration script execution.");
                    throw;
                }
            }
        }

        #endregion Methods
    }
}
