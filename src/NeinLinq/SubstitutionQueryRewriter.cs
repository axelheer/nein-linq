namespace NeinLinq;

/// <summary>
/// Expression visitor for replacing method types.
/// </summary>
public class SubstitutionQueryRewriter : ExpressionVisitor
{
    private readonly Type from;
    private readonly Type to;

    /// <summary>
    /// Creates a new substitution query rewriter.
    /// </summary>
    /// <param name="from">A type to replace.</param>
    /// <param name="to">A type to use instead.</param>
    public SubstitutionQueryRewriter(Type from, Type to)
    {
        if (from is null)
            throw new ArgumentNullException(nameof(from));
        if (to is null)
            throw new ArgumentNullException(nameof(to));

        this.from = from;
        this.to = to;
    }

    /// <inheritdoc />
    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        if (node is null)
            throw new ArgumentNullException(nameof(node));

        if (node.Method.DeclaringType == from)
        {
            var typeArguments = node.Method.GetGenericArguments();
            var arguments = node.Arguments.Select(Visit).ToArray();

            // assume equivalent method signature
            return Expression.Call(to, node.Method.Name, typeArguments, arguments!);
        }

        return base.VisitMethodCall(node);
    }
}
