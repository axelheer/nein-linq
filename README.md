NeinLinq
========

*NeinLinq* provides helpful extensions for using LINQ providers supporting only a minor subset of .NET functions (like Entity Framework), rewriting LINQ queries (even making them null-safe), and building dynamic queries using (translatable) predicates.

To install *NeinLinq*, run the following command in the [NuGet Package Manager Console](http://docs.nuget.org/docs/start-here/using-the-package-manager-console).

    PM> Install-Package NeinLinq 

Lambda injection
----------------

Many LINQ providers can only support a very minor subset of .NET functionality, they cannot even support our own "functions". Say, we implement a simple method `LimitText` and use it within an ordinary LINQ query, which will get translated to SQL through Entity Framework...

> *LINQ to Entities does not recognize the method 'System.String LimitText(System.String, Int32)' method, and this method cannot be translated into a store expression.*

This is what we get; in fact, it's really annoying. We have to scatter our logic between code, that will be translated by any LINQ query provider, and code, that won't. It gets even worse: if some logic is "translatable", which is good, we have to copy and paste! Consolidating the code within an ordinary function does not work since the provider is unable to translate this simple method call. Meh.

Let us introduce "lambda injection":

    [InjectLambda]
    public static string LimitText(this string value, int maxLength)
    {
        if (value != null && value.Length > maxLength - 3)
            return value.Substring(0, maxLength - 3) + "...";
        return value;
    }

    public static Expression<Func<string, int, string>> LimitText()
    {
        return (v, l) => v != null && v.Length > l - 3 ? v.Substring(0, l - 3) + "..." : v;
    }

If a query is marked as "injectable" and a function within this query is marked as "inject here", the rewrite engine of *NeinLinq* replaces the method call with the matching lambda expression, which can get translate to SQL or whatever. Thus, we are able to encapsulate unsupported .NET functionality and even create our own. Bazinga!

Finally, let us look at a query using *Entity Framework* or the like:


    from d in data.ToInjectable()
    where d.FulfillsSomeCriteria()
    select new
    {
        Id = d.Id,
        Value = d.DoSomethingFancy()
    }

The methods `FulfillsSomeCriteria` and `DoSomethingFancy` should be marked accordingly, using an attribute or just the simple convention "same class, same name, matching signature" (which requires the class to be whitelisted by the way). And the call `ToInjectable` can happen anywhere within the LINQ query chain, so we don't have to pollute our business logic...


Null-safe queries
-----------------

We are writing the year 2014 and still have to worry about null values.

Howsoever, we got used to it and we are fine. But writing queries in C# loaded with null checks doesn't feel right, it just looks awful, the tranlated SQL even gets worse. A LINQ query just for SQL dbs can spare these null checks, a LINQ query just for in-memory calculcations must include them. And a LINQ query for both has a problem, which *NeinLinq* tries to solve.

The following query may trigger null references:

    from a in data
    orderby a.SomeInteger
    select new
    {
        Year = a.SomeDate.Year,
        Integer = a.SomeOther.SomeInteger,
        Others = from b in a.SomeOthers
                 select b.SomeDate.Month,
        More = from c in a.MoreOthers
               select c.SomeOther.SomeDate.Day
    }

While the following query *should* not:

    from a in data
    orderby a.SomeInteger
    select new
    {
        Year = a.SomeDate.Year,
        Integer = a.SomeOther != null
                ? a.SomeOther.SomeInteger
                : 0,
        Others = a.SomeOthers != null
               ? from b in a.SomeOthers
                 select b.SomeDate.Month
               : null,
        More = a.MoreOthers != null
             ? from c in a.MoreOthers
               select c.SomeOther != null
                    ? c.SomeOther.SomeDate.Day
                    : 0
             : null
    }

Maybe we've forgot some check? Or we can relax thanks to *NeinLinq*:

    from a in data.ToNullsafe()
    orderby a.SomeInteger
    select new
    {
        Year = a.SomeDate.Year,
        Integer = a.SomeOther.SomeInteger,
        Others = from b in a.SomeOthers
                 select b.SomeDate.Month,
        More = from c in a.MoreOthers
               select c.SomeOther.SomeDate.Day
    }

As with every `ToWhatever` helper within *NeinLinq*, `ToNullsafe` can be called wherever within the LINQ query chain.

Predicate translator
--------------------

Many data driven applications need to build some kind of dynamic queries. This can lead to dirty string manipulations, complex expression tree plumbing, or a combination of those. Simple *and*/*or*-conjunctions are already solved within other libraries, but conjunctions of "foreign" predicates are not that easy.

Let us think of three entities: Academy has Courses, Courses has Lectures.

    Expression<Func<Course, bool>> p = ...
    Expression<Func<Course, bool>> q = ...

    db.Courses.Where(p.And(q))...

Ok, we already know that.

    Expression<Func<Academy, bool>> p = ...
    Expression<Func<Academy, bool>> q = ...

    db.Courses.Where(p.And(q).Translate().To<Course>(c => c.Academy))...

We now can translate a (combined) predicate for a parent entity...

    Expression<Func<Lecture, bool>> p = ...
    Expression<Func<Lecture, bool>> q = ...

    db.Courses.Where(p.And(q).Translate().To<Course>((c, r) => c.Lectures.Any(l => r(l))))...

..and even for child entities. Awesome!

Let us use all of this as a windup:

    IEnumerable<Expression<Func<Academy, bool>>> predicatesForAcademy = ...
    IEnumerable<Expression<Func<Course, bool>>> predicatesForCourse = ...
    IEnumerable<Expression<Func<Lecture, bool>>> predicatesForLecture = ...

    var singlePredicateForAcademy = predicatesForAcademy.Aggregate((p, q) => p.And(q));
    var singlePredicateForCourse = predicatesForCourse.Aggregate((p, q) => p.And(q));
    var singlePredicateForLecture = predicatesForLecture.Aggregate((p, q) => p.And(q));

    var academyPredicateForCourse = singlePredicateForAcademy.Translate().To<Course>(c => c.Academy);
    var coursePredicateForCourse = singlePredicateForCourse;
    var lecturePredicateForCourse = singlePredicateForLecture.Translate().To<Course>((c, p) => c.Lectures.Any(l => p(l)));

    var finalPredicate = academyPredicateForCourse.And(coursePredicateForCourse).And(lecturePredicateForCourse);

    db.Courses.Where(finalPredicate)...

Function substitution
---------------------

This is a really dead simple one. Maybe i should've started here...

Just think of helper functions like the `SqlFunctions` class provided by *Entity Framework*. And we need to replace the whole class for unit testing or whatsoever.

    var query = ...

    CallCodeUsingSqlFunctions(query.ToSubstitution(typeof(SqlFunctions), typeof(SqlCeFunctions)));
    CallCodeUsingSqlFunctions(query.ToSubstitution(typeof(SqlFunctions), typeof(FakeFunctions)));
    ...

That's it.
