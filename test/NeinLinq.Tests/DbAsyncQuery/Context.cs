using System.Data.Entity;
using System.IO;
using NeinLinq.Fakes.DbAsyncQuery;

namespace NeinLinq.Tests.DbAsyncQuery
{
    [DbConfigurationType(typeof(Config))]
    public class Context : DbContext
    {
        public DbSet<Dummy> Dummies { get; }

        public Context() : base("Data Source=NeinLinq.EntityFramework.db")
        {
            Dummies = Set<Dummy>();
        }

        public void CreateDatabase(params Dummy[] seed)
        {
            if (File.Exists("NeinLinq.EntityFramework.db"))
                return;

            File.Create("NeinLinq.EntityFramework.db").Close();

            _ = Database.ExecuteSqlCommand(@"
                CREATE TABLE Dummies (
                    Id INTEGER PRIMARY KEY NOT NULL,
                    Name TEXT,
                    Number REAL NOT NULL,
                    OtherId INTEGER NOT NULL
            )");

            _ = Database.ExecuteSqlCommand(@"
                CREATE TABLE OtherDummies (
                    Id INTEGER PRIMARY KEY  NOT NULL,
                    Name TEXT
            )");

            _ = Dummies.AddRange(seed);
            _ = SaveChanges();
        }
    }
}
