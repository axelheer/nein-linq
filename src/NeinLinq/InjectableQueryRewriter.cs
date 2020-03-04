using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NeinLinq
{
    /// <summary>
    /// Expression visitor for injecting lambda expressions.
    /// </summary>
    public class InjectableQueryRewriter : ExpressionVisitor
    {
        private static readonly ObjectCache<MemberInfo, InjectLambdaMetadata> cache = new ObjectCache<MemberInfo, InjectLambdaMetadata>();

        private readonly Type[] whitelist;

        /// <summary>
        /// Creates a new injectable query rewriter.
        /// </summary>
        /// <param name="whitelist">A list of types to inject, whether marked as injectable or not.</param>
        public InjectableQueryRewriter(params Type[] whitelist)
        {
            if (whitelist is null)
                throw new ArgumentNullException(nameof(whitelist));

            foreach (var item in whitelist)
            {
                if (item is null)
                    throw new ArgumentOutOfRangeException(nameof(whitelist));
            }

            this.whitelist = whitelist;
        }

        /// <inheritdoc />
        protected override Expression VisitMember(MemberExpression node)
        {
            if (node is null)
                throw new ArgumentNullException(nameof(node));

            var property = node.Member as PropertyInfo;

            if (property?.GetGetMethod(true) is { } && property.GetSetMethod(true) is null)
            {
                // cache "meta-data" for performance reasons
                var data = cache.GetOrAdd(property, _ => InjectLambdaMetadata.Create(property));

                if (ShouldInject(property, data))
                {
                    var lambda = data.Lambda(null);

                    if (lambda is null)
                        throw new InvalidOperationException($"Lambda factory for {property.Name} returns null.");

                    // only one parameter for property getter
                    var argument = lambda.Parameters.Single();

                    // rebind expression for single (!) lambda argument
                    var binder = new ParameterBinder(argument, node.Expression);

                    return Visit(binder.Visit(lambda.Body));
                }
            }

            return base.VisitMember(node);
        }

        /// <inheritdoc />
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node is null)
                throw new ArgumentNullException(nameof(node));

            // cache "meta-data" for performance reasons
            var data = cache.GetOrAdd(node.Method, _ => InjectLambdaMetadata.Create(node.Method));

            if (ShouldInject(node.Method, data))
            {
                var lambda = data.Lambda(node.Object);

                if (lambda is null)
                    throw new InvalidOperationException($"Lambda factory for {node.Method.Name} returns null.");

                // rebind expression parameters for current arguments
                var binders = lambda.Parameters.Zip(node.Arguments,
                    (p, a) => new ParameterBinder(p, a));

                return Visit(binders.Aggregate(lambda.Body, (e, b) => b.Visit(e)));
            }

            return base.VisitMethodCall(node);
        }

        private bool ShouldInject(MemberInfo member, InjectLambdaMetadata data)
        {
            if (member.DeclaringType is null)
                throw new InvalidOperationException($"Member {member.Name} has no declaring type.");

            // inject only configured or white-listed targets
            return data.Config || whitelist.Any(member.DeclaringType.IsAssignableFrom);
        }
    }
}
