namespace NeinLinq;

#pragma warning disable CA1812

internal sealed class EntityQueryCompilerAdapterOptions
{
    public IReadOnlyList<ExpressionVisitor> Rewriters { get; }

    public EntityQueryCompilerAdapterOptions(params ExpressionVisitor[] rewriters)
    {
        Rewriters = new List<ExpressionVisitor>(rewriters);
    }
}

#pragma warning restore CA1812
