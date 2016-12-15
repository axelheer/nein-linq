using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NeinLinq
{
    /// <summary>
    /// Represents a translation of a given selector.
    /// </summary>
    /// <typeparam name="T">The type of the selector's source parameter.</typeparam>
    /// <typeparam name="U">The type of the selector's result parameter.</typeparam>
    public class SelectorTranslation<T, U>
    {
        readonly Expression<Func<T, U>> selector;

        /// <summary>
        /// Starts translation of a given selector.
        /// </summary>
        /// <param name="selector">The selector expression to translate.</param>
        public SelectorTranslation(Expression<Func<T, U>> selector)
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            this.selector = selector;
        }

        /// <summary>
        /// Translates a given selector for a given subtype using it's source parameter.
        /// </summary>
        /// <typeparam name="V">The type of the translated selector's source parameter.</typeparam>
        /// <returns>A translated selector expression.</returns>
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
        public Expression<Func<V, U>> Source<V>(Expression<Func<V, T>> path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

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
        public Expression<Func<V, U>> Source<V>(Expression<Func<V, Func<T, U>, U>> translation)
        {
            if (translation == null)
                throw new ArgumentNullException(nameof(translation));

            var v = translation.Parameters[0];
            var s = translation.Parameters[1];

            var binder = new ParameterBinder(s, selector);

            return Expression.Lambda<Func<V, U>>(
                binder.Visit(translation.Body), v);
        }

        /// <summary>
        /// Translates a given selector for a given related type using it's source parameter.
        /// </summary>
        /// <typeparam name="V">The type of the translated selector's source parameter.</typeparam>
        /// <param name="translation">The translation from the desired type to the given type,
        /// using the initially given selector to be injected into a new selector.</param>
        /// <returns>A translated selector expression.</returns>
        public Expression<Func<V, IEnumerable<U>>> Source<V>(Expression<Func<V, Func<T, U>, IEnumerable<U>>> translation)
        {
            if (translation == null)
                throw new ArgumentNullException(nameof(translation));

            var v = translation.Parameters[0];
            var s = translation.Parameters[1];

            var binder = new ParameterBinder(s, selector);

            return Expression.Lambda<Func<V, IEnumerable<U>>>(
                binder.Visit(translation.Body), v);
        }

        /// <summary>
        /// Translates a given selector for a given subtype using it's result parameter.
        /// </summary>
        /// <typeparam name="V">The type of the translated selector's result parameter.</typeparam>
        /// <returns>A translated selector expression.</returns>
        public Expression<Func<T, V>> Result<V>()
            where V : U
        {
            var init = selector.Body as MemberInitExpression;

            if (init == null)
                throw new NotSupportedException("Only member init expressions are supported yet.");
            if (init.NewExpression.Arguments.Any())
                throw new NotSupportedException("Only parameterless constructors are supported yet.");

            var t = selector.Parameters[0];

            return Expression.Lambda<Func<T, V>>(
                Expression.MemberInit(Expression.New(typeof(V)), init.Bindings), t);
        }

        /// <summary>
        /// Translates a given selector for a given related type using it's result parameter.
        /// </summary>
        /// <typeparam name="V">The type of the translated selector's result parameter.</typeparam>
        /// <param name="path">The path from the desired type to the given type.</param>
        /// <returns>A translated selector expression.</returns>
        public Expression<Func<T, V>> Result<V>(Expression<Func<V, U>> path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            var member = path.Body as MemberExpression;
            if (member == null)
                throw new NotSupportedException("Only member expressions are supported yet.");

            var t = selector.Parameters[0];

            var bind = Expression.Bind(member.Member, selector.Body);

            return Expression.Lambda<Func<T, V>>(
                Expression.MemberInit(Expression.New(typeof(V)), bind), t);
        }

        /// <summary>
        /// Translates a given selector for a given related type using it's result parameter.
        /// </summary>
        /// <typeparam name="V">The type of the translated selector's result parameter.</typeparam>
        /// <param name="translation">The translation from the desired type to the given type,
        /// using the initially given selector to be injected into a new selector.</param>
        /// <returns>A translated selector expression.</returns>
        public Expression<Func<T, V>> Result<V>(Expression<Func<T, Func<T, U>, V>> translation)
        {
            if (translation == null)
                throw new ArgumentNullException(nameof(translation));

            var t = translation.Parameters[0];
            var s = translation.Parameters[1];

            var binder = new ParameterBinder(s, selector);

            return Expression.Lambda<Func<T, V>>(
                binder.Visit(translation.Body), t);
        }

        /// <summary>
        /// Continues translation of a given selector for a given subtype using it's source parameter.
        /// </summary>
        /// <typeparam name="V">The type of the translated selector's source parameter.</typeparam>
        /// <returns>Another translation object for the given selector.</returns>
        public SelectorTranslation<V, U> Cross<V>()
            where V : T
        {
            return Source<V>().Translate();
        }

        /// <summary>
        /// Continues translation of a given selector for a given related type using it's source parameter.
        /// </summary>
        /// <typeparam name="V">The type of the translated selector's source parameter.</typeparam>
        /// <param name="path">The path from the desired type to the given type.</param>
        /// <returns>Another translation object for the given selector.</returns>
        public SelectorTranslation<V, U> Cross<V>(Expression<Func<V, T>> path)
        {
            return Source(path).Translate();
        }

        /// <summary>
        /// Continues translation of a given selector for a given related type using it's source parameter.
        /// </summary>
        /// <typeparam name="V">The type of the translated selector's source parameter.</typeparam>
        /// <param name="translation">The translation from the desired type to the given type,
        /// using the initially given selector to be injected into a new selector.</param>
        /// <returns>Another translation object for the given selector.</returns>
        public SelectorTranslation<V, U> Cross<V>(Expression<Func<V, Func<T, U>, U>> translation)
        {
            return Source(translation).Translate();
        }

        /// <summary>
        /// Continues translation of a given selector for a given related type using it's source parameter.
        /// </summary>
        /// <typeparam name="V">The type of the translated selector's source parameter.</typeparam>
        /// <param name="translation">The translation from the desired type to the given type,
        /// using the initially given selector to be injected into a new selector.</param>
        /// <returns>Another translation object for the given selector.</returns>
        public SelectorTranslation<V, IEnumerable<U>> Cross<V>(Expression<Func<V, Func<T, U>, IEnumerable<U>>> translation)
        {
            return Source(translation).Translate();
        }

        /// <summary>
        /// Translates a given selector for a given subtype using it's result parameter
        /// and combines it with another given selector by merging their member bindings.
        /// </summary>
        /// <typeparam name="V">The type of the translated selector's result parameter.</typeparam>
        /// <param name="value">The additional selector expression to combine.</param>
        /// <returns>A single translated and combined selector expression.</returns>
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
        public Expression<Func<T, V>> Apply<V>(Expression<Func<V, U>> path, Expression<Func<T, V>> value)
        {
            return Result(path).Apply(value);
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
        public Expression<Func<T, V>> Apply<V>(Expression<Func<T, Func<T, U>, V>> translation, Expression<Func<T, V>> value)
        {
            return Result(translation).Apply(value);
        }

        /// <summary>
        /// Translates a given selector for given subtypes using it's source and result parameter
        /// and combines it with another given selector by merging their member bindings.
        /// </summary>
        /// <typeparam name="V">The type of the translated selector's source parameter.</typeparam>
        /// <typeparam name="W">The type of the translated selector's result parameter.</typeparam>
        /// <param name="value">The additional selector expression to combine.</param>
        /// <returns>A single translated and combined selector expression.</returns>
        public Expression<Func<V, W>> To<V, W>(Expression<Func<V, W>> value = null)
            where V : T
            where W : U
        {
            var result = Cross<V>().Result<W>();

            return value != null ? result.Apply(value) : result;
        }

        /// <summary>
        /// Translates a given selector for a given related type using it's source and result parameter
        /// and combines it with another given selector by merging their member bindings.
        /// </summary>
        /// <typeparam name="V">The type of the translated selector's source parameter.</typeparam>
        /// <typeparam name="W">The type of the translated selector's result parameter.</typeparam>
        /// <param name="sourcePath">The path from the desired source type to the given type.</param>
        /// <param name="resultPath">The path from the desired result type to the given type.</param>
        /// <param name="value">The additional selector expression to combine.</param>
        /// <returns>A single translated and combined selector expression.</returns>
        public Expression<Func<V, W>> To<V, W>(Expression<Func<V, T>> sourcePath, Expression<Func<W, U>> resultPath, Expression<Func<V, W>> value = null)
        {
            var result = Cross(sourcePath).Result(resultPath);

            return value != null ? result.Apply(value) : result;
        }

        /// <summary>
        /// Translates a given selector for a given related type using it's source and result parameter
        /// and combines it with another given selector by merging their member bindings.
        /// </summary>
        /// <typeparam name="V">The type of the translated selector's source parameter.</typeparam>
        /// <typeparam name="W">The type of the translated selector's result parameter.</typeparam>
        /// <param name="translation">The translation from the desired type to the given type,
        /// using the initially given selector to be injected into a new selector.</param>
        /// <param name="value">The additional selector expression to combine.</param>
        /// <returns>A single translated and combined selector expression.</returns>
        public Expression<Func<V, W>> To<V, W>(Expression<Func<V, Func<T, U>, W>> translation, Expression<Func<V, W>> value = null)
        {
            if (translation == null)
                throw new ArgumentNullException(nameof(translation));

            var v = translation.Parameters[0];
            var s = translation.Parameters[1];

            var binder = new ParameterBinder(s, selector);

            var result = Expression.Lambda<Func<V, W>>(
                binder.Visit(translation.Body), v);

            return value != null ? result.Apply(value) : result;
        }
    }
}
