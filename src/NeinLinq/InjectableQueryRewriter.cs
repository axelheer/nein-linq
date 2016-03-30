using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NeinLinq
{
    /// <summary>
    /// Expression visitor for injecting lambda expressions.
    /// </summary>
    /// <remarks>
    /// Use <see cref="InjectableQueryBuilder" /> to inject lambda expressions.
    /// </remarks>
    public class InjectableQueryRewriter : ExpressionVisitor
    {
        static readonly ConcurrentDictionary<MemberInfo, InjectLambdaMetadata> cache =
            new ConcurrentDictionary<MemberInfo, InjectLambdaMetadata>();

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

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node != null && node.Member != null)
            {
                var property = node.Member as PropertyInfo;

                if (property != null && property.GetMethod != null)
                {
                    // cache "meta-data" for performance reasons
                    var data = cache.GetOrAdd(property, p => InjectLambdaMetadata.Create((PropertyInfo)p));

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
            }

            return base.VisitMember(node);
        }

        /// <inheritdoc />
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node != null && node.Method != null)
            {
                // cache "meta-data" for performance reasons
                var data = cache.GetOrAdd(node.Method, m => InjectLambdaMetadata.Create((MethodInfo)m));

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

            if (member is PropertyInfo)
                return false;

            // ...or white-listed targets
            var info = member.DeclaringType.GetTypeInfo();
            return whitelist.Any(info.IsAssignableFrom);
        }
    }
}
