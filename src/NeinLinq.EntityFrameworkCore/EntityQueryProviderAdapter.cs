using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace NeinLinq
{
    [ExcludeFromCodeCoverage]
    internal class EntityQueryProviderAdapter : EntityQueryProvider
    {
        private readonly RewriteEntityQueryProvider provider;

        public EntityQueryProviderAdapter(RewriteEntityQueryProvider provider)
            : base(new EmptyQueryCompiler())
        {
            this.provider = provider;
        }

        public override IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return provider.CreateQuery<TElement>(expression);
        }

        public override IQueryable CreateQuery(Expression expression)
        {
            return provider.CreateQuery(expression);
        }

        public override TResult Execute<TResult>(Expression expression)
        {
            return provider.Execute<TResult>(expression);
        }

        public override object Execute(Expression expression)
        {
            return provider.Execute(expression);
        }

        public override IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
        {
            return provider.ExecuteAsync<TResult>(expression);
        }

        public override Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            return provider.ExecuteAsync<TResult>(expression, cancellationToken);
        }

        private class EmptyQueryCompiler : IQueryCompiler
        {
            public Func<QueryContext, IAsyncEnumerable<TResult>> CreateCompiledAsyncEnumerableQuery<TResult>(Expression query)
            {
                throw new NotSupportedException();
            }

            public Func<QueryContext, Task<TResult>> CreateCompiledAsyncTaskQuery<TResult>(Expression query)
            {
                throw new NotSupportedException();
            }

            public Func<QueryContext, TResult> CreateCompiledQuery<TResult>(Expression query)
            {
                throw new NotSupportedException();
            }

            public TResult Execute<TResult>(Expression query)
            {
                throw new NotSupportedException();
            }

            public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression query)
            {
                throw new NotSupportedException();
            }

            public Task<TResult> ExecuteAsync<TResult>(Expression query, CancellationToken cancellationToken)
            {
                throw new NotSupportedException();
            }
        }
    }
}
