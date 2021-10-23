#pragma warning disable CA1812

namespace NeinLinq;

internal sealed class EntityQueryCompilerAdapterOptions
{
    public IReadOnlyList<ExpressionVisitor> Rewriters { get; }

    public EntityQueryCompilerAdapterOptions(params ExpressionVisitor[] rewriters)
    {
        Rewriters = new List<ExpressionVisitor>(rewriters);
    }
}
