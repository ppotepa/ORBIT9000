using Microsoft.EntityFrameworkCore;

namespace ORBIT9000.Data.Context
{
    public class ReflectiveInMemoryContext : ExtendedDbContext
    {
        public DbSet<string> Data { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(nameof(ReflectiveInMemoryContext));
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //DataSeed.Seed(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }
    }
}