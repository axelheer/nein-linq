using System;
using System.Reflection;

namespace NeinLinq
{
    /// <summary>
    /// Marks a method as injectable.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class InjectLambdaAttribute : Attribute
    {
        internal static readonly InjectLambdaAttribute None = new InjectLambdaAttribute();

        internal static InjectLambdaAttribute? GetCustomAttribute(MemberInfo element)
        {
            return (InjectLambdaAttribute?)GetCustomAttribute(element, typeof(InjectLambdaAttribute));
        }

        /// <summary>
        /// The target type for the method's expression. The current type, if null.
        /// </summary>
        public Type? Target { get; }

        /// <summary>
        /// The method's name for creating the method's expression. The same name, if null or empty.
        /// </summary>
        public string? Method { get; }

        /// <summary>
        /// Marks a method as injectable.
        /// </summary>
        public InjectLambdaAttribute()
        {
        }

        /// <summary>
        /// Marks a method as injectable.
        /// </summary>
        /// <param name="target">The target type for the method's expression.</param>
        public InjectLambdaAttribute(Type target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            Target = target;
        }

        /// <summary>
        /// Marks a method as injectable.
        /// </summary>
        /// <param name="target">The target type for the method's expression.</param>
        /// <param name="method">The method's name for creating the method's expression.</param>
        public InjectLambdaAttribute(Type target, string method)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (string.IsNullOrEmpty(method))
                throw new ArgumentNullException(nameof(method));

            Target = target;
            Method = method;
        }

        /// <summary>
        /// Marks a method as injectable.
        /// </summary>
        /// <param name="method">The method's name for creating the method's expression.</param>
        public InjectLambdaAttribute(string method)
        {
            if (string.IsNullOrEmpty(method))
                throw new ArgumentNullException(nameof(method));

            Method = method;
        }
    }
}
