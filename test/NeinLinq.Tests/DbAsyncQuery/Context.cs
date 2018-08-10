using System.Data.Entity;
using NeinLinq.Fakes.DbAsyncQuery;

namespace NeinLinq.Tests.DbAsyncQuery
{
    [DbConfigurationType(typeof(Config))]
    public class Context : DbContext
    {
        public DbSet<Dummy> Dummies { get; set; }

        public Context() : base("NeinLinq.EntityFramework")
        {
        }

        public void ResetDatabase()
        {
            Dummies.RemoveRange(Dummies);
            SaveChanges();
        }
    }
}
