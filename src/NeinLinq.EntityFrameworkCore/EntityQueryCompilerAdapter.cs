using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace NeinLinq;

#pragma warning disable CA1812, EF1001

internal sealed class EntityQueryCompilerAdapter<TInnerCompiler> : IQueryCompiler
    where TInnerCompiler : IQueryCompiler
{
    private readonly EntityQueryCompilerAdapterOptions options;
    private readonly TInnerCompiler innerCompiler;

    public EntityQueryCompilerAdapter(EntityQueryCompilerAdapterOptions options, TInnerCompiler innerCompiler)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
        this.innerCompiler = innerCompiler ?? throw new ArgumentNullException(nameof(innerCompiler));
    }

    public TResult Execute<TResult>(Expression query)
        => innerCompiler.Execute<TResult>(RewriteQuery(query));

    public TResult ExecuteAsync<TResult>(Expression query, CancellationToken cancellationToken)
        => innerCompiler.ExecuteAsync<TResult>(RewriteQuery(query), cancellationToken);

    public Func<QueryContext, TResult> CreateCompiledQuery<TResult>(Expression query)
        => innerCompiler.CreateCompiledQuery<TResult>(RewriteQuery(query));

    public Func<QueryContext, TResult> CreateCompiledAsyncQuery<TResult>(Expression query)
        => innerCompiler.CreateCompiledQuery<TResult>(RewriteQuery(query));

    private readonly ExpressionVisitor cleaner
        = new RewriteQueryCleaner();

    private Expression RewriteQuery(Expression query)
        => options.Rewriters.Prepend(cleaner).Aggregate(query, (q, r) => r.Visit(q));
}

#pragma warning restore CA1812, EF1001
