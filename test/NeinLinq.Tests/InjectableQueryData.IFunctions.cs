namespace NeinLinq.Tests.InjectableQueryData
{
    public interface IFunctions
    {
        double VelocityWithoutSibling(Dummy value);

        double VelocityWithConvention(Dummy value);

        [InjectLambda]
        double VelocityWithMetadata(Dummy value);

        [InjectLambda("ignore-me")]
        double VelocityWithMethodMetadata(Dummy value);

        double VelocityWithInvalidSiblingResult(Dummy value);

        double VelocityWithInvalidSiblingSignature(Dummy value);
    }
}
