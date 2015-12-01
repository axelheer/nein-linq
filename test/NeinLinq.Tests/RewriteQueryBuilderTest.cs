using NeinLinq.Tests.RewriteQueryData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NeinLinq.Tests
{
    public class RewriteQueryBuilderTest
    {
        readonly IOrderedQueryable<Dummy> query = Enumerable.Empty<Dummy>().AsQueryable().OrderBy(d => d.Id);

        [Fact]
        public void RewriteShouldHandleInvalidArguments()
        {
            Assert.Throws<ArgumentNullException>(() => default(IQueryable).Rewrite(new Rewriter()));
            Assert.Throws<ArgumentNullException>(() => ((IQueryable)query).Rewrite(null));
            Assert.Throws<ArgumentNullException>(() => default(IQueryable<Dummy>).Rewrite(new Rewriter()));
            Assert.Throws<ArgumentNullException>(() => ((IQueryable<Dummy>)query).Rewrite(null));
            Assert.Throws<ArgumentNullException>(() => default(IOrderedQueryable).Rewrite(new Rewriter()));
            Assert.Throws<ArgumentNullException>(() => ((IOrderedQueryable)query).Rewrite(null));
            Assert.Throws<ArgumentNullException>(() => default(IOrderedQueryable<Dummy>).Rewrite(new Rewriter()));
            Assert.Throws<ArgumentNullException>(() => ((IOrderedQueryable<Dummy>)query).Rewrite(null));
        }

        [Fact]
        public void RewriteShouldRewriteUntypedQueryable()
        {
            var actual = ((IQueryable)query).Rewrite(new Rewriter());

            Assert.IsType<RewriteQuery<Dummy>>(actual);
        }

        [Fact]
        public void RewriteShouldRewriteTypedQueryable()
        {
            var actual = ((IQueryable<Dummy>)query).Rewrite(new Rewriter());

            Assert.IsType<RewriteQuery<Dummy>>(actual);
        }

        [Fact]
        public void RewriteShouldRewriteUntypedOrderedQueryable()
        {
            var actual = ((IOrderedQueryable)query).Rewrite(new Rewriter());

            Assert.IsType<RewriteQuery<Dummy>>(actual);
        }

        [Fact]
        public void RewriteShouldRewriteTypedOrderedQueryable()
        {
            var actual = ((IOrderedQueryable<Dummy>)query).Rewrite(new Rewriter());

            Assert.IsType<RewriteQuery<Dummy>>(actual);
        }
    }
}
