namespace NeinLinq;

/// <summary>
/// Rebinds a parameter expression to any expression.
/// </summary>
public class ParameterBinder : ExpressionVisitor
{
    private readonly ParameterExpression parameter;
    private readonly Expression replacement;

    /// <summary>
    /// Create an new binder.
    /// </summary>
    /// <param name="parameter">Parameter to find.</param>
    /// <param name="replacement">Expression to insert.</param>
    public ParameterBinder(ParameterExpression parameter, Expression replacement)
    {
        if (parameter is null)
            throw new ArgumentNullException(nameof(parameter));
        if (replacement is null)
            throw new ArgumentNullException(nameof(replacement));

        this.parameter = parameter;
        this.replacement = replacement;
    }

    /// <inheritdoc />
    protected override Expression VisitParameter(ParameterExpression node)
    {
        if (node is null)
            throw new ArgumentNullException(nameof(node));

        return node == parameter
            ? replacement
            : base.VisitParameter(node);
    }

    /// <inheritdoc />
    protected override Expression VisitInvocation(InvocationExpression node)
    {
        if (node is null)
            throw new ArgumentNullException(nameof(node));

        if (node.Expression == parameter && replacement is LambdaExpression lambda)
        {
            var binders = lambda.Parameters.Zip(node.Arguments,
                (p, a) => new ParameterBinder(p, a));

            return binders.Aggregate(lambda.Body, (e, b) => b.Visit(e));
        }

        return base.VisitInvocation(node);
    }
}
