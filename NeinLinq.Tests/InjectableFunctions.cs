using System;
using System.Linq.Expressions;

namespace NeinLinq.Tests
{
    public static class InjectableFunctions
    {
        public static double VelocityWithoutSibling(this InjectableDummy value)
        {
            throw new NotImplementedException();
        }

        public static double VelocityWithConvention(this InjectableDummy value)
        {
            throw new NotImplementedException();
        }

        public static Expression<Func<InjectableDummy, double>> VelocityWithConvention()
        {
            return v => v.Distance / v.Time;
        }

        [InjectLambda]
        public static double VelocityWithMetadata(this InjectableDummy value)
        {
            throw new NotImplementedException();
        }

        public static Expression<Func<InjectableDummy, double>> VelocityWithMetadata()
        {
            return v => v.Distance / v.Time;
        }

        [InjectLambda(typeof(InjectableFunctions), "Narf")]
        public static double VelocityWithAdvancedMetadata(this InjectableDummy value)
        {
            throw new NotImplementedException();
        }

        public static Expression<Func<InjectableDummy, double>> Narf()
        {
            return v => v.Distance / v.Time;
        }

        public static double VelocityWithInvalidSiblingResult(this InjectableDummy value)
        {
            throw new NotImplementedException();
        }

        public static Func<InjectableDummy, double> VelocityWithInvalidSiblingResult()
        {
            return v => v.Distance / v.Time;
        }

        public static double VelocityWithInvalidSiblingSignature(this InjectableDummy value)
        {
            throw new NotImplementedException();
        }

        public static Expression<Func<double, double, double>> VelocityWithInvalidSiblingSignature()
        {
            return (d, t) => d / t;
        }
    }
}
