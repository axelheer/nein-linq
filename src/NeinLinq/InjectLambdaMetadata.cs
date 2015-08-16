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

        InjectLambdaMetadata(Type target, string method, bool config, Type returns, params Type[] args)
        {
            this.target = target;
            this.method = method;
            this.config = config;

            factory = new Lazy<Func<Expression, LambdaExpression>>(() =>
            {
                // assume method without any parameters
                var factoryMethod = target.GetMethodInfo(method, new Type[0]);
                if (factoryMethod == null)
                    return null;

                // method returns lambda expression?
                var expressionInfo = factoryMethod.ReturnType;
                if (expressionInfo.IsInheritorOf(typeof(LambdaExpression)) == false)
                    return null;

                // lambda signature matches original method's signature?
                var delegateInfo = expressionInfo.GetGenericTypeArguments()[0];
                var delegateSignature = delegateInfo.GetMethodInfo("Invoke", args);
                if (delegateSignature == null || delegateSignature.ReturnParameter.ParameterType != returns)
                    return null;

                if (factoryMethod.IsStatic)
                {
                    // compile factory call for performance reasons
                    return Expression.Lambda<Func<Expression, LambdaExpression>>(
                        Expression.Call(factoryMethod), Expression.Parameter(typeof(Expression))).Compile();
                }
                else
                {
                    // parameterize actual target value, compiles every time... :-(
                    return value => Expression.Lambda<Func<LambdaExpression>>(Expression.Call(value, factoryMethod)).Compile()();
                }
            });
        }

        readonly Lazy<Func<Expression, LambdaExpression>> factory;

        public LambdaExpression Replacement(Expression value)
        {
            if (factory.Value == null)
                return null;

            return factory.Value(value);
        }

        public static InjectLambdaMetadata Create(MethodInfo call)
        {
            if (call == null)
                throw new ArgumentNullException(nameof(call));

            // inject by convention
            var target = call.DeclaringType;
            var method = call.Name;
            var config = false;

            // configuration over convention, if any
            var metadata = call.GetAttribute<InjectLambdaAttribute>();
            if (metadata != null)
            {
                if (metadata.Target != null)
                    target = metadata.Target;
                if (!string.IsNullOrEmpty(metadata.Method))
                    method = metadata.Method;
                config = true;
            }

            // retrieve method's signature
            var returns = call.ReturnParameter.ParameterType;
            var args = call.GetParameters().Select(p => p.ParameterType).ToArray();

            return new InjectLambdaMetadata(target, method, config, returns, args);
        }
    }
}
