using System;
using System.Linq.Expressions;

namespace NeinLinq.Tests.InjectableQueryData
{
    public static class Functions
    {
        public static double VelocityWithoutSibling(this Dummy value)
        {
            throw new NotSupportedException();
        }

        public static double VelocityWithConvention(this Dummy value)
        {
            throw new NotSupportedException();
        }

        public static Expression<Func<Dummy, double>> VelocityWithConvention()
        {
            return v => v.Distance / v.Time;
        }

        [InjectLambda]
        public static double VelocityWithMetadata(this Dummy value)
        {
            throw new NotSupportedException();
        }

        public static Expression<Func<Dummy, double>> VelocityWithMetadata()
        {
            return v => v.Distance / v.Time;
        }

        [InjectLambda(typeof(OtherFunctions), nameof(OtherFunctions.Narf))]
        public static double VelocityWithTypeAndMethodMetadata(this Dummy value)
        {
            throw new NotSupportedException();
        }

        [InjectLambda(typeof(OtherFunctions))]
        public static double VelocityWithTypeMetadata(this Dummy value)
        {
            throw new NotSupportedException();
        }

        [InjectLambda(nameof(Narf))]
        public static double VelocityWithMethodMetadata(this Dummy value)
        {
            throw new NotSupportedException();
        }

        public static Expression<Func<Dummy, double>> Narf()
        {
            return v => v.Distance / v.Time;
        }

        public static double VelocityWithInvalidSiblingResult(this Dummy value)
        {
            throw new NotSupportedException();
        }

        public static Func<Dummy, double> VelocityWithInvalidSiblingResult()
        {
            return v => v.Distance / v.Time;
        }

        public static double VelocityWithInvalidSiblingSignature(this Dummy value)
        {
            throw new NotSupportedException();
        }

        public static Expression<Func<double, double, double>> VelocityWithInvalidSiblingSignature()
        {
            return (d, t) => d / t;
        }
    }

    public static class OtherFunctions
    {
        public static Expression<Func<Dummy, double>> Narf()
        {
            return v => v.Distance / v.Time;
        }

        public static Expression<Func<Dummy, double>> VelocityWithTypeMetadata()
        {
            return v => v.Distance / v.Time;
        }
    }
}
