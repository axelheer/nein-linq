#if EF

using System.Data.Entity;

namespace NeinLinq.Tests.DbAsyncQuery
{
    [DbConfigurationType(typeof(Config))]
    public class Context : DbContext
    {
        public DbSet<Dummy> Dummies { get; set; }

        public Context () : base("NeinLinq.EF")
        {
        }

        public void ResetDatabase()
        {
            Dummies.RemoveRange(Dummies);
            SaveChanges();
        }
    }
}

#elif EFCORE

using Microsoft.EntityFrameworkCore;

namespace NeinLinq.Tests.DbAsyncQuery
{
    public class Context : DbContext
    {
        public DbSet<Dummy> Dummies { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=NeinLinq.EFCore; Integrated Security=true;");
        }

        public void ResetDatabase()
        {
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }
    }
}

#endif
