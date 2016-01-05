using System;
using System.Linq.Expressions;

namespace NeinLinq.Tests.InjectableQueryData
{
    public sealed class ConcreteFunctions : FunctionsBase
    {
        public ConcreteFunctions(int digits)
            : base(digits)
        {
        }

        public Expression<Func<Dummy, double>> VelocityWithConvention()
        {
            return v => Math.Round(v.Distance / v.Time, Digits);
        }

        public Expression<Func<Dummy, double>> VelocityWithMetadata()
        {
            return v => Math.Round(v.Distance / v.Time, Digits);
        }

        [InjectLambda(nameof(Narf))]
        public override double VelocityWithMethodMetadata(Dummy value)
        {
            throw new NotSupportedException();
        }

        public Expression<Func<Dummy, double>> Narf()
        {
            return v => Math.Round(v.Distance / v.Time, Digits);
        }

        public Func<Dummy, double> VelocityWithInvalidSiblingResult()
        {
            return v => Math.Round(v.Distance / v.Time, Digits);
        }

        public Expression<Func<double, double, double>> VelocityWithInvalidSiblingSignature()
        {
            return (d, t) => Math.Round(d / t, Digits);
        }
    }
}
