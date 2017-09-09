using System;
using System.Linq.Expressions;

namespace NeinLinq.Fakes.InjectableQuery
{
    public sealed class ConcreteFunctions : FunctionsBase
    {
        readonly int digits;

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

        public Func<Dummy, double> VelocityWithInvalidSiblingResult()
        {
            return v => Math.Round(v.Distance / v.Time, digits);
        }

        public Expression<Func<double, double, double>> VelocityWithInvalidSiblingSignature()
        {
            return (d, t) => Math.Round(d / t, digits);
        }

        public Expression<Func<TDummy, double>> VelocityWithGenericArguments<TDummy>()
            where TDummy : IDummy
        {
            return v => v.Distance / v.Time;
        }

        public Expression<Func<Dummy, double>> VelocityWithInvalidGenericArguments()
        {
            return v => v.Distance / v.Time;
        }
    }
}
