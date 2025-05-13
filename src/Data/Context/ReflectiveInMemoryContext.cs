using Microsoft.EntityFrameworkCore;
using ORBIT9000.ExampleDomain.Entities;

namespace ORBIT9000.Data.Context
{
    public class ReflectiveInMemoryContext : ExtendedDbContext
    {
        #region Properties

        public DbSet<WeatherData> WeatherData { get; set; }

        #endregion Properties

        #region Methods

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(nameof(ReflectiveInMemoryContext));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        #endregion Methods
    }
}