using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NeinLinq
{
    sealed class InjectLambdaMetadata
    {
        readonly bool config;

        public bool Config
        {
            get { return config; }
        }

        readonly Lazy<Func<Expression, LambdaExpression>> lambda;

        public LambdaExpression Lambda(Expression value)
        {
            return lambda.Value(value);
        }

        InjectLambdaMetadata(bool config, Lazy<Func<Expression, LambdaExpression>> lambda)
        {
            this.config = config;
            this.lambda = lambda;
        }

        public static InjectLambdaMetadata Create(MethodInfo call)
        {
            var metadata = call.GetCustomAttribute<InjectLambdaAttribute>();

            var lambdaFactory = new Lazy<Func<Expression, LambdaExpression>>(() => LambdaFactory(call, metadata));

            return new InjectLambdaMetadata(metadata != null, lambdaFactory);
        }

        static Func<Expression, LambdaExpression> LambdaFactory(MethodInfo call, InjectLambdaAttribute metadata)
        {
            // retrieve method's signature
            var args = call.GetParameters().Select(p => p.ParameterType).ToArray();
            var result = call.ReturnParameter.ParameterType;

            // special ultra-fast treatment for static methods and sealed classes
            if (call.IsStatic || call.DeclaringType.GetTypeInfo().IsSealed)
            {
                return FixedLambdaFactory(call, metadata, args, result);
            }

            // dynamic but not that fast treatment for other stuff
            return DynamicLambdaFactory(call, args, result);
        }

        static Func<Expression, LambdaExpression> FixedLambdaFactory(MethodInfo call, InjectLambdaAttribute metadata, Type[] args, Type result)
        {
            // inject by convention
            var target = call.DeclaringType;
            var method = call.Name;

            // apply configuration, if any
            if (metadata != null)
            {
                if (metadata.Target != null)
                    target = metadata.Target;
                if (!string.IsNullOrEmpty(metadata.Method))
                    method = metadata.Method;
            }

            // retrieve validated factory method once
            var factory = FactoryMethod(target, method, args, result);

            if (call.IsStatic)
            {
                // compile factory call for performance reasons :-)
                return Expression.Lambda<Func<Expression, LambdaExpression>>(
                    Expression.Call(factory), Expression.Parameter(typeof(Expression))).Compile();
            }

            // call actual target object, compiles every time during execution... :-|
            return value => Expression.Lambda<Func<LambdaExpression>>(Expression.Call(value, factory)).Compile()();
        }

        static Func<Expression, LambdaExpression> DynamicLambdaFactory(MethodInfo call, Type[] args, Type result)
        {
            return value =>
            {
                // retrieve actual target object, compiles every time and needs reflection too... :-(
                var targetObject = Expression.Lambda<Func<object>>(Expression.Convert(value, typeof(object))).Compile()();
                if (targetObject == null)
                    throw new InvalidOperationException($"Unable to retrieve object from '{value}': expression evaluates to null.");

                var target = targetObject.GetType();

                // actual method may provide different information
                var concreteCall = target.GetRuntimeMethod(call.Name, args);
                if (concreteCall == null)
                    throw new InvalidOperationException($"Unable to retrieve lambda meta-data from {target.FullName}.{call.Name}: what evil treachery is this?");

                var method = concreteCall.Name;

                // configuration over convention, if any
                var metadata = concreteCall.GetCustomAttribute<InjectLambdaAttribute>();
                if (metadata != null && !string.IsNullOrEmpty(metadata.Method))
                    method = metadata.Method;

                // retrieve validated factory method
                var factory = FactoryMethod(target, method, args, result);

                // finally call lambda factory *uff*
                return (LambdaExpression)factory.Invoke(targetObject, null);
            };
        }

        static readonly Type[] emptyTypes = new Type[0];

        static MethodInfo FactoryMethod(Type target, string method, Type[] args, Type result)
        {
            // assume method without any parameters
            var factory = target.GetRuntimeMethod(method, emptyTypes);
            if (factory == null)
                throw new InvalidOperationException($"Unable to retrieve lambda expression from {target.FullName}.{method}: no parameterless method found.");

            // method returns lambda expression?
            var returns = factory.ReturnType;
            if (!returns.IsConstructedGenericType || returns.GetGenericTypeDefinition() != typeof(Expression<>))
                throw new InvalidOperationException($"Unable to retrieve lambda expression from {target.FullName}.{method}: method does not return generic lambda expression.");

            // lambda signature matches original method's signature?
            var signature = returns.GenericTypeArguments[0].GetRuntimeMethod("Invoke", args);
            if (signature == null || signature.ReturnParameter.ParameterType != result)
                throw new InvalidOperationException($"Unable to retrieve lambda expression from {target.FullName}.{method}: method returns lambda expression with non matching signature.");

            return factory;
        }
    }
}
