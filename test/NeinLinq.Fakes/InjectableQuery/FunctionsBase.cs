using System;

namespace NeinLinq.Fakes.InjectableQuery
{
    public abstract class FunctionsBase : IFunctions
    {
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
        public double VelocityWithMethodMetadata(Dummy value)
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

        [InjectLambda("fail-me")]
        public double VelocityWithGenericArguments<TDummy, TOther>(TDummy value, TOther other)
            where TDummy : IDummy
        {
            throw new NotImplementedException();
        }

        [InjectLambda("fail-me")]
        public double VelocityWithGenericArguments<TDummy>(TDummy value, object other)
            where TDummy : IDummy
        {
            throw new NotImplementedException();
        }

        [InjectLambda("fail-me")]
        public double VelocityWithGenericArguments<TDummy>(object other)
            where TDummy : IDummy
        {
            throw new NotImplementedException();
        }

        public double VelocityWithGenericArguments<TDummy>(TDummy value)
            where TDummy : IDummy
        {
            throw new NotImplementedException();
        }

        public double VelocityWithInvalidGenericArguments<TDummy>(TDummy value)
            where TDummy : IDummy
        {
            throw new NotImplementedException();
        }
    }
}
