using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;
using NeinLinq.Fakes.EntityAsyncQuery;

namespace NeinLinq.Tests.EntityAsyncQuery
{
    public class Context : DbContext
    {
        public DbSet<Dummy> Dummies { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                optionsBuilder.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=NeinLinq.EntityFrameworkCore; Integrated Security=true;");
            }
            else
            {
                optionsBuilder.UseInMemoryDatabase("NeinLinq.EntityFrameworkCore");
            }
        }

        public void ResetDatabase()
        {
            Database.EnsureCreated();
            Dummies.RemoveRange(Dummies);
            SaveChanges();
        }
    }
}
