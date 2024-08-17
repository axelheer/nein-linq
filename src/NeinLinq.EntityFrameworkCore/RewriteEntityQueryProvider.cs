
using System.Reflection;
using Microsoft.EntityFrameworkCore.Query;

namespace NeinLinq;

/// <summary>
/// Proxy for query provider.
/// </summary>
public class RewriteEntityQueryProvider : RewriteQueryProvider, IAsyncQueryProvider
{
    /// <summary>
    /// Create a new rewrite query provider.
    /// </summary>
    /// <param name="provider">The actual query provider.</param>
    /// <param name="rewriter">The rewriter to rewrite the query.</param>
    public RewriteEntityQueryProvider(IQueryProvider provider, ExpressionVisitor rewriter)
        : base(provider, rewriter)
    {
    }

    /// <inheritdoc />
    public override IQueryable<TElement> CreateQuery<TElement>(Expression expression)
    {
        // create query and make proxy again for rewritten query chaining
        var query = Provider.CreateQuery<TElement>(expression);
        return new RewriteEntityQueryable<TElement>(query, this);
    }

    /// <inheritdoc />
    public override IQueryable CreateQuery(Expression expression)
    {
        // create query and make proxy again for rewritten query chaining
        var query = Provider.CreateQuery(expression);
        return (IQueryable)Activator.CreateInstance(
            typeof(RewriteEntityQueryable<>).MakeGenericType(query.ElementType),
            query, this)!;
    }

    /// <inheritdoc />
    public virtual TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
    {
        // execute query with rewritten expression; async, if possible
        var rewritten = Rewrite(expression);
        return Provider is IAsyncQueryProvider asyncProvider
            ? asyncProvider.ExecuteAsync<TResult>(rewritten, cancellationToken)
            : typeof(Task).IsAssignableFrom(typeof(TResult))
            ? Execute<TResult>(ExecuteTaskMethod, rewritten)
            : Provider.Execute<TResult>(rewritten);
    }

    private TResult Execute<TResult>(MethodInfo method, Expression expression)
        => (TResult)(method.MakeGenericMethod(typeof(TResult).GetGenericArguments()[0])
            .Invoke(this, new object[] { expression })!);

    private static readonly MethodInfo ExecuteTaskMethod
        = typeof(RewriteEntityQueryProvider).GetMethod(nameof(ExecuteTask), BindingFlags.Instance | BindingFlags.NonPublic)!;

    private Task<TResult> ExecuteTask<TResult>(Expression expression)
        => Task.FromResult(Provider.Execute<TResult>(expression));
}
