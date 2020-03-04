using System;
using System.Collections.Generic;
using System.Linq.Expressions;

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
        {
            return v => Math.Round(v.Distance / v.Time, digits);
        }

        public Expression<Func<Dummy, double>> VelocityWithMetadata()
        {
            return v => Math.Round(v.Distance / v.Time, digits);
        }

        [InjectLambda(nameof(Narf))]
        public new double VelocityWithMethodMetadata(Dummy value)
        {
            throw new NotSupportedException();
        }

        public Expression<Func<Dummy, double>> Narf()
        {
            return v => Math.Round(v.Distance / v.Time, digits);
        }

        public IEnumerable<Func<Dummy, double>> VelocityWithStupidSiblingResult()
        {
            yield return v => Math.Round(v.Distance / v.Time, digits);
        }

        public Func<Dummy, double> VelocityWithInvalidSiblingResult()
        {
            return v => Math.Round(v.Distance / v.Time, digits);
        }

        public Expression<Func<Dummy, float>> VelocityWithStupidSiblingSignature()
        {
            return v => (float)Math.Round(v.Distance / v.Time, digits);
        }

        public Expression<Func<double, double, double>> VelocityWithInvalidSiblingSignature()
        {
            return (d, t) => Math.Round(d / t, digits);
        }

        public Expression<Func<TDummy, TOther, double>> VelocityWithGenericArguments<TDummy, TOther>()
            where TDummy : IDummy
        {
            return (v, _) => Math.Round(v.Distance / v.Time, digits);
        }

        public Expression<Func<TDummy, double>> VelocityWithGenericArguments<TDummy>()
            where TDummy : IDummy
        {
            return v => Math.Round(v.Distance / v.Time, digits);
        }

        public Expression<Func<Dummy, double>> VelocityWithInvalidGenericArguments()
        {
            return v => Math.Round(v.Distance / v.Time, digits);
        }

        private Expression<Func<Dummy, double>> VelocityWithNonPublicSibling()
        {
            return v => Math.Round(v.Distance / v.Time, digits);
        }

        public new Expression<Func<Dummy, double>> VelocityWithHiddenSibling()
        {
            throw new InvalidOperationException("Implementing sibling has been hidden.");
        }

        public override Expression<Func<Dummy, double>> VelocityWithAbstractSibling()
        {
            return v => Math.Round(v.Distance / v.Time, digits);
        }

        public override Expression<Func<Dummy, double>> VelocityWithVirtualSibling()
        {
            return v => Math.Round(v.Distance / v.Time, digits);
        }
    }
}
