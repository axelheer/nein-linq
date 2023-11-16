using Xunit;

namespace NeinLinq.Tests;

public class NullsafeQueryTest
{
    [Fact]
    public void Query_StructMember_SelectsDefault()
    {
        var query = from a in CreateQuery().ToNullsafe()
                    orderby a.SomeNumeric
                    select new ModelView
                    {
                        Year = a.OneDay.Year
                    };

        var result = query.ToList();

        Assert.Collection(result,
            r => Assert.Equal(1, r.Year),
            r => Assert.Equal(1977, r.Year),
            r => Assert.Equal(1980, r.Year),
            r => Assert.Equal(1983, r.Year),
            r => Assert.Equal(2015, r.Year));
    }

    [Fact]
    public void Query_ClassMember_SelectsDefault()
    {
        var query = from a in CreateQuery().ToNullsafe()
                    orderby a.SomeNumeric
                    select new ModelView
                    {
                        Numeric = a.SomeOther.SomeNumeric
                    };

        var result = query.ToList();

        Assert.Collection(result,
            r => Assert.Equal(0, r.Numeric),
            r => Assert.Equal(42, r.Numeric),
            r => Assert.Equal(0, r.Numeric),
            r => Assert.Equal(0, r.Numeric),
            r => Assert.Equal(0, r.Numeric));
    }

    [Fact]
    public void Query_MethodCall_SelectsDefault()
    {
        var query = from a in CreateQuery().ToNullsafe()
                    orderby a.SomeNumeric
                    select new ModelView
                    {
                        Question = a.SomeText.Contains('?')
                    };

        var result = query.ToList();

        Assert.Collection(result,
            r => Assert.False(r.Question),
            r => Assert.False(r.Question),
            r => Assert.True(r.Question),
            r => Assert.False(r.Question),
            r => Assert.False(r.Question));
    }

    [Fact]
    public void Query_ChainedMethodCall_SelectsDefault()
    {
        var query = from a in CreateQuery().ToNullsafe()
                    let firstSpace = a.SomeText.IndexOf(' ')
                    orderby a.SomeNumeric
                    select new ModelView
                    {
                        FirstWord = firstSpace != -1 ? a.SomeText.Remove(firstSpace) : a.SomeText,
                        CharacterCount = a.SomeText.ToCharArray().GetLength(0)
                    };

        var result = query.ToList();

        Assert.Collection(result,
            r => Assert.Null(r.FirstWord),
            r => Assert.Equal("Narf", r.FirstWord),
            r => Assert.Equal("What", r.FirstWord),
            r => Assert.Null(r.FirstWord),
            r => Assert.Equal("", r.FirstWord));

        Assert.Collection(result,
            r => Assert.Equal(0, r.CharacterCount),
            r => Assert.Equal(4, r.CharacterCount),
            r => Assert.Equal(20, r.CharacterCount),
            r => Assert.Equal(0, r.CharacterCount),
            r => Assert.Equal(0, r.CharacterCount));
    }

    [Fact]
    public void Query_EnumerableSubQuery_SelectsEmpty()
    {
        var query = from a in CreateQuery().ToNullsafe()
                    orderby a.SomeNumeric
                    select new ModelView
                    {
                        Other = from b in a.SomeOthers
                                select b.OneDay.Month
                    };

        var result = query.ToList();

        Assert.Collection(result,
            r => Assert.Empty(r.Other),
            r => Assert.Empty(r.Other),
            r => Assert.Equal([1, 3, 6], r.Other),
            r => Assert.Empty(r.Other),
            r => Assert.Empty(r.Other));
    }

    [Fact]
    public void Query_CollectionSubQuery_SelectsEmpty()
    {
        var query = from a in CreateQuery().ToNullsafe()
                    orderby a.SomeNumeric
                    select new ModelView
                    {
                        More = from c in a.MoreOthers
                               select c.SomeOther.OneDay.Day
                    };

        var result = query.ToList();

        Assert.Collection(result,
            r => Assert.Empty(r.More),
            r => Assert.Empty(r.More),
            r => Assert.Empty(r.More),
            r => Assert.Equal([1, 1, 5, 8], r.More),
            r => Assert.Empty(r.More));
    }

    [Fact]
    public void Query_SetSubQuery_SelectsEmpty()
    {
        var query = from a in CreateQuery().ToNullsafe()
                    orderby a.SomeNumeric
                    select new ModelView
                    {
                        Lot = from d in a.EvenLotMoreOthers
                              select d.SomeOther.OneDay.Day
                    };

        var result = query.ToList();

        Assert.Collection(result,
            r => Assert.Empty(r.Lot),
            r => Assert.Empty(r.Lot),
            r => Assert.Empty(r.Lot),
            r => Assert.Empty(r.Lot),
            r => Assert.Equal([1, 1, 4, 7], r.Lot));
    }

    [Fact]
    public void Query_NullableMember_SelectsDefault()
    {
        var query = from a in CreateQuery().ToNullsafe()
                    orderby a.SomeNumeric
                    select new ModelView
                    {
                        Numeric = a.DaNullable!.Value
                    };

        var result = query.ToList();

        Assert.Collection(result,
            r => Assert.Equal(0, r.Numeric),
            r => Assert.Equal(2017, r.Numeric),
            r => Assert.Equal(0, r.Numeric),
            r => Assert.Equal(0, r.Numeric),
            r => Assert.Equal(0, r.Numeric));
    }

    [Fact]
    public void Query_ExtensionMethod_SelectsDefault()
    {
        var query = from a in CreateQuery().ToNullsafe()
                    group a by a.MoreOthers.Count into g
                    where g.Key > 0
                    select g.Sum(b => b.MoreOthers.Sum(c => c.SomeOther.OneDay.Month));

        var result = query.ToList();

        Assert.Equal([4], result);
    }

    [Fact]
    public void Query_StaticMembers_Ignores()
    {
        var query = from _ in Enumerable.Range(1, 3).AsQueryable().ToNullsafe()
                    select new
                    {
                        Property = Guid.Empty,
                        Method = Guid.NewGuid()
                    };

        var result = query.ToList();

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public void Query_ForeignMember_Handles()
    {
        string danger = default!;

        var query = from a in CreateQuery().ToNullsafe()
                    orderby a.SomeNumeric
                    select new ModelView
                    {
                        Numeric = danger.Length
                    };

        var result = query.ToList();

        Assert.Collection(result,
            r => Assert.Equal(0, r.Numeric),
            r => Assert.Equal(0, r.Numeric),
            r => Assert.Equal(0, r.Numeric),
            r => Assert.Equal(0, r.Numeric),
            r => Assert.Equal(0, r.Numeric));
    }

    [Fact]
    public void Query_LetStatements_Handles()
    {
        var query = from a in CreateQuery().ToNullsafe()
                    let x = a.SomeOthers.FirstOrDefault()
                    let y = a.MoreOthers.FirstOrDefault()
                    let z = a.EvenLotMoreOthers.FirstOrDefault()
                    orderby a.SomeNumeric
                    select new ModelView
                    {
                        Year = x.OneDay.Year,
                        Numeric = y.SomeOther.SomeNumeric,
                        Question = z.SomeText.Contains('?')
                    };

        _ = query.ToList();

        Assert.True(true);
    }

    private static IQueryable<Model> CreateQuery()
    {
        var data = new[]
        {
            new Model
            {
                SomeNumeric = 7,
                SomeText = "Narf",
                OneDay = new DateTime(1977, 05, 25),
                SomeOther = new Model { SomeNumeric = 42 },
                DaNullable = 2017
            },
            new Model
            {
                SomeNumeric = 1138,
                SomeText = "What is thy bidding?",
                OneDay = new DateTime(1980, 05, 21),
                SomeOthers = new[]
                {
                    null!,
                    new Model { OneDay = new DateTime(2000, 3, 1) },
                    new Model { OneDay = new DateTime(2000, 6, 1) }
                }
            },
            new Model
            {
                SomeNumeric = 123456,
                OneDay = new DateTime(1983, 05, 25),
                MoreOthers = new[]
                {
                    null!,
                    new Model(),
                    new Model { SomeOther = new Model { OneDay = new DateTime(2000, 1, 5) } },
                    new Model { SomeOther = new Model { OneDay = new DateTime(2000, 1, 8) } }
                }
            },
            new Model
            {
                SomeNumeric = 654321,
                SomeText = "",
                OneDay = new DateTime(2015, 12, 18),
                EvenLotMoreOthers = new HashSet<Model>
                {
                    null!,
                    new Model(),
                    new Model { SomeOther = new Model { OneDay = new DateTime(2000, 1, 4) } },
                    new Model { SomeOther = new Model { OneDay = new DateTime(2000, 1, 7) } }
                }
            },
            null!
        };

        return data.AsQueryable();
    }

    private class Model
    {
        public int SomeNumeric { get; set; }

        public string SomeText { get; set; } = null!;

        public DateTime OneDay { get; set; }

        public Model SomeOther { get; set; } = null!;

        public int? DaNullable { get; set; }

        public IEnumerable<Model> SomeOthers { get; set; } = null!;

        public ICollection<Model> MoreOthers { get; set; } = null!;

        public ISet<Model> EvenLotMoreOthers { get; set; } = null!;
    }

    private class ModelView
    {
        public int Year { get; set; }

        public int Numeric { get; set; }

        public bool Question { get; set; }

        public string? FirstWord { get; set; }

        public int CharacterCount { get; set; }

        public IEnumerable<int> Other { get; set; } = null!;

        public IEnumerable<int> More { get; set; } = null!;

        public IEnumerable<int> Lot { get; set; } = null!;
    }
}
