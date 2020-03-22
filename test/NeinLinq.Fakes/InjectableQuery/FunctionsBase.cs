using System;
using System.Linq.Expressions;

#pragma warning disable CA1801
#pragma warning disable CA1822

namespace NeinLinq.Fakes.InjectableQuery
{
    public abstract class FunctionsBase : IFunctions
    {
        public double VelocityWithoutSibling(Dummy value)
            => throw new NotSupportedException();

        public double VelocityWithConvention(Dummy value)
            => throw new NotSupportedException();

        [InjectLambda]
        public double VelocityWithMetadata(Dummy value)
            => throw new NotSupportedException();

        [InjectLambda("ignore-me")]
        public double VelocityWithMethodMetadata(Dummy value)
            => throw new NotSupportedException();

        public double VelocityWithStupidSiblingResult(Dummy value)
            => throw new NotSupportedException();

        public double VelocityWithInvalidSiblingResult(Dummy value)
            => throw new NotSupportedException();

        public double VelocityWithStupidSiblingSignature(Dummy value)
            => throw new NotSupportedException();

        public double VelocityWithInvalidSiblingSignature(Dummy value)
            => throw new NotSupportedException();

        [InjectLambda("fail-me")]
        public double VelocityWithGenericArguments<TDummy, TOther>(TDummy value, TOther other)
            where TDummy : IDummy
            => throw new NotSupportedException();

        [InjectLambda("fail-me")]
        public double VelocityWithGenericArguments<TDummy>(TDummy value, object other)
            where TDummy : IDummy
            => throw new NotSupportedException();

        [InjectLambda("fail-me")]
        public double VelocityWithGenericArguments<TDummy>(object other)
            where TDummy : IDummy
            => throw new NotSupportedException();

        public double VelocityWithGenericArguments<TDummy>(TDummy value)
            where TDummy : IDummy
            => throw new NotSupportedException();

        public double VelocityWithInvalidGenericArguments<TDummy>(TDummy value)
            where TDummy : IDummy
            => throw new NotSupportedException();

        public double VelocityWithNonPublicSibling(Dummy value)
            => throw new NotSupportedException();

        [InjectLambda]
        public double VelocityWithHiddenSibling(Dummy value)
            => throw new NotSupportedException();

        public Expression<Func<Dummy, double>> VelocityWithHiddenSibling()
            => v => v.Distance / v.Time;

        [InjectLambda]
        public double VelocityWithAbstractSibling(Dummy value)
            => throw new NotSupportedException();

        public abstract Expression<Func<Dummy, double>> VelocityWithAbstractSibling();

        [InjectLambda]
        public double VelocityWithVirtualSibling(Dummy value)
            => throw new NotSupportedException();

        public virtual Expression<Func<Dummy, double>> VelocityWithVirtualSibling()
            => throw new InvalidOperationException("Implementing sibling is missing.");
    }
}
