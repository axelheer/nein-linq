using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

#if NET40

using TypeInfo = System.Type;

#endif

namespace NeinLinq
{
    /// <summary>
    /// Expression visitor for injecting lambda expressions.
    /// </summary>
    public class InjectableQueryRewriter : ExpressionVisitor
    {
        static readonly ObjectCache<MemberInfo, InjectLambdaMetadata> cache = new ObjectCache<MemberInfo, InjectLambdaMetadata>();

        readonly TypeInfo[] whitelist;

        /// <summary>
        /// Creates a new injectable query rewriter.
        /// </summary>
        /// <param name="whitelist">A list of types to inject, whether marked as injectable or not.</param>
        public InjectableQueryRewriter(params Type[] whitelist)
        {
            if (whitelist == null)
                throw new ArgumentNullException(nameof(whitelist));
            if (whitelist.Contains(null))
                throw new ArgumentOutOfRangeException(nameof(whitelist));

            this.whitelist = whitelist.Length != 0 ? whitelist.Select(t => t.GetTypeInfo()).ToArray() : null;
        }

        /// <inheritdoc />
        protected override Expression VisitMember(MemberExpression node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            var property = node.Member as PropertyInfo;

            if (property?.GetMethod() != null && property.SetMethod() == null)
            {
                // cache "meta-data" for performance reasons
                var data = cache.GetOrAdd(property, _ => InjectLambdaMetadata.Create(property));

                if (ShouldInject(property, data))
                {
                    var lambda = data.Lambda(null);

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
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (node.Method != null)
            {
                // cache "meta-data" for performance reasons
                var data = cache.GetOrAdd(node.Method, _ => InjectLambdaMetadata.Create(node.Method));

                if (ShouldInject(node.Method, data))
                {
                    var lambda = data.Lambda(node.Object);

                    // rebind expression parameters for current arguments
                    var binders = lambda.Parameters.Zip(node.Arguments,
                        (p, a) => new ParameterBinder(p, a));

                    return Visit(binders.Aggregate(lambda.Body, (e, b) => b.Visit(e)));
                }
            }

            return base.VisitMethodCall(node);
        }

        bool ShouldInject(MemberInfo member, InjectLambdaMetadata data)
        {
            // inject only configured...
            if (data.Config)
                return true;

            if (whitelist == null)
                return false;

            // ...or white-listed targets
            var info = member.DeclaringType.GetTypeInfo();
            return whitelist.Any(info.IsAssignableFrom);
        }
    }
}
