using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace NeinLinq
{
    /// <summary>
    /// Provides methods for combining and translating lambda expressions representing predicates.
    /// </summary>
    public static class PredicateTranslator
    {
        /// <summary>
        /// Starts translation of a given predicate.
        /// </summary>
        /// <typeparam name="T">The type of the predicate's parameter.</typeparam>
        /// <param name="predicate">The predicate expression to translate.</param>
        /// <returns>A translation object for the given predicate.</returns>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static PredicateTranslation<T> Translate<T>(this Expression<Func<T, bool>> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException("predicate");

            return new PredicateTranslation<T>(predicate);
        }

        /// <summary>
        /// Combines two given predicates using a conditional AND operation.
        /// </summary>
        /// <typeparam name="T">The type of the predicate's parameter.</typeparam>
        /// <param name="left">The first predicate expression to combine.</param>
        /// <param name="right">The second predicate expression to combine.</param>
        /// <returns>A single combined predicate expression.</returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            if (left == null)
                throw new ArgumentNullException("left");
            if (right == null)
                throw new ArgumentNullException("right");

            var l = left.Parameters[0];
            var r = right.Parameters[0];

            var binder = new ParameterBinder(l, r);

            return Expression.Lambda<Func<T, bool>>(
                Expression.AndAlso(binder.Visit(left.Body), right.Body), r);
        }

        /// <summary>
        /// Combines two given predicates using a conditional OR operation.
        /// </summary>
        /// <typeparam name="T">The type of the predicate's parameter.</typeparam>
        /// <param name="left">The first predicate expression to combine.</param>
        /// <param name="right">The second predicate expression to combine.</param>
        /// <returns>A single combined predicate expression.</returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            if (left == null)
                throw new ArgumentNullException("left");
            if (right == null)
                throw new ArgumentNullException("right");

            var l = left.Parameters[0];
            var r = right.Parameters[0];

            var binder = new ParameterBinder(l, r);

            return Expression.Lambda<Func<T, bool>>(
                Expression.OrElse(binder.Visit(left.Body), right.Body), r);
        }

        /// <summary>
        /// Negates the given predicate using a binary NOT operation.
        /// </summary>
        /// <typeparam name="T">The type of the predicate's parameter.</typeparam>
        /// <param name="predicate">The predicate expression.</param>
        /// <returns>A predicate expression.</returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException("predicate");

            return Expression.Lambda<Func<T, bool>>(
                Expression.Not(predicate.Body), predicate.Parameters);
        }
    }
}
