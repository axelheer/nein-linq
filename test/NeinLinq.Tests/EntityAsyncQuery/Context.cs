using System.Runtime.InteropServices;
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
        {
            _ = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? optionsBuilder.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=NeinLinq.EntityFrameworkCore; Integrated Security=true;")
                : optionsBuilder.UseInMemoryDatabase("NeinLinq.EntityFrameworkCore");
        }

        public void ResetDatabase()
        {
            _ = Database.EnsureCreated();
            Dummies.RemoveRange(Dummies);
            _ = SaveChanges();
        }
    }
}
