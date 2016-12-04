#if EF

using System.Data.Entity;

namespace NeinLinq.Tests.DbAsyncQuery
{
    [DbConfigurationType(typeof(Config))]
    public class Context : DbContext
    {
        public DbSet<Dummy> Dummies { get; set; }

        public Context () : base("NeinLinq.EF6")
        {
        }
    }
}

#elif EFCore

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

        public override int SaveChanges()
        {
            Database.EnsureCreated();
            return base.SaveChanges();
        }
    }
}

#endif
