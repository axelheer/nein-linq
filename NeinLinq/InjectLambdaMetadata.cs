using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NeinLinq
{
    internal sealed class InjectLambdaMetadata
    {
        private readonly Type target;

        public Type Target
        {
            get { return target; }
        }

        private readonly string method;

        public string Method
        {
            get { return method; }
        }

        private readonly bool config;

        public bool Config
        {
            get { return config; }
        }

        private InjectLambdaMetadata(Type target, string method, bool config, Type returns, params Type[] args)
        {
            this.target = target;
            this.method = method;
            this.config = config;

            factory = new Lazy<Func<LambdaExpression>>(() =>
            {
                // assume method without any parameters
                var factoryMethod = target.GetMethod(method, new Type[0]);
                if (factoryMethod == null)
                    return null;

                // method returns lambda expression?
                var expressionInfo = factoryMethod.ReturnType;
                if (expressionInfo.IsSubclassOf(typeof(LambdaExpression)) == false)
                    return null;

                // lambda signature matches original method's signature?
                var delegateInfo = expressionInfo.GetGenericArguments()[0];
                var delegateSignature = delegateInfo.GetMethod("Invoke", args);
                if (delegateSignature == null || delegateSignature.ReturnParameter.ParameterType != returns)
                    return null;

                // compile factory call for performance reasons
                return Expression.Lambda<Func<LambdaExpression>>(Expression.Call(factoryMethod)).Compile();
            });
        }

        private readonly Lazy<Func<LambdaExpression>> factory;

        public Func<LambdaExpression> CreateFactory()
        {
            return factory.Value;
        }

        public static InjectLambdaMetadata Create(MethodInfo call)
        {
            if (call == null)
                throw new ArgumentNullException("call");

            // inject by convention
            var target = call.DeclaringType;
            var method = call.Name;
            var config = false;

            // configuration over convention, if any
            var metadata = (InjectLambdaAttribute)Attribute.GetCustomAttribute(call, typeof(InjectLambdaAttribute));
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
