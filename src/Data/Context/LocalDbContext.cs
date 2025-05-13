using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ORBIT9000.ExampleDomain.Entities;

namespace ORBIT9000.Data.Context
{
    public class LocalDbContext(IConfiguration configuration) : ExtendedDbContext
    {
        private readonly IConfiguration _configuration = configuration;

        #region Properties

        public DbSet<WeatherData> WeatherData { get; set; }

        #endregion Properties

        #region Methods

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string? connectionString = this._configuration.GetConnectionString("Debug");
            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        #endregion Methods
    }
}