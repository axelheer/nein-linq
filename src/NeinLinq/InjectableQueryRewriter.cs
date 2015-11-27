using System;
using System.Collections.Generic;
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
        static readonly object cacheLock = new object();

        static readonly Dictionary<MethodInfo, InjectLambdaMetadata> cache =
            new Dictionary<MethodInfo, InjectLambdaMetadata>();

        readonly Type[] whitelist;

        /// <summary>
        /// Creates a new injectable query rewriter.
        /// </summary>
        /// <param name="whitelist">A list of types to inject, whether marked as injectable or not.</param>
        public InjectableQueryRewriter(params Type[] whitelist)
        {
            this.whitelist = whitelist ?? new Type[0];
        }

        /// <inheritdoc />
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node != null && node.Method != null)
            {
                // cache "metadata" for performance reasons
                var data = default(InjectLambdaMetadata);
                lock (cacheLock)
                {
                    if (!cache.TryGetValue(node.Method, out data))
                    {
                        data = InjectLambdaMetadata.Create(node.Method);
                        cache.Add(node.Method, data);
                    }
                }

                // inject only configured or whitelisted targets
                if (data.Config || whitelist.Contains(data.Target))
                {
                    var replacement = data.Replacement(node.Object);
                    if (replacement == null)
                        throw new InvalidOperationException(
                            string.Concat("Unable to retrieve lambda expression from ",
                                data.Target.FullName, ".", data.Method, "."));

                    // rebind expression parameters for current arguments
                    var binders = replacement.Parameters.Zip(node.Arguments,
                        (p, a) => new ParameterBinder(p, a));

                    return Visit(binders.Aggregate(replacement.Body, (e, b) => b.Visit(e)));
                }
            }

            return base.VisitMethodCall(node);
        }
    }
}
