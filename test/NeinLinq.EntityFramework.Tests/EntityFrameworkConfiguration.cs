using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Infrastructure;
using System.Data.SQLite;
using System.Data.SQLite.EF6;

namespace NeinLinq.Tests;

public class TestConfiguration : DbConfiguration, IDbConnectionFactory
{
    public TestConfiguration()
    {
        SetProviderFactory("System.Data.SQLite", SQLiteFactory.Instance);
        SetProviderFactory("System.Data.SQLite.EF6", SQLiteProviderFactory.Instance);

        SetProviderServices("System.Data.SQLite", (DbProviderServices)SQLiteProviderFactory.Instance.GetService(typeof(DbProviderServices)));
        SetProviderServices("System.Data.SQLite.EF6", (DbProviderServices)SQLiteProviderFactory.Instance.GetService(typeof(DbProviderServices)));

        SetDefaultConnectionFactory(this);
    }

    public DbConnection CreateConnection(string nameOrConnectionString)
        => new SQLiteConnection(nameOrConnectionString);
}
