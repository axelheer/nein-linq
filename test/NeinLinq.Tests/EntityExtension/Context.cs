using Microsoft.EntityFrameworkCore;
using NeinLinq.Fakes.EntityExtension;

namespace NeinLinq.Tests.EntityExtension
{
    public class Context : DbContext
    {
        public DbSet<Dummy> Dummies { get; }

        public Context(DbContextOptions<Context> options)
            : base(options)
        {
            Dummies = Set<Dummy>();
        }
    }
}
