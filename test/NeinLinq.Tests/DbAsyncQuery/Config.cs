using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Data.SQLite;
using System.Data.SQLite.EF6;

namespace NeinLinq.Tests.DbAsyncQuery
{
    public class Config : DbConfiguration
    {
        public Config()
        {
            AddDependencyResolver(SQLiteDependencyResolver.Instance);
        }

        private class SQLiteDependencyResolver : IDbDependencyResolver
        {
            public static IDbDependencyResolver Instance { get; }
                = new SQLiteDependencyResolver();

            public object GetService(Type type, object key)
            {
                if (type == typeof(DbProviderFactory))
                    return SQLiteProviderFactory.Instance;
                if (type == typeof(IDbProviderFactoryResolver))
                    return SQLiteDbProviderFactoryResolver.Instance;
                if (type == typeof(IProviderInvariantName))
                    return SQLiteProviderInvariantName.Instance;
                return SQLiteProviderFactory.Instance.GetService(type);
            }

            public IEnumerable<object> GetServices(Type type, object key)
            {
                var service = GetService(type, key);
                if (service != null)
                    yield return service;
            }
        }

        private class SQLiteDbProviderFactoryResolver : IDbProviderFactoryResolver
        {
            public static IDbProviderFactoryResolver Instance { get; }
                = new SQLiteDbProviderFactoryResolver();

            public DbProviderFactory ResolveProviderFactory(DbConnection connection)
            {
                if (connection is SQLiteConnection)
                    return SQLiteProviderFactory.Instance;
                if (connection is EntityConnection)
                    return EntityProviderFactory.Instance;
                return null!;
            }
        }

        private class SQLiteProviderInvariantName : IProviderInvariantName
        {
            public static IProviderInvariantName Instance { get; }
                = new SQLiteProviderInvariantName();

            public string Name { get; } = "System.Data.SQLite.EF6";
        }
    }
}
