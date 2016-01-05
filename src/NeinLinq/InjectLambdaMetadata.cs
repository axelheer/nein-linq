using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NeinLinq
{
    sealed class InjectLambdaMetadata
    {
        readonly Type target;

        public Type Target
        {
            get { return target; }
        }

        readonly string method;

        public string Method
        {
            get { return method; }
        }

        readonly bool config;

        public bool Config
        {
            get { return config; }
        }

        InjectLambdaMetadata(Type target, string method, bool config, bool instance, Type[] args, Type result)
        {
            this.target = target;
            this.method = method;
            this.config = config;

            factory = new Lazy<Func<Expression, LambdaExpression>>(() =>
            {
                if (!instance)
                {
                    // retrieve validated factory method
                    var factoryMethod = FactoryMethod(target, method, args, result);
                    if (factoryMethod == null)
                        return _ => null;

                    // compile factory call for performance reasons :-)
                    return Expression.Lambda<Func<Expression, LambdaExpression>>(
                        Expression.Call(factoryMethod), Expression.Parameter(typeof(Expression))).Compile();
                }

                return value =>
                {
                    // retrieve actual target object, compiles every time... :-(
                    var actualTarget = Expression.Lambda<Func<object>>(Expression.Convert(value, typeof(object))).Compile()();
                    if (actualTarget == null)
                        return null;

                    var actualTargetType = actualTarget.GetType();

                    // actual method may provide different information (freaks)
                    var actualMethod = actualTargetType.GetRuntimeMethod(method, args);
                    if (actualMethod == null)
                        return null;

                    var actualMethodName = actualMethod.Name;

                    // configuration over convention, if any (again)
                    var metadata = actualMethod.GetCustomAttribute<InjectLambdaAttribute>();
                    if (metadata != null && !string.IsNullOrEmpty(metadata.Method))
                        actualMethodName = metadata.Method;

                    // retrieve validated factory method
                    var factoryMethod = FactoryMethod(actualTargetType, actualMethodName, args, result);
                    if (factoryMethod == null)
                        return null;

                    // finally call lambda factory *uff*
                    return (LambdaExpression)factoryMethod.Invoke(actualTarget, null);
                };
            });
        }

        readonly Lazy<Func<Expression, LambdaExpression>> factory;

        internal LambdaExpression Replacement(Expression value)
        {
            return factory.Value(value);
        }

        internal static InjectLambdaMetadata Create(MethodInfo call)
        {
            // inject by convention
            var target = call.DeclaringType;
            var method = call.Name;
            var config = false;

            // special treatment for instance methods
            var instance = !call.IsStatic;

            // configuration over convention, if any
            var metadata = call.GetCustomAttribute<InjectLambdaAttribute>();
            if (metadata != null)
            {
                if (!instance)
                {
                    if (metadata.Target != null)
                        target = metadata.Target;
                    if (!string.IsNullOrEmpty(metadata.Method))
                        method = metadata.Method;
                }
                config = true;
            }

            // retrieve method's signature
            var result = call.ReturnParameter.ParameterType;
            var args = call.GetParameters().Select(p => p.ParameterType).ToArray();

            return new InjectLambdaMetadata(target, method, config, instance, args, result);
        }

        static readonly Type[] emptyTypes = new Type[0];

        static MethodInfo FactoryMethod(Type target, string method, Type[] args, Type result)
        {
            // assume method without any parameters
            var factoryMethod = target.GetRuntimeMethod(method, emptyTypes);
            if (factoryMethod == null)
                return null;

            // method returns lambda expression?
            var factoryType = factoryMethod.ReturnType;
            if (!factoryType.IsConstructedGenericType || factoryType.GetGenericTypeDefinition() != typeof(Expression<>))
                return null;

            // lambda signature matches original method's signature?
            var factorySignature = factoryType.GenericTypeArguments[0].GetRuntimeMethod("Invoke", args);
            if (factorySignature == null || factorySignature.ReturnParameter.ParameterType != result)
                return null;

            return factoryMethod;
        }
    }
}
