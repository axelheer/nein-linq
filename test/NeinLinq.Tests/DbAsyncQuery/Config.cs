using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Infrastructure;
using System.Data.SQLite;
using System.Data.SQLite.EF6;
using Microsoft.Extensions.DependencyInjection;

namespace NeinLinq.Tests.DbAsyncQuery
{
    public class Config : DbConfiguration, IDbConnectionFactory
    {
        public Config()
        {
            SetProviderFactory("System.Data.SQLite", SQLiteFactory.Instance);
            SetProviderFactory("System.Data.SQLite.EF6", SQLiteProviderFactory.Instance);

            SetProviderServices("System.Data.SQLite", SQLiteProviderFactory.Instance.GetRequiredService<DbProviderServices>());
            SetProviderServices("System.Data.SQLite.EF6", SQLiteProviderFactory.Instance.GetRequiredService<DbProviderServices>());

            SetDefaultConnectionFactory(this);
        }

        public DbConnection CreateConnection(string connectionString)
            => new SQLiteConnection(connectionString);
    }
}
