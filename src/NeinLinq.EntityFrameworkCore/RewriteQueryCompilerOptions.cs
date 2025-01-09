namespace NeinLinq;

internal sealed class RewriteQueryCompilerOptions
{
    public IReadOnlyList<ExpressionVisitor> Rewriters { get; }

    public RewriteQueryCompilerOptions(params ExpressionVisitor[] rewriters)
    {
        Rewriters = new List<ExpressionVisitor>(rewriters);
    }
}
