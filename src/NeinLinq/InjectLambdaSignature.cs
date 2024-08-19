using System.Reflection;

namespace NeinLinq;

internal sealed class InjectLambdaSignature
{
    private readonly Type[] genericArguments;

    private readonly Type[] parameterTypes;

    private readonly Type returnType;

    private readonly bool isStatic;

    public InjectLambdaSignature(MethodInfo method)
    {
        if (method.DeclaringType is null)
            throw new InvalidOperationException($"Method {method.Name} has no declaring type.");

        genericArguments = method.GetGenericArguments();
        parameterTypes = method.GetParameters().Select(p => p.ParameterType).ToArray();
        returnType = method.ReturnParameter.ParameterType;
        isStatic = method.IsStatic;
    }

    public InjectLambdaSignature(PropertyInfo property)
    {
        if (property.DeclaringType is null)
            throw new InvalidOperationException($"Property {property.Name} has no declaring type.");

        genericArguments = Type.EmptyTypes;
        parameterTypes = [property.DeclaringType];
        returnType = property.PropertyType;
        isStatic = true;
    }

    public MethodInfo FindFactory(Type target, string method, Type? injectedType = null)
    {
        var factory = FindMatchWithoutParameters(target, method, genericArguments, injectedType);

        // okay, no more checks here...
        if (factory is null)
            throw FailFactory(target, method, "no matching parameterless member found");

        // apply type arguments, if any
        if (factory.IsGenericMethodDefinition)
            factory = factory.MakeGenericMethod(genericArguments);

        // mixed static and non-static methods?
        if (!factory.IsStatic && isStatic)
            throw FailFactory(target, factory.Name, "static implementation expected");

        // method returns lambda expression?
        if (!IsLambdaExpression(factory.ReturnType))
            throw FailFactory(target, factory.Name, "returns no lambda expression");

        // lambda signature matches original method's signature?
        if (!IsMatchingDelegate(factory.ReturnType))
            throw FailFactory(target, factory.Name, "returns non-matching expression");

        return factory;
    }

    private bool IsMatchingDelegate(Type type)
        => Array.Find(type.GetGenericArguments(), typeof(Delegate).IsAssignableFrom)?
                .GetMethod("Invoke", parameterTypes)?
                .ReturnParameter
                .ParameterType == returnType;

    private static bool IsLambdaExpression(Type type)
        => typeof(LambdaExpression).IsAssignableFrom(type)
            || type.GetMethods(BindingFlags.Public | BindingFlags.Static)
                   .Any(method => typeof(LambdaExpression).IsEquivalentTo(method.ReturnType)
                               && (method.Name == "op_Implicit" || method.Name == "op_Explicit"));

    private static Exception FailFactory(Type target, string method, string error)
    {
        if (method.StartsWith("get_", StringComparison.Ordinal))
            method = method.Substring(4);

        throw new InvalidOperationException($"Unable to retrieve lambda expression from {target.FullName}.{method}: {error}.");
    }

    public MethodInfo? FindMatch(Type target, string method, Type? injectedType = null)
        => FindMatch(target, method, genericArguments, parameterTypes, injectedType);

    private const BindingFlags Everything
        = BindingFlags.NonPublic
        | BindingFlags.Public
        | BindingFlags.Static
        | BindingFlags.Instance;

    private static MethodInfo? FindMatchWithoutParameters(Type target, string method, Type[] genericArguments, Type? injectedType = null)
    {
        var factory = FindMatch(target, method, genericArguments, Type.EmptyTypes, injectedType);
        if (genericArguments.Length == 0)
        {
            factory ??= target.GetProperty(method, Everything)?.GetGetMethod(true)
                ?? target.GetProperty(method + "Expr", Everything)?.GetGetMethod(true);
        }
        return factory;
    }

    private static MethodInfo? FindMatch(Type target, string method, Type[] genericArguments, Type[] parameterTypes, Type? injectedType = null)
    {
        MethodInfo? result = null;

        foreach (var candidate in target.GetMethods(Everything))
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
                var candidateParameterType = candidateParameters[index].ParameterType;
                if (candidateParameterType.IsGenericParameter)
                    continue;
                if (candidateParameterType != parameterTypes[index])
                    break;
            }

            // non-matching parameter type?
            if (index != parameterTypes.Length)
                continue;

            // There can only be one!
            if (result is not null)
            {
                // base definition is virtual
                if (candidate.IsVirtual)
                    continue;
                // base definition is non-virtual, but hiding is allowed anyway
                if (injectedType is null || result.DeclaringType!.IsAssignableFrom(injectedType))
                    continue;
            }

            result = candidate;
        }

        return result is null && target.BaseType is { } baseTarget
            ? FindMatch(baseTarget, method, genericArguments, parameterTypes, injectedType)
            : result;
    }
}
