using Microsoft.EntityFrameworkCore;
using NeinLinq.Fakes.EntityAsyncQuery;

namespace NeinLinq.Tests.EntityAsyncQuery
{
    public class Context : DbContext
    {
        public DbSet<Dummy> Dummies { get; }

        public Context()
        {
            Dummies = Set<Dummy>();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlite("Data Source=NeinLinq.EntityFrameworkCore.db");

        public void ResetDatabase()
        {
            _ = Database.EnsureDeleted();
            _ = Database.EnsureCreated();
        }
    }
}
