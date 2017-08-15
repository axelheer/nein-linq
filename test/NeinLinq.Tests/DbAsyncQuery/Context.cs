#if NETFRAMEWORK

using NeinLinq.Fakes.DbAsyncQuery;
using System.Data.Entity;

namespace NeinLinq.Tests.DbAsyncQuery
{
    [DbConfigurationType(typeof(Config))]
    public class Context : DbContext
    {
        public DbSet<Dummy> Dummies { get; set; }

        public Context () : base("NeinLinq.EntityFramework")
        {
        }

        public void ResetDatabase()
        {
            Dummies.RemoveRange(Dummies);
            SaveChanges();
        }
    }
}

#endif
