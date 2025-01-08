using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;

namespace NeinLinq;

#pragma warning disable EF1001

internal sealed class EntityQueryCompilerAdapter : QueryCompiler
{
    private readonly EntityQueryCompilerAdapterOptions options;

    public EntityQueryCompilerAdapter(EntityQueryCompilerAdapterOptions options,
        IQueryContextFactory queryContextFactory,
        ICompiledQueryCache compiledQueryCache,
        ICompiledQueryCacheKeyGenerator compiledQueryCacheKeyGenerator,
        IDatabase database,
        IDiagnosticsLogger<DbLoggerCategory.Query> logger,
        ICurrentDbContext currentContext,
        IEvaluatableExpressionFilter evaluatableExpressionFilter,
        IModel model)
        : base(queryContextFactory, compiledQueryCache, compiledQueryCacheKeyGenerator, database, logger, currentContext, evaluatableExpressionFilter, model)
    {
        this.options = options ?? throw new ArgumentNullException(nameof(options));
    }

    private readonly ExpressionVisitor cleaner
        = new RewriteQueryCleaner();

    // Sadly, the "extraction of parameters" isn't part of any interface; thus, we have to inherit the whole internal QueryCompiler in order to make this happen!
    // Implementing IQueryExpressionVisitorInterceptor doesn't help for more complex scenarios, because inception happens after extraction, whatever that means...
    public override Expression ExtractParameters(Expression query, IParameterValues parameterValues, IDiagnosticsLogger<DbLoggerCategory.Query> logger, bool compiledQuery = false, bool generateContextAccessors = false)
        => base.ExtractParameters(options.Rewriters.Prepend(cleaner).Aggregate(query, (q, r) => r.Visit(q)), parameterValues, logger, compiledQuery, generateContextAccessors);
}

#pragma warning restore EF1001
