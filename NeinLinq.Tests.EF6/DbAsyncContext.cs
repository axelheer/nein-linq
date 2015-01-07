using System;
using System.Data.Entity;

namespace NeinLinq.Tests.EF6
{
    public class DbAsyncContext : DbContext
    {
        public DbSet<DbAsyncDummy> Dummies { get; set; }
    }
}
