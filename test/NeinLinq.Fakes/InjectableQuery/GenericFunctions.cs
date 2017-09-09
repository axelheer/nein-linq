using System;
using System.Linq.Expressions;

namespace NeinLinq.Fakes.InjectableQuery
{
    public static class GenericFunctions
    {
        public static double VelocityWithoutSibling<TDummy>(this TDummy value)
            where TDummy : IDummy
        {
            throw new NotSupportedException();
        }

        public static double VelocityWithConvention<TDummy>(this TDummy value)
            where TDummy : IDummy
        {
            throw new NotSupportedException();
        }

        public static Expression<Func<TDummy, double>> VelocityWithConvention<TDummy>()
            where TDummy : IDummy
        {
            return v => v.Distance / v.Time;
        }

        [InjectLambda]
        public static double VelocityWithMetadata<TDummy>(this TDummy value)
            where TDummy : IDummy
        {
            throw new NotSupportedException();
        }

        public static Expression<Func<TDummy, double>> VelocityWithMetadata<TDummy>()
            where TDummy : IDummy
        {
            return v => v.Distance / v.Time;
        }

        [InjectLambda(typeof(GenericOtherFunctions), nameof(GenericOtherFunctions.Narf))]
        public static double VelocityWithTypeAndMethodMetadata<TDummy>(this TDummy value)
            where TDummy : IDummy
        {
            throw new NotSupportedException();
        }

        [InjectLambda(typeof(GenericOtherFunctions))]
        public static double VelocityWithTypeMetadata<TDummy>(this TDummy value)
            where TDummy : IDummy
        {
            throw new NotSupportedException();
        }

        [InjectLambda(nameof(Narf))]
        public static double VelocityWithMethodMetadata<TDummy>(this TDummy value)
            where TDummy : IDummy
        {
            throw new NotSupportedException();
        }

        public static Expression<Func<TDummy, double>> Narf<TDummy>()
            where TDummy : IDummy
        {
            return v => v.Distance / v.Time;
        }

        public static double VelocityWithInvalidSiblingResult<TDummy>(this TDummy value)
            where TDummy : IDummy
        {
            throw new NotSupportedException();
        }

        public static Func<TDummy, double> VelocityWithInvalidSiblingResult<TDummy>()
            where TDummy : IDummy
        {
            return v => v.Distance / v.Time;
        }

        public static double VelocityWithInvalidSiblingSignature<TDummy>(this TDummy value)
            where TDummy : IDummy
        {
            throw new NotSupportedException();
        }

        public static Expression<Func<double, double, double>> VelocityWithInvalidSiblingSignature<TDummy>()
            where TDummy : IDummy
        {
            return (d, t) => d / t;
        }

        public static double VelocityWithInvalidGenericSibling<TDummy>(this TDummy value)
            where TDummy : IDummy
        {
            throw new NotSupportedException();
        }

        public static Expression<Func<Dummy, double>> VelocityWithInvalidGenericSibling()
        {
            return v => v.Distance / v.Time;
        }
    }
}
