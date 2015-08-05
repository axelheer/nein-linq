using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace NeinLinq.Tests.Injectable
{
    public static class Functions
    {
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters")]
        public static double VelocityWithoutSibling(this Dummy value)
        {
            throw new NotSupportedException();
        }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters")]
        public static double VelocityWithConvention(this Dummy value)
        {
            throw new NotSupportedException();
        }

        public static Expression<Func<Dummy, double>> VelocityWithConvention()
        {
            return v => v.Distance / v.Time;
        }

        [InjectLambda]
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters")]
        public static double VelocityWithMetadata(this Dummy value)
        {
            throw new NotSupportedException();
        }

        public static Expression<Func<Dummy, double>> VelocityWithMetadata()
        {
            return v => v.Distance / v.Time;
        }

        [InjectLambda(typeof(Functions), nameof(Narf))]
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters")]
        public static double VelocityWithAdvancedMetadata(this Dummy value)
        {
            throw new NotSupportedException();
        }

        public static Expression<Func<Dummy, double>> Narf()
        {
            return v => v.Distance / v.Time;
        }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters")]
        public static double VelocityWithInvalidSiblingResult(this Dummy value)
        {
            throw new NotSupportedException();
        }

        public static Func<Dummy, double> VelocityWithInvalidSiblingResult()
        {
            return v => v.Distance / v.Time;
        }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters")]
        public static double VelocityWithInvalidSiblingSignature(this Dummy value)
        {
            throw new NotSupportedException();
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

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters")]
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public double Velocity(Dummy value)
        {
            throw new NotSupportedException();
        }

        public Expression<Func<Dummy, double>> Velocity()
        {
            return v => Math.Round(v.Distance / v.Time, digits);
        }
    }
}
