using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace NeinLinq.Tests.DbAsyncQueryData
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
