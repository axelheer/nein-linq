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
    }
}
