using System.Data.Entity;
using NeinLinq.Fakes.DbAsyncQuery;

namespace NeinLinq.Tests.DbAsyncQuery
{
    [DbConfigurationType(typeof(Config))]
    public class Context : DbContext
    {
        public DbSet<Dummy> Dummies { get; }

        public Context() : base("NeinLinq.EntityFramework")
        {
            Dummies = Set<Dummy>();
        }

        public void ResetDatabase()
        {
            _ = Dummies.RemoveRange(Dummies);
            _ = SaveChanges();
        }
    }
}
