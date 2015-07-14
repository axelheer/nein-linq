using System;
using System.Linq.Expressions;

namespace NeinLinq.Tests.Injectable
{
    public static class Functions
    {
        public static double VelocityWithoutSibling(this Dummy value)
        {
            throw new NotImplementedException();
        }

        public static double VelocityWithConvention(this Dummy value)
        {
            throw new NotImplementedException();
        }

        public static Expression<Func<Dummy, double>> VelocityWithConvention()
        {
            return v => v.Distance / v.Time;
        }

        [InjectLambda]
        public static double VelocityWithMetadata(this Dummy value)
        {
            throw new NotImplementedException();
        }

        public static Expression<Func<Dummy, double>> VelocityWithMetadata()
        {
            return v => v.Distance / v.Time;
        }

        [InjectLambda(typeof(Functions), "Narf")]
        public static double VelocityWithAdvancedMetadata(this Dummy value)
        {
            throw new NotImplementedException();
        }

        public static Expression<Func<Dummy, double>> Narf()
        {
            return v => v.Distance / v.Time;
        }

        public static double VelocityWithInvalidSiblingResult(this Dummy value)
        {
            throw new NotImplementedException();
        }

        public static Func<Dummy, double> VelocityWithInvalidSiblingResult()
        {
            return v => v.Distance / v.Time;
        }

        public static double VelocityWithInvalidSiblingSignature(this Dummy value)
        {
            throw new NotImplementedException();
        }

        public static Expression<Func<double, double, double>> VelocityWithInvalidSiblingSignature()
        {
            return (d, t) => d / t;
        }
    }

    public class ParameterizedFunctions
    {
        private readonly int digits;

        public ParameterizedFunctions(int digits)
        {
            this.digits = digits;
        }

        public double Velocity(Dummy value)
        {
            throw new NotImplementedException();
        }

        public Expression<Func<Dummy, double>> Velocity()
        {
            return v => Math.Round(v.Distance / v.Time, digits);
        }
    }
}
