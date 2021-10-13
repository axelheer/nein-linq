using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;

#pragma warning disable CA1812, EF1001

namespace NeinLinq
{
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

#pragma warning disable RCS1047

        public TResult ExecuteAsync<TResult>(Expression query, CancellationToken cancellationToken)
            => innerCompiler.ExecuteAsync<TResult>(RewriteQuery(query), cancellationToken);

#pragma warning restore RCS1047

        public Func<QueryContext, TResult> CreateCompiledQuery<TResult>(Expression query)
            => innerCompiler.CreateCompiledQuery<TResult>(RewriteQuery(query));

        public Func<QueryContext, TResult> CreateCompiledAsyncQuery<TResult>(Expression query)
            => innerCompiler.CreateCompiledQuery<TResult>(RewriteQuery(query));

        private readonly ExpressionVisitor cleaner
            = new RewriteQueryCleaner();

        private Expression RewriteQuery(Expression query)
            => options.Rewriters.Prepend(cleaner).Aggregate(query, (q, r) => r.Visit(q));
    }
}
