using System;

namespace NeinLinq
{
    /// <summary>
    /// Marks a method as injectable.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class InjectLambdaAttribute : Attribute
    {
        /// <summary>
        /// The target type for the method's expression. The current type, if null.
        /// </summary>
        public Type Target { get; private set; }

        /// <summary>
        /// The method's name for creating the method's expression. The same name, if null or empty.
        /// </summary>
        public string Method { get; private set; }

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
            Target = target;
        }

        /// <summary>
        /// Marks a method as injectable.
        /// </summary>
        /// <param name="target">The target type for the method's expression.</param>
        /// <param name="method">The method's name for creating the method's expression.</param>
        public InjectLambdaAttribute(Type target, string method)
        {
            Target = target;
            Method = method;
        }

        /// <summary>
        /// Marks a method as injectable.
        /// </summary>
        /// <param name="method">The method's name for creating the method's expression.</param>
        public InjectLambdaAttribute(string method)
        {
            Method = method;
        }
    }
}
