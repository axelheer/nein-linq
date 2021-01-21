using System;
using System.Collections.Generic;
using System.Linq.Expressions;

#pragma warning disable CA1801
#pragma warning disable CA1822

namespace NeinLinq.Fakes.InjectableQuery
{
    public sealed class ConcreteFunctions : FunctionsBase
    {
        private readonly int digits;

        public ConcreteFunctions(int digits)
        {
            this.digits = digits;
        }

        public Expression<Func<Dummy, double>> VelocityWithConvention()
            => v => Math.Round(v.Distance / v.Time, digits);

        public Expression<Func<Dummy, double>> VelocityWithMetadata()
            => v => Math.Round(v.Distance / v.Time, digits);

        [InjectLambda(nameof(Narf))]
        public new double VelocityWithMethodMetadata(Dummy value)
            => throw new NotSupportedException();

        public Expression<Func<Dummy, double>> Narf()
            => v => Math.Round(v.Distance / v.Time, digits);

        public IEnumerable<Func<Dummy, double>> VelocityWithStupidSiblingResult()
            => new Func<Dummy, double>[] { v => Math.Round(v.Distance / v.Time, digits) };

        public Func<Dummy, double> VelocityWithInvalidSiblingResult()
            => v => Math.Round(v.Distance / v.Time, digits);

        public Expression<Func<Dummy, float>> VelocityWithStupidSiblingSignature()
            => v => (float)Math.Round(v.Distance / v.Time, digits);

        public Expression<Func<double, double, double>> VelocityWithInvalidSiblingSignature()
            => (d, t) => Math.Round(d / t, digits);

        public Expression<Func<TDummy, TOther, double>> VelocityWithGenericArguments<TDummy, TOther>()
            where TDummy : IDummy => (v, _)
            => Math.Round(v.Distance / v.Time, digits);

        public Expression<Func<TDummy, double>> VelocityWithGenericArguments<TDummy>()
            where TDummy : IDummy
            => v => Math.Round(v.Distance / v.Time, digits);

        public Expression<Func<Dummy, double>> VelocityWithInvalidGenericArguments()
            => v => Math.Round(v.Distance / v.Time, digits);

        private Expression<Func<Dummy, double>> VelocityWithNonPublicSibling()
            => v => Math.Round(v.Distance / v.Time, digits);

        public new Expression<Func<Dummy, double>> VelocityWithHiddenSibling()
            => throw new InvalidOperationException("Implementing sibling has been hidden.");

        public override Expression<Func<Dummy, double>> VelocityWithAbstractSibling()
            => v => Math.Round(v.Distance / v.Time, digits);

        public override Expression<Func<Dummy, double>> VelocityWithVirtualSibling()
            => v => Math.Round(v.Distance / v.Time, digits);

        private CachedExpression<Func<Dummy, double>> VelocityWithCachedExpressionExpr { get; }
            = CachedExpression.From<Func<Dummy, double>>(v => v.Distance / v.Time);
    }
}
