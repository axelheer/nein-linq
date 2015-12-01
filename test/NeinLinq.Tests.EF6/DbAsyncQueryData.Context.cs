using System.Data.Entity;

namespace NeinLinq.Tests.DbAsyncQueryData
{
    [DbConfigurationType(typeof(Config))]
    public class Context : DbContext
    {
        public DbSet<Dummy> Dummies { get; set; }
    }
}
