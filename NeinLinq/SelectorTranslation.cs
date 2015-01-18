using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;

namespace NeinLinq
{
    /// <summary>
    /// Represents a translation of a given selector.
    /// </summary>
    /// <typeparam name="T">The type of the selector's source parameter.</typeparam>
    /// <typeparam name="U">The type of the selector's result parameter.</typeparam>
    [SuppressMessage("Microsoft.Naming", "CA1715:IdentifiersShouldHaveCorrectPrefix", MessageId = "T")]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "U")]
    public class SelectorTranslation<T, U>
    {
        private readonly Expression<Func<T, U>> selector;

        /// <summary>
        /// Starts translation of a given selector.
        /// </summary>
        /// <param name="selector">The selector expression to translate.</param>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public SelectorTranslation(Expression<Func<T, U>> selector)
        {
            if (selector == null)
                throw new ArgumentNullException("selector");

            this.selector = selector;
        }

        /// <summary>
        /// Translates a given selector for a given subtype using it's result parameter.
        /// </summary>
        /// <typeparam name="V">The type of the translated selector's result parameter.</typeparam>
        /// <returns>A translated selector expression.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        [SuppressMessage("Microsoft.Naming", "CA1715:IdentifiersShouldHaveCorrectPrefix", MessageId = "T")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "V")]
        public Expression<Func<T, V>> Result<V>()
            where V : U
        {
            var init = selector.Body as MemberInitExpression;

            if (init == null)
                throw new NotSupportedException("Only member init expressions are supported yet.");
            if (init.NewExpression.Arguments.Any())
                throw new NotSupportedException("Only parameterless constructors are supported yet.");

            return Expression.Lambda<Func<T, V>>(
                Expression.MemberInit(Expression.New(typeof(V)), init.Bindings), selector.Parameters);
        }

        /// <summary>
        /// Translates a given selector for a given subtype using it's source parameter.
        /// </summary>
        /// <typeparam name="V">The type of the translated selector's source parameter.</typeparam>
        /// <returns>A translated selector expression.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        [SuppressMessage("Microsoft.Naming", "CA1715:IdentifiersShouldHaveCorrectPrefix", MessageId = "T")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "V")]
        public Expression<Func<V, U>> Source<V>()
            where V : T
        {
            var t = selector.Parameters[0];
            var v = Expression.Parameter(typeof(V), t.Name);

            var binder = new ParameterBinder(t, v);

            return Expression.Lambda<Func<V, U>>(
                binder.Visit(selector.Body), v);
        }

        /// <summary>
        /// Continues translation of a given selector for a given subtype using it's source parameter.
        /// </summary>
        /// <typeparam name="V">The type of the translated selector's source parameter.</typeparam>
        /// <returns>Another translation object for the given selector.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        [SuppressMessage("Microsoft.Naming", "CA1715:IdentifiersShouldHaveCorrectPrefix", MessageId = "T")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "V")]
        public SelectorTranslation<V, U> Cross<V>()
            where V : T
        {
            return new SelectorTranslation<V, U>(Source<V>());
        }

        /// <summary>
        /// Translates a given selector for a given subtype using it's result parameter
        /// and combines it with another given selector by merging their member bindings.
        /// </summary>
        /// <typeparam name="V">The type of the translated selector's result parameter.</typeparam>
        /// <param name="value">The additional selector expression to combine.</param>
        /// <returns>A single translated and combined selector expression.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        [SuppressMessage("Microsoft.Naming", "CA1715:IdentifiersShouldHaveCorrectPrefix", MessageId = "T")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "V")]
        public Expression<Func<T, V>> Apply<V>(Expression<Func<T, V>> value)
            where V : U
        {
            return Result<V>().Apply(value);
        }
    }
}
