namespace NeinLinq.Fakes.InjectableQuery
{
    public interface IFunctions
    {
        double VelocityWithoutSibling(Dummy value);

        double VelocityWithConvention(Dummy value);

        [InjectLambda]
        double VelocityWithMetadata(Dummy value);

        [InjectLambda("ignore-me")]
        double VelocityWithMethodMetadata(Dummy value);

        double VelocityWithStupidSiblingResult(Dummy value);

        double VelocityWithInvalidSiblingResult(Dummy value);

        double VelocityWithStupidSiblingSignature(Dummy value);

        double VelocityWithInvalidSiblingSignature(Dummy value);

        double VelocityWithGenericArguments<TDummy>(TDummy value)
            where TDummy : IDummy;

        double VelocityWithInvalidGenericArguments<TDummy>(TDummy value)
            where TDummy : IDummy;
    }
}
