#if EF6

using System;
using System.Data.Entity;

namespace NeinLinq.Tests.DbAsync
{
    [DbConfigurationType(typeof(Config))]
    public class Context : DbContext
    {
        public DbSet<Dummy> Dummies { get; set; }
    }
}

#endif
