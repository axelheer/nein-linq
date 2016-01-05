using System;
using System.Linq.Expressions;

namespace NeinLinq.Tests.InjectableQueryData
{
    public interface IParameterizedFunctions
    {
        double VelocityWithoutSibling(Dummy value);

        double VelocityWithConvention(Dummy value);

        [InjectLambda]
        double VelocityWithMetadata(Dummy value);

        [InjectLambda]
        double VelocityWithMethodMetadata(Dummy value);

        double VelocityWithInvalidSiblingResult(Dummy value);

        double VelocityWithInvalidSiblingSignature(Dummy value);
    }

    public abstract class ParameterizedFunctions : IParameterizedFunctions
    {
        readonly int digits;

        public int Digits
        {
            get { return digits; }
        }

        public ParameterizedFunctions(int digits)
        {
            this.digits = digits;
        }

        public double VelocityWithoutSibling(Dummy value)
        {
            throw new NotSupportedException();
        }

        public double VelocityWithConvention(Dummy value)
        {
            throw new NotSupportedException();
        }

        [InjectLambda]
        public double VelocityWithMetadata(Dummy value)
        {
            throw new NotSupportedException();
        }

        [InjectLambda("ignore-me")]
        public virtual double VelocityWithMethodMetadata(Dummy value)
        {
            throw new NotSupportedException();
        }

        public double VelocityWithInvalidSiblingResult(Dummy value)
        {
            throw new NotSupportedException();
        }

        public double VelocityWithInvalidSiblingSignature(Dummy value)
        {
            throw new NotSupportedException();
        }
    }

    public sealed class ParameterizedFunctionsWithExpression : ParameterizedFunctions
    {
        public ParameterizedFunctionsWithExpression(int digits)
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
