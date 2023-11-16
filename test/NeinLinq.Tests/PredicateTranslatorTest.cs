using Xunit;

namespace NeinLinq.Tests;

public class PredicateTranslatorTest
{
    [Fact]
    public void Translate_NullArgument_Throws()
    {
        var error = Assert.Throws<ArgumentNullException>(()
            => PredicateTranslator.Translate<Model>(null!));

        Assert.Equal("predicate", error.ParamName);
    }

    [Fact]
    public void And_NullArgument_Throws()
    {
        Expression<Func<IModel, bool>> p = _ => false;
        Expression<Func<IModel, bool>> q = _ => false;

        var leftError = Assert.Throws<ArgumentNullException>(()
            => PredicateTranslator.And(null!, q));
        var rightError = Assert.Throws<ArgumentNullException>(()
            => PredicateTranslator.And(p, null!));

        Assert.Equal("left", leftError.ParamName);
        Assert.Equal("right", rightError.ParamName);
    }

    [Fact]
    public void And_Combines()
    {
        Expression<Func<IModel, bool>> p = d => d.Id % 2 == 1;
        Expression<Func<IModel, bool>> q = d => d.Name == "Narf";

        var r = CreateQuery().Where(p).Count();
        var s = CreateQuery().Where(q).Count();
        var t = CreateQuery().Where(p.And(q)).Count();

        Assert.Equal(6, r);
        Assert.Equal(4, s);
        Assert.Equal(2, t);
    }

    [Fact]
    public void Or_NullArgument_Throws()
    {
        Expression<Func<IModel, bool>> p = _ => false;
        Expression<Func<IModel, bool>> q = _ => false;

        var leftError = Assert.Throws<ArgumentNullException>(()
            => PredicateTranslator.Or(null!, q));
        var rightError = Assert.Throws<ArgumentNullException>(()
            => PredicateTranslator.Or(p, null!));

        Assert.Equal("left", leftError.ParamName);
        Assert.Equal("right", rightError.ParamName);
    }

    [Fact]
    public void Or_Combines()
    {
        Expression<Func<IModel, bool>> p = d => d.Id % 2 == 1;
        Expression<Func<IModel, bool>> q = d => d.Name == "Narf";

        var r = CreateQuery().Where(p).Count();
        var s = CreateQuery().Where(q).Count();
        var t = CreateQuery().Where(p.Or(q)).Count();

        Assert.Equal(6, r);
        Assert.Equal(4, s);
        Assert.Equal(8, t);
    }

    [Fact]
    public void Not_NullArgument_Throws()
    {
        var error = Assert.Throws<ArgumentNullException>(()
            => PredicateTranslator.Not<IModel>(null!));

        Assert.Equal("predicate", error.ParamName);
    }

    [Fact]
    public void Not_Negates()
    {
        Expression<Func<IModel, bool>> p = d => d.Name == "Narf";

        var r = CreateQuery().Where(p).Count();
        var s = CreateQuery().Where(p.Not()).Count();

        Assert.Equal(4, r);
        Assert.Equal(8, s);
    }

    [Fact]
    public void To_Substitutes()
    {
        Expression<Func<Model, bool>> p = d => d.Name == "Narf";

        var r = CreateQuery().OfType<Model>().Where(p).Count();
        var s = CreateQuery().OfType<SpecialModel>().Where(p.Translate().To<SpecialModel>()).Count();

        Assert.Equal(2, r);
        Assert.Equal(1, s);
    }

    [Fact]
    public void ToPath_NullArgument_Throws()
    {
        Expression<Func<ParentModel, bool>> p = _ => false;

        var error = Assert.Throws<ArgumentNullException>(()
            => p.Translate().To((Expression<Func<ChildModel, ParentModel>>)null!));

        Assert.Equal("path", error.ParamName);
    }

    [Fact]
    public void ToPath_Substitutes()
    {
        Expression<Func<ParentModel, bool>> p = d => d.Name == "Narf";

        var r = CreateQuery().OfType<ParentModel>().Where(p).Count();
        var s = CreateQuery().OfType<ChildModel>().Where(p.Translate().To<ChildModel>(c => c.Parent)).Count();

        Assert.Equal(1, r);
        Assert.Equal(1, s);
    }

    [Fact]
    public void ToTranslation_NullArgument_Throws()
    {
        Expression<Func<ChildModel, bool>> p = _ => false;

        var error = Assert.Throws<ArgumentNullException>(()
            => p.Translate().To((Expression<Func<ParentModel, Func<ChildModel, bool>, bool>>)null!));

        Assert.Equal("translation", error.ParamName);
    }

    [Fact]
    public void ToTranslation_Substitutes()
    {
        Expression<Func<ChildModel, bool>> p = d => d.Name == "Narf";

        var r = CreateQuery().OfType<ChildModel>().Where(p).Count();
        var s = CreateQuery().OfType<ParentModel>().Where(p.Translate().To<ParentModel>((b, q) => b.Children.Any(q))).Count();

        Assert.Equal(1, r);
        Assert.Equal(2, s);
    }

    private static IQueryable<IModel> CreateQuery()
    {
        var d = new[]
        {
            new Model { Id = 1, Name = "Asdf" },
            new Model { Id = 2, Name = "Narf" },
            new Model { Id = 3, Name = "Qwer" }
        };

        var s = new[]
        {
            new SpecialModel { Id = 4, Name = "Asdf" },
            new SpecialModel { Id = 5, Name = "Narf" },
            new SpecialModel { Id = 6, Name = "Qwer" }
        };

        var p = new[]
        {
            new ParentModel { Id = 7, Name = "Asdf" },
            new ParentModel { Id = 8, Name = "Narf" },
            new ParentModel { Id = 9, Name = "Qwer" }
        };

        var c = new[]
        {
            new ChildModel { Id = 10, Name = "Asdf", Parent = p[1] },
            new ChildModel { Id = 11, Name = "Narf", Parent = p[2] },
            new ChildModel { Id = 12, Name = "Qwer", Parent = p[0] }
        };

        p[0].Children = new[] { c[0], c[1] };
        p[1].Children = new[] { c[1], c[2] };
        p[2].Children = new[] { c[0], c[2] };

        return d.Concat<IModel>(s).Concat(p).Concat(c).AsQueryable();
    }

    private interface IModel
    {
        int Id { get; set; }

        string Name { get; set; }
    }

    private class Model : IModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;
    }

    private class SpecialModel : Model
    {
        public string Description { get; set; } = null!;
    }

    private class ParentModel : IModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public ICollection<ChildModel> Children { get; set; } = null!;
    }

    private class ChildModel : IModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public ParentModel Parent { get; set; } = null!;
    }
}
