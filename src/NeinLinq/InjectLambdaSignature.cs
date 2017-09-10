using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NeinLinq
{
    sealed class InjectLambdaSignature
    {
        readonly Type[] genericArguments;

        readonly Type[] parameterTypes;

        readonly Type returnType;

        readonly bool isStatic;

        public InjectLambdaSignature(MethodInfo method)
        {
            if (method.IsGenericMethod)
            {
                genericArguments = method.GetGenericArguments();
            }

            parameterTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();
            returnType = method.ReturnParameter.ParameterType;

            isStatic = method.IsStatic;
        }

        public InjectLambdaSignature(PropertyInfo property)
        {
            parameterTypes = new[] { property.DeclaringType };
            returnType = property.PropertyType;

            isStatic = true;
        }

        public MethodInfo FindMatch(Type target, string method)
        {
            if (genericArguments == null)
            {
                // use build-in mechanism for non-generic stuff
                return target.GetRuntimeMethod(method, parameterTypes);
            }

            foreach (var candidate in target.GetRuntimeMethods())
            {
                // non-matching name?
                if (candidate.Name != method)
                    continue;

                // non-matching generic argument count?
                var candidateArguments = candidate.GetGenericArguments();
                if (candidateArguments.Length != genericArguments.Length)
                    continue;

                // non-matching parameter count?
                var candidateParameters = candidate.GetParameters();
                if (candidateParameters.Length != parameterTypes.Length)
                    continue;

                var index = 0;
                for (; index < parameterTypes.Length; index++)
                {
                    var type = candidateParameters[index].ParameterType;
                    if (type.IsGenericParameter)
                        continue;
                    if (type != parameterTypes[index])
                        break;
                }

                // non-matching parameter type?
                if (index != parameterTypes.Length)
                    continue;

                return candidate;
            }

            throw FailMatch(target, method, "evil treachery detected");
        }

        static Exception FailMatch(Type target, string method, string error)
        {
            throw new InvalidOperationException($"Unable to retrieve lambda meta-data from {target.FullName}.{method}: {error}.");
        }

        static readonly Type[] emptyTypes = new Type[0];

        public MethodInfo FindFactory(Type target, string method)
        {
            // assume method without any parameters
            var factory = target.GetRuntimeMethod(method, emptyTypes) ?? target.GetRuntimeProperty(method)?.GetMethod;
            if (factory == null)
                throw FailFactory(target, method, "no parameterless member found");

            // apply type arguments, if any
            if (genericArguments != null)
            {
                if (!factory.IsGenericMethodDefinition)
                    throw FailFactory(target, method, "generic implementation expected");
                factory = factory.MakeGenericMethod(genericArguments);
            }

            // mixed static and non-static methods?
            if (isStatic && !factory.IsStatic)
                throw FailFactory(target, method, "static implementation expected");
            if (!isStatic && factory.IsStatic)
                throw FailFactory(target, method, "non-static implementation expected");

            // method returns lambda expression?
            var returns = factory.ReturnType;
            if (!returns.IsConstructedGenericType || returns.GetGenericTypeDefinition() != typeof(Expression<>))
                throw FailFactory(target, method, "returns no lambda expression");

            // lambda signature matches original method's signature?
            var signature = returns.GenericTypeArguments[0].GetRuntimeMethod("Invoke", parameterTypes);
            if (signature == null || signature.ReturnParameter.ParameterType != returnType)
                throw FailFactory(target, method, "returns non-matching expression");

            return factory;
        }

        static Exception FailFactory(Type target, string method, string error)
        {
            throw new InvalidOperationException($"Unable to retrieve lambda expression from {target.FullName}.{method}: {error}.");
        }
    }
}
