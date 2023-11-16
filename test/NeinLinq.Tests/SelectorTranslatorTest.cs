using Xunit;

namespace NeinLinq.Tests;

public class SelectorTranslatorTest
{
    [Fact]
    public void Translate_NullArgument_Throws()
    {
        var error = Assert.Throws<ArgumentNullException>(()
            => SelectorTranslator.Translate<Model, ModelView>(null!));

        Assert.Equal("selector", error.ParamName);
    }

    [Fact]
    public void Apply_NullArgument_Throws()
    {
        Expression<Func<Model, ModelView>> s = _ => new ModelView { Id = 1 };
        Expression<Func<Model, ModelView>> t = _ => new ModelView { Id = 1 };

        var leftError = Assert.Throws<ArgumentNullException>(()
            => SelectorTranslator.Apply(null!, t));
        var rightError = Assert.Throws<ArgumentNullException>(()
            => SelectorTranslator.Apply(s, null!));

        Assert.Equal("left", leftError.ParamName);
        Assert.Equal("right", rightError.ParamName);
    }

    [Fact]
    public void Apply_NullInit_Throws()
    {
        Expression<Func<Model, ModelView>> s = _ => new ModelView { Id = 1 };
        Expression<Func<Model, ModelView>> t = _ => new ModelView { Id = 1 };

        var leftError = Assert.Throws<NotSupportedException>(()
            => SelectorTranslator.Apply(_ => null!, t));
        var rightError = Assert.Throws<NotSupportedException>(()
            => SelectorTranslator.Apply(s, _ => null!));

        Assert.Equal("Only member init expressions and new expressions are supported yet.", leftError.Message);
        Assert.Equal("Only member init expressions and new expressions are supported yet.", rightError.Message);
    }

    [Fact]
    public void Apply_InitWithParam_Throws()
    {
        Expression<Func<Model, ModelView>> s = _ => new ModelView { Id = 1 };
        Expression<Func<Model, ModelView>> t = _ => new ModelView { Id = 1 };

        var leftError = Assert.Throws<NotSupportedException>(()
            => SelectorTranslator.Apply(_ => new ModelView(1), t));
        var rightError = Assert.Throws<NotSupportedException>(()
            => SelectorTranslator.Apply(s, _ => new ModelView(1)));

        Assert.Equal("Only parameterless constructors are supported yet.", leftError.Message);
        Assert.Equal("Only parameterless constructors are supported yet.", rightError.Message);
    }

    [Fact]
    public void Apply_Merges()
    {
        Expression<Func<Model, ModelView>> s = d => new ModelView { Id = d.Id };
        Expression<Func<Model, ModelView>> t = d => new ModelView { Name = d.Name };

        var select = s.Apply(t);
        var result = CreateQuery().OfType<Model>().Where(m => !(m is SpecialModel)).Select(select);

        Assert.Collection(result,
            v => { Assert.Equal(1, v.Id); Assert.Equal("Asdf", v.Name); },
            v => { Assert.Equal(2, v.Id); Assert.Equal("Narf", v.Name); },
            v => { Assert.Equal(3, v.Id); Assert.Equal("Qwer", v.Name); });
    }

    [Fact]
    public void Apply_WithOverride_Merges()
    {
        Expression<Func<Model, ModelView>> s = d => new ModelView { Id = d.Id, Name = d.Name };
        Expression<Func<Model, ModelView>> t = d => new ModelView { Name = d.Name + " (!)" };

        var select = s.Apply(t);
        var result = CreateQuery().OfType<Model>().Where(m => !(m is SpecialModel)).Select(select);

        Assert.Collection(result,
            v => { Assert.Equal(1, v.Id); Assert.Equal("Asdf (!)", v.Name); },
            v => { Assert.Equal(2, v.Id); Assert.Equal("Narf (!)", v.Name); },
            v => { Assert.Equal(3, v.Id); Assert.Equal("Qwer (!)", v.Name); });
    }

    [Fact]
    public void Apply_EmptyInit_Merges()
    {
        Expression<Func<Model, ModelView>> s = _ => new ModelView();
        Expression<Func<Model, ModelView>> t = _ => new ModelView();

        var select = s.Apply(t);
        var result = CreateQuery().OfType<Model>().Where(m => !(m is SpecialModel)).Select(select);

        Assert.Collection(result,
            v => { Assert.Equal(0, v.Id); Assert.Null(v.Name); },
            v => { Assert.Equal(0, v.Id); Assert.Null(v.Name); },
            v => { Assert.Equal(0, v.Id); Assert.Null(v.Name); });
    }

    [Fact]
    public void Apply_EmptyNotEmptyInit_Merges()
    {
        Expression<Func<Model, ModelView>> s = _ => new ModelView();
        Expression<Func<Model, ModelView>> t = d => new ModelView
        {
            Id = d.Id + 5,
            Name = d.Name
        };

        var select = s.Apply(t);
        var result = CreateQuery().OfType<Model>().Where(m => !(m is SpecialModel)).Select(select);

        Assert.Collection(result,
            v => { Assert.Equal(6, v.Id); Assert.Equal("Asdf", v.Name); },
            v => { Assert.Equal(7, v.Id); Assert.Equal("Narf", v.Name); },
            v => { Assert.Equal(8, v.Id); Assert.Equal("Qwer", v.Name); }
        );
    }

    [Fact]
    public void Apply_NotEmptyEmptyInit_Merges()
    {
        Expression<Func<Model, ModelView>> s = _ => new ModelView();
        Expression<Func<Model, ModelView>> t = d => new ModelView
        {
            Id = d.Id + 5,
            Name = d.Name
        };

        var select = t.Apply(s);
        var result = CreateQuery().OfType<Model>().Where(m => !(m is SpecialModel)).Select(select);

        Assert.Collection(result,
            v => { Assert.Equal(6, v.Id); Assert.Equal("Asdf", v.Name); },
            v => { Assert.Equal(7, v.Id); Assert.Equal("Narf", v.Name); },
            v => { Assert.Equal(8, v.Id); Assert.Equal("Qwer", v.Name); }
        );
    }

    [Fact]
    public void Source_Substitutes()
    {
        Expression<Func<Model, ModelView>> s = d => new ModelView { Id = d.Id, Name = d.Name };

        var select = s.Translate().Source<SpecialModel>();
        var result = CreateQuery().OfType<SpecialModel>().Select(select);

        Assert.Collection(result,
            v => { Assert.Equal(4, v.Id); Assert.Equal("Asdf", v.Name); },
            v => { Assert.Equal(5, v.Id); Assert.Equal("Narf", v.Name); },
            v => { Assert.Equal(6, v.Id); Assert.Equal("Qwer", v.Name); });
    }

    [Fact]
    public void SourcePath_NullArgument_Throws()
    {
        Expression<Func<ParentModel, ParentModelView>> s = _ => new ParentModelView();

        var error = Assert.Throws<ArgumentNullException>(()
            => s.Translate().Source((Expression<Func<ChildModel, ParentModel>>)null!));

        Assert.Equal("path", error.ParamName);
    }

    [Fact]
    public void SourcePath_Substitutes()
    {
        Expression<Func<ParentModel, ParentModelView>> s = d => new ParentModelView { Id = d.Id, Name = d.Name };

        var select = s.Translate().Source<ChildModel>(d => d.Parent);
        var result = CreateQuery().OfType<ChildModel>().Select(select);

        Assert.Collection(result,
            v => { Assert.Equal(8, v.Id); Assert.Equal("Narf", v.Name); },
            v => { Assert.Equal(9, v.Id); Assert.Equal("Qwer", v.Name); },
            v => { Assert.Equal(7, v.Id); Assert.Equal("Asdf", v.Name); });
    }

    [Fact]
    public void SourceTranslation_NullArgument_Throws()
    {
        Expression<Func<ChildModel, ChildModelView>> s = _ => new ChildModelView();

        var error = Assert.Throws<ArgumentNullException>(()
            => s.Translate().Source((Expression<Func<ParentModel, Func<ChildModel, ChildModelView>, ChildModelView>>)null!));

        Assert.Equal("translation", error.ParamName);
    }

    [Fact]
    public void SourceTranslation_Substitutes()
    {
        Expression<Func<ChildModel, ChildModelView>> s = d => new ChildModelView { Id = d.Id, Name = d.Name };

        var select = s.Translate().Source<ParentModel>((d, v) => d.Children.Select(v).First());
        var result = CreateQuery().OfType<ParentModel>().Select(select);

        Assert.Collection(result,
            v => { Assert.Equal(10, v.Id); Assert.Equal("Asdf", v.Name); },
            v => { Assert.Equal(11, v.Id); Assert.Equal("Narf", v.Name); },
            v => { Assert.Equal(12, v.Id); Assert.Equal("Qwer", v.Name); });
    }

    [Fact]
    public void SourceCollectionTranslation_NullArgument_Throws()
    {
        Expression<Func<ChildModel, ChildModelView>> s = _ => new ChildModelView();

        var error = Assert.Throws<ArgumentNullException>(()
            => s.Translate().Source((Expression<Func<ParentModel, Func<ChildModel, ChildModelView>, IEnumerable<ChildModelView>>>)null!));

        Assert.Equal("translation", error.ParamName);
    }

    [Fact]
    public void SourceCollectionTranslation_Substitutes()
    {
        Expression<Func<ChildModel, ChildModelView>> s = d => new ChildModelView { Id = d.Id, Name = d.Name };

        var select = s.Translate().Source<ParentModel>((d, v) => d.Children.Select(v));
        var result = CreateQuery().OfType<ParentModel>().Select(select);

        Assert.Collection(result,
            v => Assert.Collection(v,
                w => { Assert.Equal(10, w.Id); Assert.Equal("Asdf", w.Name); },
                w => { Assert.Equal(11, w.Id); Assert.Equal("Narf", w.Name); }),
            v => Assert.Collection(v,
                w => { Assert.Equal(11, w.Id); Assert.Equal("Narf", w.Name); },
                w => { Assert.Equal(12, w.Id); Assert.Equal("Qwer", w.Name); }),
            v => Assert.Collection(v,
                w => { Assert.Equal(12, w.Id); Assert.Equal("Qwer", w.Name); },
                w => { Assert.Equal(10, w.Id); Assert.Equal("Asdf", w.Name); }));
    }

    [Fact]
    public void Result_NullInit_Throws()
    {
        Expression<Func<SpecialModel, ModelView>> s = _ => null!;

        var error = Assert.Throws<NotSupportedException>(()
            => s.Translate().Result<SpecialModelView>());

        Assert.Equal("Only member init expressions are supported yet.", error.Message);
    }

    [Fact]
    public void Result_InitWithParam_Throws()
    {
        Expression<Func<SpecialModel, ModelView>> s = _ => new ModelView(1) { Name = "Narf" };

        var error = Assert.Throws<NotSupportedException>(()
            => s.Translate().Result<SpecialModelView>());

        Assert.Equal("Only parameterless constructors are supported yet.", error.Message);
    }

    [Fact]
    public void Result_Substitutes()
    {
        Expression<Func<SpecialModel, ModelView>> s = d => new ModelView { Id = d.Id, Name = d.Name };

        var select = s.Translate().Result<SpecialModelView>();
        var result = CreateQuery().OfType<SpecialModel>().Select(select);

        Assert.Collection(result,
            v => { Assert.Equal(4, v.Id); Assert.Equal("Asdf", v.Name); Assert.Null(v.Description); },
            v => { Assert.Equal(5, v.Id); Assert.Equal("Narf", v.Name); Assert.Null(v.Description); },
            v => { Assert.Equal(6, v.Id); Assert.Equal("Qwer", v.Name); Assert.Null(v.Description); });
    }

    [Fact]
    public void ResultPath_NullArgument_Throws()
    {
        Expression<Func<ChildModel, ParentModelView>> s = _ => new ParentModelView();

        var error = Assert.Throws<ArgumentNullException>(()
            => s.Translate().Result((Expression<Func<ChildModelView, ParentModelView>>)null!));

        Assert.Equal("path", error.ParamName);
    }

    [Fact]
    public void ResultPath_NullInit_Throws()
    {
        Expression<Func<ChildModel, ParentModelView>> s = _ => new ParentModelView();

        var error = Assert.Throws<NotSupportedException>(()
            => s.Translate().Result<ChildModelView>(_ => null!));

        Assert.Equal("Only member expressions are supported yet.", error.Message);
    }

    [Fact]
    public void ResultPath_Substitutes()
    {
        Expression<Func<ChildModel, ParentModelView>> s = d => new ParentModelView { Id = d.Parent!.Id, Name = d.Parent.Name };

        var select = s.Translate().Result<ChildModelView>(d => d.Parent);
        var result = CreateQuery().OfType<ChildModel>().Select(select);

        Assert.Collection(result,
            v => { Assert.Equal(8, v.Parent.Id); Assert.Equal("Narf", v.Parent.Name); },
            v => { Assert.Equal(9, v.Parent.Id); Assert.Equal("Qwer", v.Parent.Name); },
            v => { Assert.Equal(7, v.Parent.Id); Assert.Equal("Asdf", v.Parent.Name); });
    }

    [Fact]
    public void ResultTranslation_NullArgument_Throws()
    {
        Expression<Func<ParentModel, IEnumerable<ChildModelView>>> s = _ => Array.Empty<ChildModelView>();

        var error = Assert.Throws<ArgumentNullException>(()
            => s.Translate().Result((Expression<Func<ParentModel, Func<ParentModel, IEnumerable<ChildModelView>>, ParentModelView>>)null!));

        Assert.Equal("translation", error.ParamName);
    }

    [Fact]
    public void ResultTranslation_Substitutes()
    {
        Expression<Func<ParentModel, IEnumerable<ChildModelView>>> s = d => d.Children!.Select(e => new ChildModelView { Id = e.Id, Name = e.Name });

        var select = s.Translate().Result((d, v) => new ParentModelView { FirstChild = v(d).First() });
        var result = CreateQuery().OfType<ParentModel>().Select(select);

        Assert.Collection(result,
            v => { Assert.Equal(10, v.FirstChild.Id); Assert.Equal("Asdf", v.FirstChild.Name); },
            v => { Assert.Equal(11, v.FirstChild.Id); Assert.Equal("Narf", v.FirstChild.Name); },
            v => { Assert.Equal(12, v.FirstChild.Id); Assert.Equal("Qwer", v.FirstChild.Name); });
    }

    [Fact]
    public void Cross_Substitutes()
    {
        Expression<Func<Model, ModelView>> s = d => new ModelView { Id = d.Id, Name = d.Name };
        Expression<Func<SpecialModel, SpecialModelView>> t = d => new SpecialModelView { Description = d.Description };

        var select = s.Translate().Cross<SpecialModel>().Apply(t);
        var result = CreateQuery().OfType<SpecialModel>().Select(select);

        Assert.Collection(result,
            v => { Assert.Equal(4, v.Id); Assert.Equal("Asdf", v.Name); Assert.Equal("Asdf", v.Description); },
            v => { Assert.Equal(5, v.Id); Assert.Equal("Narf", v.Name); Assert.Equal("Narf", v.Description); },
            v => { Assert.Equal(6, v.Id); Assert.Equal("Qwer", v.Name); Assert.Equal("Qwer", v.Description); });
    }

    [Fact]
    public void CrossPath_Substitutes()
    {
        Expression<Func<ParentModel, ParentModelView>> s = d => new ParentModelView { Id = d.Id, Name = d.Name };
        Expression<Func<ChildModel, ChildModelView>> t = d => new ChildModelView { Id = d.Id, Name = d.Name };

        var select = s.Translate().Cross<ChildModel>(d => d.Parent).Apply(d => d.Parent, t);
        var result = CreateQuery().OfType<ChildModel>().Select(select);

        Assert.Collection(result,
            v => { Assert.Equal(10, v.Id); Assert.Equal("Asdf", v.Name); Assert.Equal(8, v.Parent.Id); Assert.Equal("Narf", v.Parent.Name); },
            v => { Assert.Equal(11, v.Id); Assert.Equal("Narf", v.Name); Assert.Equal(9, v.Parent.Id); Assert.Equal("Qwer", v.Parent.Name); },
            v => { Assert.Equal(12, v.Id); Assert.Equal("Qwer", v.Name); Assert.Equal(7, v.Parent.Id); Assert.Equal("Asdf", v.Parent.Name); });
    }

    [Fact]
    public void CrossTranslation_Substitutes()
    {
        Expression<Func<ChildModel, ChildModelView>> s = d => new ChildModelView { Id = d.Id, Name = d.Name };
        Expression<Func<ParentModel, ParentModelView>> t = d => new ParentModelView { Id = d.Id, Name = d.Name };

        var select = s.Translate().Cross<ParentModel>((d, v) => d.Children.Select(v).First()).Apply((d, v) => new ParentModelView { FirstChild = v(d) }, t);
        var result = CreateQuery().OfType<ParentModel>().Select(select);

        Assert.Collection(result,
            v => { Assert.Equal(7, v.Id); Assert.Equal("Asdf", v.Name); Assert.Equal(10, v.FirstChild.Id); Assert.Equal("Asdf", v.FirstChild.Name); },
            v => { Assert.Equal(8, v.Id); Assert.Equal("Narf", v.Name); Assert.Equal(11, v.FirstChild.Id); Assert.Equal("Narf", v.FirstChild.Name); },
            v => { Assert.Equal(9, v.Id); Assert.Equal("Qwer", v.Name); Assert.Equal(12, v.FirstChild.Id); Assert.Equal("Qwer", v.FirstChild.Name); });
    }

    [Fact]
    public void CrossCollectionTranslation_Substitutes()
    {
        Expression<Func<ChildModel, ChildModelView>> s = d => new ChildModelView { Id = d.Id, Name = d.Name };
        Expression<Func<ParentModel, ParentModelView>> t = d => new ParentModelView { Id = d.Id, Name = d.Name };

        var select = s.Translate().Cross<ParentModel>((d, v) => d.Children.Select(v)).Apply((d, v) => new ParentModelView { FirstChild = v(d).First() }, t);
        var result = CreateQuery().OfType<ParentModel>().Select(select);

        Assert.Collection(result,
            v => { Assert.Equal(7, v.Id); Assert.Equal("Asdf", v.Name); Assert.Equal(10, v.FirstChild.Id); Assert.Equal("Asdf", v.FirstChild.Name); },
            v => { Assert.Equal(8, v.Id); Assert.Equal("Narf", v.Name); Assert.Equal(11, v.FirstChild.Id); Assert.Equal("Narf", v.FirstChild.Name); },
            v => { Assert.Equal(9, v.Id); Assert.Equal("Qwer", v.Name); Assert.Equal(12, v.FirstChild.Id); Assert.Equal("Qwer", v.FirstChild.Name); });
    }

    [Fact]
    public void To_Substitutes()
    {
        Expression<Func<Model, ModelView>> s = d => new ModelView { Id = d.Id, Name = d.Name };
        Expression<Func<SpecialModel, SpecialModelView>> t = d => new SpecialModelView { Description = d.Description };

        var select = s.Translate().To(t);
        var result = CreateQuery().OfType<SpecialModel>().Select(select);

        Assert.Collection(result,
            v => { Assert.Equal(4, v.Id); Assert.Equal("Asdf", v.Name); Assert.Equal("Asdf", v.Description); },
            v => { Assert.Equal(5, v.Id); Assert.Equal("Narf", v.Name); Assert.Equal("Narf", v.Description); },
            v => { Assert.Equal(6, v.Id); Assert.Equal("Qwer", v.Name); Assert.Equal("Qwer", v.Description); });
    }

    [Fact]
    public void To_WithoutValue_Substitutes()
    {
        Expression<Func<Model, ModelView>> s = d => new ModelView { Id = d.Id, Name = d.Name };

        var select = s.Translate().To<SpecialModel, SpecialModelView>();
        var result = CreateQuery().OfType<SpecialModel>().Select(select);

        Assert.Collection(result,
            v => { Assert.Equal(4, v.Id); Assert.Equal("Asdf", v.Name); Assert.Null(v.Description); },
            v => { Assert.Equal(5, v.Id); Assert.Equal("Narf", v.Name); Assert.Null(v.Description); },
            v => { Assert.Equal(6, v.Id); Assert.Equal("Qwer", v.Name); Assert.Null(v.Description); });
    }

    [Fact]
    public void ToPath_Substitutes()
    {
        Expression<Func<ParentModel, ParentModelView>> s = d => new ParentModelView { Id = d.Id, Name = d.Name };
        Expression<Func<ChildModel, ChildModelView>> t = d => new ChildModelView { Id = d.Id, Name = d.Name };

        var select = s.Translate().To(d => d.Parent, d => d.Parent, t);
        var result = CreateQuery().OfType<ChildModel>().Select(select);

        Assert.Collection(result,
            v => { Assert.Equal(10, v.Id); Assert.Equal("Asdf", v.Name); Assert.Equal(8, v.Parent.Id); Assert.Equal("Narf", v.Parent.Name); },
            v => { Assert.Equal(11, v.Id); Assert.Equal("Narf", v.Name); Assert.Equal(9, v.Parent.Id); Assert.Equal("Qwer", v.Parent.Name); },
            v => { Assert.Equal(12, v.Id); Assert.Equal("Qwer", v.Name); Assert.Equal(7, v.Parent.Id); Assert.Equal("Asdf", v.Parent.Name); });
    }

    [Fact]
    public void ToPath_WithoutValue_Substitutes()
    {
        Expression<Func<ParentModel, ParentModelView>> s = d => new ParentModelView { Id = d.Id, Name = d.Name };

        var select = s.Translate().To<ChildModel, ChildModelView>(d => d.Parent, d => d.Parent);
        var result = CreateQuery().OfType<ChildModel>().Select(select);

        Assert.Collection(result,
            v => { Assert.Equal(0, v.Id); Assert.Null(v.Name); Assert.Equal(8, v.Parent.Id); Assert.Equal("Narf", v.Parent.Name); },
            v => { Assert.Equal(0, v.Id); Assert.Null(v.Name); Assert.Equal(9, v.Parent.Id); Assert.Equal("Qwer", v.Parent.Name); },
            v => { Assert.Equal(0, v.Id); Assert.Null(v.Name); Assert.Equal(7, v.Parent.Id); Assert.Equal("Asdf", v.Parent.Name); });
    }

    [Fact]
    public void ToTranslation_NullArgument_Throws()
    {
        Expression<Func<ChildModel, ChildModelView>> s = d => new ChildModelView { Id = d.Id, Name = d.Name };

        var error = Assert.Throws<ArgumentNullException>(()
            => s.Translate().To((Expression<Func<ParentModel, Func<ChildModel, ChildModelView>, ParentModelView>>)null!));

        Assert.Equal("translation", error.ParamName);
    }

    [Fact]
    public void ToTranslation_Substitutes()
    {
        Expression<Func<ChildModel, ChildModelView>> s = d => new ChildModelView { Id = d.Id, Name = d.Name };
        Expression<Func<ParentModel, ParentModelView>> t = d => new ParentModelView { Id = d.Id, Name = d.Name };

        var select = s.Translate().To((d, v) => new ParentModelView { FirstChild = d.Children.Select(v).First() }, t);
        var result = CreateQuery().OfType<ParentModel>().Select(select);

        Assert.Collection(result,
            v => { Assert.Equal(7, v.Id); Assert.Equal("Asdf", v.Name); Assert.Equal(10, v.FirstChild.Id); Assert.Equal("Asdf", v.FirstChild.Name); },
            v => { Assert.Equal(8, v.Id); Assert.Equal("Narf", v.Name); Assert.Equal(11, v.FirstChild.Id); Assert.Equal("Narf", v.FirstChild.Name); },
            v => { Assert.Equal(9, v.Id); Assert.Equal("Qwer", v.Name); Assert.Equal(12, v.FirstChild.Id); Assert.Equal("Qwer", v.FirstChild.Name); });
    }

    [Fact]
    public void ToTranslation_WithoutValue_Substitutes()
    {
        Expression<Func<ChildModel, ChildModelView>> s = d => new ChildModelView { Id = d.Id, Name = d.Name };

        var select = s.Translate().To<ParentModel, ParentModelView>((d, v) => new ParentModelView { FirstChild = d.Children.Select(v).First() });
        var result = CreateQuery().OfType<ParentModel>().Select(select);

        Assert.Collection(result,
            v => { Assert.Equal(0, v.Id); Assert.Null(v.Name); Assert.Equal(10, v.FirstChild.Id); Assert.Equal("Asdf", v.FirstChild.Name); },
            v => { Assert.Equal(0, v.Id); Assert.Null(v.Name); Assert.Equal(11, v.FirstChild.Id); Assert.Equal("Narf", v.FirstChild.Name); },
            v => { Assert.Equal(0, v.Id); Assert.Null(v.Name); Assert.Equal(12, v.FirstChild.Id); Assert.Equal("Qwer", v.FirstChild.Name); });
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
            new SpecialModel { Id = 4, Name = "Asdf", Description = "Asdf" },
            new SpecialModel { Id = 5, Name = "Narf", Description = "Narf" },
            new SpecialModel { Id = 6, Name = "Qwer", Description = "Qwer" }
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
        p[2].Children = new[] { c[2], c[0] };

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

    private class ModelView
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public ModelView()
        {
        }

        public ModelView(int id)
        {
            Id = id;
        }
    }

    private class SpecialModelView : ModelView
    {
        public string Description { get; set; } = null!;
    }

    private class ParentModelView
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public ChildModelView FirstChild { get; set; } = null!;

        public IEnumerable<ChildModelView> Children { get; set; } = null!;
    }

    private class ChildModelView
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public ParentModelView Parent { get; set; } = null!;
    }
}
