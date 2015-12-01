using Microsoft.Data.Entity;

namespace NeinLinq.Tests.DbAsyncQueryData
{
    public class Context : DbContext
    {
        public DbSet<Dummy> Dummies { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase();
        }
    }
}
