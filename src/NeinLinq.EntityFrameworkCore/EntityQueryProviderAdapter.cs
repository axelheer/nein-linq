using Microsoft.EntityFrameworkCore.Query.Internal;

namespace NeinLinq;

#pragma warning disable EF1001

internal sealed class EntityQueryProviderAdapter : EntityQueryProvider
{
    private readonly RewriteEntityQueryProvider provider;

    public ExpressionVisitor Rewriter => provider.Rewriter;

    public EntityQueryProviderAdapter(RewriteEntityQueryProvider provider)
        : base(null!)
    {
        this.provider = provider;
    }

    public override IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        => provider.CreateQuery<TElement>(expression);

    public override IQueryable CreateQuery(Expression expression)
        => provider.CreateQuery(expression);

    public override TResult Execute<TResult>(Expression expression)
        => provider.Execute<TResult>(expression);

    public override object Execute(Expression expression)
        => provider.Execute(expression)!;

    public override TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
        => provider.ExecuteAsync<TResult>(expression, cancellationToken);
}

#pragma warning restore EF1001
