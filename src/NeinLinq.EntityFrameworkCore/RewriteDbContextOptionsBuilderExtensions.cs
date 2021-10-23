using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace NeinLinq;

/// <summary>
/// Replaces method calls with lambda expressions.
/// </summary>
[CLSCompliant(false)]
public static class RewriteDbContextOptionsBuilderExtensions
{
    /// <summary>
    /// Replaces method calls with lambda expressions.
    /// </summary>
    /// <param name="optionsBuilder">The builder being used to configure the context.</param>
    /// <param name="greenlist">A list of types to inject, whether marked as injectable or not.</param>
    /// <returns>The options builder so that further configuration can be chained.</returns>
    public static DbContextOptionsBuilder WithLambdaInjection(this DbContextOptionsBuilder optionsBuilder, params Type[] greenlist)
        => WithRewriter(optionsBuilder, new InjectableQueryRewriter(greenlist));

    /// <summary>
    /// Rewrite all the queries.
    /// </summary>
    /// <param name="optionsBuilder">The builder being used to configure the context.</param>
    /// <param name="rewriter">The rewriter to rewrite every query.</param>
    /// <returns>The options builder so that further configuration can be chained.</returns>
    public static DbContextOptionsBuilder WithRewriter(this DbContextOptionsBuilder optionsBuilder, ExpressionVisitor rewriter)
    {
        if (optionsBuilder is null)
            throw new ArgumentNullException(nameof(optionsBuilder));
        if (rewriter is null)
            throw new ArgumentNullException(nameof(rewriter));

        var extension = GetOrCreateExtension(optionsBuilder).WithRewriter(rewriter);
        ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

        return optionsBuilder;
    }

    private static RewriteDbContextOptionsExtension GetOrCreateExtension(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.Options.FindExtension<RewriteDbContextOptionsExtension>()
            ?? new RewriteDbContextOptionsExtension();
}
