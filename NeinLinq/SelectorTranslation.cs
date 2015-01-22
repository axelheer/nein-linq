using System;
using System.Collections.Generic;
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
        /// Translates a given selector for a given related type using it's source parameter.
        /// </summary>
        /// <typeparam name="V">The type of the translated selector's source parameter.</typeparam>
        /// <param name="path">The path from the desired type to the given type.</param>
        /// <returns>A translated selector expression.</returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        [SuppressMessage("Microsoft.Naming", "CA1715:IdentifiersShouldHaveCorrectPrefix", MessageId = "T")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "V")]
        public Expression<Func<V, U>> Source<V>(Expression<Func<V, T>> path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            var t = selector.Parameters[0];
            var v = path.Parameters[0];

            var binder = new ParameterBinder(t, path.Body);

            return Expression.Lambda<Func<V, U>>(
                binder.Visit(selector.Body), v);
        }

        /// <summary>
        /// Translates a given selector for a given related type using it's source parameter.
        /// </summary>
        /// <typeparam name="V">The type of the translated selector's source parameter.</typeparam>
        /// <param name="translation">The translation from the desired type to the given type,
        /// using the initially given selector to be injected into a new selector.</param>
        /// <returns>A translated selector expression.</returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        [SuppressMessage("Microsoft.Naming", "CA1715:IdentifiersShouldHaveCorrectPrefix", MessageId = "T")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "V")]
        public Expression<Func<V, IEnumerable<U>>> Source<V>(Expression<Func<V, Func<T, U>, IEnumerable<U>>> translation)
        {
            throw new NotImplementedException();
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
        /// Translates a given selector for a given related type using it's result parameter.
        /// </summary>
        /// <typeparam name="V">The type of the translated selector's result parameter.</typeparam>
        /// <param name="path">The path from the desired type to the given type.</param>
        /// <returns>A translated selector expression.</returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        [SuppressMessage("Microsoft.Naming", "CA1715:IdentifiersShouldHaveCorrectPrefix", MessageId = "T")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "V")]
        public Expression<Func<T, V>> Result<V>(Expression<Func<V, U>> path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            var member = path.Body as MemberExpression;

            if (member == null)
                throw new NotSupportedException("Only member expressions are supported yet.");

            var bind = Expression.Bind(member.Member, selector.Body);

            return Expression.Lambda<Func<T, V>>(
                Expression.MemberInit(Expression.New(typeof(V)), bind), selector.Parameters);
        }

        /// <summary>
        /// Translates a given selector for a given related type using it's result parameter.
        /// </summary>
        /// <typeparam name="V">The type of the translated selector's result parameter.</typeparam>
        /// <param name="translation">The translation from the desired type to the given type,
        /// using the initially given selector to be injected into a new selector.</param>
        /// <returns>A translated selector expression.</returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        [SuppressMessage("Microsoft.Naming", "CA1715:IdentifiersShouldHaveCorrectPrefix", MessageId = "T")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "V")]
        public Expression<Func<T, V>> Result<V>(Expression<Func<T, Func<T, U>, V>> translation)
        {
            throw new NotImplementedException();
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
        /// Continues translation of a given selector for a given related type using it's source parameter.
        /// </summary>
        /// <typeparam name="V">The type of the translated selector's source parameter.</typeparam>
        /// <param name="path">The path from the desired type to the given type.</param>
        /// <returns>Another translation object for the given selector.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        [SuppressMessage("Microsoft.Naming", "CA1715:IdentifiersShouldHaveCorrectPrefix", MessageId = "T")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "V")]
        public SelectorTranslation<V, U> Cross<V>(Expression<Func<V, T>> path)
        {
            return new SelectorTranslation<V, U>(Source<V>(path));
        }

        /// <summary>
        /// Continues translation of a given selector for a given related type using it's source parameter.
        /// </summary>
        /// <typeparam name="V">The type of the translated selector's source parameter.</typeparam>
        /// <param name="translation">The translation from the desired type to the given type,
        /// using the initially given selector to be injected into a new selector.</param>
        /// <returns>Another translation object for the given selector.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        [SuppressMessage("Microsoft.Naming", "CA1715:IdentifiersShouldHaveCorrectPrefix", MessageId = "T")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "V")]
        public SelectorTranslation<V, IEnumerable<U>> Cross<V>(Expression<Func<V, Func<T, U>, IEnumerable<U>>> translation)
        {
            throw new NotImplementedException();
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

        /// <summary>
        /// Translates a given selector for a given related type using it's result parameter
        /// and combines it with another given selector by merging their member bindings.
        /// </summary>
        /// <typeparam name="V">The type of the translated selector's result parameter.</typeparam>
        /// <param name="path">The path from the desired type to the given type.</param>
        /// <param name="value">The additional selector expression to combine.</param>
        /// <returns>A single translated and combined selector expression.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        [SuppressMessage("Microsoft.Naming", "CA1715:IdentifiersShouldHaveCorrectPrefix", MessageId = "T")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "V")]
        public Expression<Func<T, V>> Apply<V>(Expression<Func<V, U>> path, Expression<Func<T, V>> value)
        {
            return Result<V>(path).Apply(value);
        }

        /// <summary>
        /// Translates a given selector for a given related type using it's result parameter
        /// and combines it with another given selector by merging their member bindings.
        /// </summary>
        /// <typeparam name="V">The type of the translated selector's result parameter.</typeparam>
        /// <param name="translation">The translation from the desired type to the given type,
        /// using the initially given selector to be injected into a new selector.</param>
        /// <param name="value">The additional selector expression to combine.</param>
        /// <returns>A single translated and combined selector expression.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        [SuppressMessage("Microsoft.Naming", "CA1715:IdentifiersShouldHaveCorrectPrefix", MessageId = "T")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "V")]
        public Expression<Func<T, V>> Apply<V>(Expression<Func<T, Func<T, U>, V>> translation, Expression<Func<T, V>> value)
        {
            throw new NotImplementedException();
        }
    }
}
