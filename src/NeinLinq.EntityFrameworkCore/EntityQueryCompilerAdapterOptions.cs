using System.Collections.Generic;
using System.Linq.Expressions;

#pragma warning disable CA1812

namespace NeinLinq
{
    internal class EntityQueryCompilerAdapterOptions
    {
        public IReadOnlyList<ExpressionVisitor> Rewriters { get; }

        public EntityQueryCompilerAdapterOptions(params ExpressionVisitor[] rewriters)
        {
            Rewriters = new List<ExpressionVisitor>(rewriters);
        }
    }
}
