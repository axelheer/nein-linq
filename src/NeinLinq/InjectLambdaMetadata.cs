using System;
using System.Linq.Expressions;
using System.Reflection;

namespace NeinLinq
{
    internal sealed class InjectLambdaMetadata
    {
        private readonly bool config;

        public bool Config => config;

        private readonly Lazy<Func<Expression, LambdaExpression>> lambda;

        public LambdaExpression Lambda(Expression value) => lambda.Value(value);

        private InjectLambdaMetadata(bool config, Lazy<Func<Expression, LambdaExpression>> lambda)
        {
            this.config = config;
            this.lambda = lambda;
        }

        public static InjectLambdaMetadata Create(MethodInfo method)
        {
            var metadata = method.GetCustomAttribute<InjectLambdaAttribute>();

            var lambdaFactory = new Lazy<Func<Expression, LambdaExpression>>(() =>
                LambdaFactory(method, metadata ?? InjectLambdaAttribute.None));

            return new InjectLambdaMetadata(metadata != null, lambdaFactory);
        }

        public static InjectLambdaMetadata Create(PropertyInfo property)
        {
            var metadata = property.GetCustomAttribute<InjectLambdaAttribute>()
                ?? property.GetMethod.GetCustomAttribute<InjectLambdaAttribute>();

            var lambdaFactory = new Lazy<Func<Expression, LambdaExpression>>(() =>
                LambdaFactory(property, metadata ?? InjectLambdaAttribute.None));

            return new InjectLambdaMetadata(metadata != null, lambdaFactory);
        }

        private static Func<Expression, LambdaExpression> LambdaFactory(MethodInfo method, InjectLambdaAttribute metadata)
        {
            // retrieve method's signature
            var signature = new InjectLambdaSignature(method);

            // special ultra-fast treatment for static methods and sealed classes
            if (method.IsStatic || method.DeclaringType.IsSealed)
            {
                return FixedLambdaFactory(metadata.Target ?? method.DeclaringType, metadata.Method ?? method.Name, signature);
            }

            // dynamic but not that fast treatment for other stuff
            return DynamicLambdaFactory(method.Name, signature);
        }

        private static Func<Expression, LambdaExpression> LambdaFactory(PropertyInfo property, InjectLambdaAttribute metadata)
        {
            // retrieve method's signature
            var signature = new InjectLambdaSignature(property);

            // apply "Expr" convention for property "overloading"
            var method = metadata.Target == null ? property.Name + "Expr" : property.Name;

            // special treatment for super-heroic property getters
            return FixedLambdaFactory(metadata.Target ?? property.DeclaringType, metadata.Method ?? method, signature);
        }

        private static Func<Expression, LambdaExpression> FixedLambdaFactory(Type target, string method, InjectLambdaSignature signature)
        {
            // retrieve validated factory method once
            var factory = signature.FindFactory(target, method);

            if (factory.IsStatic)
            {
                // compile factory call for performance reasons :-)
                return Expression.Lambda<Func<Expression, LambdaExpression>>(
                    Expression.Call(factory), Expression.Parameter(typeof(Expression))).Compile();
            }

            // call actual target object, compiles every time during execution... :-|
            return value => Expression.Lambda<Func<LambdaExpression>>(Expression.Call(value, factory)).Compile()();
        }

        private static Func<Expression, LambdaExpression> DynamicLambdaFactory(string method, InjectLambdaSignature signature)
        {
            return value =>
            {
                // retrieve actual target object, compiles every time and needs reflection too... :-(
                var targetObject = Expression.Lambda<Func<object>>(Expression.Convert(value, typeof(object))).Compile()();

                // retrieve actual target type at runtime, whatever it may be
                var target = targetObject.GetType();

                // actual method may provide different information
                var concreteMethod = signature.FindMatch(target, method);

                // configuration over convention, if any
                var metadata = concreteMethod.GetCustomAttribute<InjectLambdaAttribute>() ?? InjectLambdaAttribute.None;

                // retrieve validated factory method
                var factory = signature.FindFactory(target, metadata.Method ?? method);

                // finally call lambda factory *uff*
                return (LambdaExpression)factory.Invoke(targetObject, null);
            };
        }
    }
}
