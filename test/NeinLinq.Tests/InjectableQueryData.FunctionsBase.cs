using System;

namespace NeinLinq.Tests.InjectableQueryData
{
    public abstract class FunctionsBase : IFunctions
    {
        readonly int digits;

        public int Digits
        {
            get { return digits; }
        }

        protected FunctionsBase(int digits)
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
    }
}
