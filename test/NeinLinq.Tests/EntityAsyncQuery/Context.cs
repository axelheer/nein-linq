using NeinLinq.Fakes.DbAsyncQuery;
using Microsoft.EntityFrameworkCore;

namespace NeinLinq.Tests.EntityAsyncQuery
{
    public class Context : DbContext
    {
        public DbSet<Dummy> Dummies { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=NeinLinq.EntityFrameworkCore; Integrated Security=true;");
        }

        public void ResetDatabase()
        {
            Database.EnsureCreated();
            Dummies.RemoveRange(Dummies);
            SaveChanges();
        }
    }
}
