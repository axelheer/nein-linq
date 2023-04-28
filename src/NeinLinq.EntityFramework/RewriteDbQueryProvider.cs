using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace NeinLinq;

/// <summary>
/// Proxy for query provider.
/// </summary>
public class RewriteDbQueryProvider : RewriteQueryProvider, IDbAsyncQueryProvider
{
    private static readonly Type DbQueryVisitorType = typeof(DbContext).Assembly.GetType("System.Data.Entity.Internal.Linq.DbQueryVisitor")
        ?? throw new InvalidOperationException("Unable to resolve 'DbQueryVisitor'.");

    /// <summary>
    /// Create a new rewrite query provider.
    /// </summary>
    /// <param name="provider">The actual query provider.</param>
    /// <param name="rewriter">The rewriter to rewrite the query.</param>
    public RewriteDbQueryProvider(IQueryProvider provider, ExpressionVisitor rewriter)
        : base(provider, rewriter)
    {
    }

    /// <inheritdoc />
    protected override Expression Rewrite(Expression expression)
    {
        var rewritten = base.Rewrite(expression);
        return Activator.CreateInstance(DbQueryVisitorType) is ExpressionVisitor dbQueryVisitor
            ? dbQueryVisitor.Visit(rewritten)
            : rewritten;
    }

    /// <inheritdoc />
    public override IQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
        // create query and make proxy again for rewritten query chaining
        var query = Provider.CreateQuery<TElement>(expression);
        return new RewriteDbQueryable<TElement>(query, this);
    }

    /// <inheritdoc />
    public override IQueryable CreateQuery(Expression expression)
    {
        // create query and make proxy again for rewritten query chaining
        var query = Provider.CreateQuery(expression);
        return (IQueryable)Activator.CreateInstance(
            typeof(RewriteDbQueryable<>).MakeGenericType(query.ElementType),
            query, this)!;
    }

    /// <inheritdoc />
    public virtual Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
    {
        // execute query with rewritten expression; async, if possible
        var rewritten = Rewrite(expression);
        return Provider is IDbAsyncQueryProvider asyncProvider
            ? asyncProvider.ExecuteAsync<TResult>(rewritten, cancellationToken)
            : Task.FromResult(Provider.Execute<TResult>(rewritten));
    }

    /// <inheritdoc />
    public virtual Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken)
    {
        // execute query with rewritten expression; async, if possible
        var rewritten = Rewrite(expression);
        return Provider is IDbAsyncQueryProvider asyncProvider
            ? asyncProvider.ExecuteAsync(rewritten, cancellationToken)
            : Task.FromResult(Provider.Execute(rewritten)!);
    }
}
