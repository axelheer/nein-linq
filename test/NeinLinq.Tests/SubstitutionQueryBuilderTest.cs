using NeinLinq.Tests.SubstitutionQueryData;
using System;
using System.Linq;
using Xunit;

namespace NeinLinq.Tests
{
    public class SubstitutionQueryBuilderTest
    {
        readonly IOrderedQueryable<Dummy> query = Enumerable.Empty<Dummy>().AsQueryable().OrderBy(d => d.Id);

        [Fact]
        public void ToSubstitutionShouldHandleInvalidArguments()
        {
            Assert.Throws<ArgumentNullException>(() => ((IQueryable)query).ToSubstitution(null, typeof(OtherFunctions)));
            Assert.Throws<ArgumentNullException>(() => ((IQueryable)query).ToSubstitution(typeof(Functions), null));
            Assert.Throws<ArgumentNullException>(() => ((IQueryable<Dummy>)query).ToSubstitution(null, typeof(OtherFunctions)));
            Assert.Throws<ArgumentNullException>(() => ((IQueryable<Dummy>)query).ToSubstitution(typeof(Functions), null));
            Assert.Throws<ArgumentNullException>(() => ((IOrderedQueryable)query).ToSubstitution(null, typeof(OtherFunctions)));
            Assert.Throws<ArgumentNullException>(() => ((IOrderedQueryable)query).ToSubstitution(typeof(Functions), null));
            Assert.Throws<ArgumentNullException>(() => ((IOrderedQueryable<Dummy>)query).ToSubstitution(null, typeof(OtherFunctions)));
            Assert.Throws<ArgumentNullException>(() => ((IOrderedQueryable<Dummy>)query).ToSubstitution(typeof(Functions), null));
        }

        [Fact]
        public void ToSubstitutionShouldRewriteUntypedQueryable()
        {
            var actual = ((IQueryable)query).ToSubstitution(typeof(Functions), typeof(OtherFunctions));

            Assert.IsType<RewriteQuery<Dummy>>(actual);
        }

        [Fact]
        public void ToSubstitutionShouldRewriteTypedQueryable()
        {
            var actual = ((IQueryable<Dummy>)query).ToSubstitution(typeof(Functions), typeof(OtherFunctions));

            Assert.IsType<RewriteQuery<Dummy>>(actual);
        }

        [Fact]
        public void ToSubstitutionShouldRewriteUntypedOrderedQueryable()
        {
            var actual = ((IOrderedQueryable)query).ToSubstitution(typeof(Functions), typeof(OtherFunctions));

            Assert.IsType<RewriteQuery<Dummy>>(actual);
        }

        [Fact]
        public void ToSubstitutionShouldRewriteTypedOrderedQueryable()
        {
            var actual = ((IOrderedQueryable<Dummy>)query).ToSubstitution(typeof(Functions), typeof(OtherFunctions));

            Assert.IsType<RewriteQuery<Dummy>>(actual);
        }
    }
}
