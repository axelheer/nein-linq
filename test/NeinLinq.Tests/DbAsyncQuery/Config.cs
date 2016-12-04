#if EF

using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace NeinLinq.Tests.DbAsyncQuery
{
    public class Config : DbConfiguration
    {
        public Config()
        {
            SetDatabaseInitializer(new DropCreateDatabaseAlways<Context>());
            SetDefaultConnectionFactory(new LocalDbConnectionFactory("MSSQLLocalDB"));
        }
    }
}

#endif
