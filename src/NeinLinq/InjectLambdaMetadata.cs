using System.Reflection;

namespace NeinLinq;

internal sealed class InjectLambdaMetadata
{
    public bool Config { get; }

    private readonly Lazy<Func<Expression?, LambdaExpression?>> lambda;

    public LambdaExpression? Lambda(Expression? value)
        => lambda.Value(value);

    private InjectLambdaMetadata(bool config, Lazy<Func<Expression?, LambdaExpression?>> lambda)
    {
        Config = config;

        this.lambda = lambda;
    }

    public static InjectLambdaMetadata Create(MethodInfo method)
    {
        var metadata = InjectLambdaAttribute.Provider(method);

        var lambdaFactory = new Lazy<Func<Expression?, LambdaExpression?>>(()
            => LambdaFactory(method, metadata ?? InjectLambdaAttribute.None));

        return new InjectLambdaMetadata(metadata is not null, lambdaFactory);
    }

    public static InjectLambdaMetadata Create(PropertyInfo property)
    {
        var metadata = InjectLambdaAttribute.Provider(property)
            ?? InjectLambdaAttribute.Provider(property.GetGetMethod(true)!);

        var lambdaFactory = new Lazy<Func<Expression?, LambdaExpression?>>(()
            => LambdaFactory(property, metadata ?? InjectLambdaAttribute.None));

        return new InjectLambdaMetadata(metadata is not null, lambdaFactory);
    }

    private static Func<Expression?, LambdaExpression?> LambdaFactory(MethodInfo method, InjectLambdaAttribute metadata)
    {
        if (method.DeclaringType is null)
            throw new InvalidOperationException($"Method {method.Name} has no declaring type.");

        // retrieve method's signature
        var signature = new InjectLambdaSignature(method);

        // special ultra-fast treatment for static methods and sealed classes
        if (method.IsStatic || method.DeclaringType.IsSealed)
        {
            return FixedLambdaFactory(metadata.Target ?? method.DeclaringType, metadata.Method ?? method.Name, signature);
        }

        // dynamic but not that fast treatment for other stuff
        return DynamicLambdaFactory(method, signature);
    }

    private static Func<Expression?, LambdaExpression?> LambdaFactory(PropertyInfo property, InjectLambdaAttribute metadata)
    {
        if (property.DeclaringType is null)
            throw new InvalidOperationException($"Property {property.Name} has no declaring type.");

        // retrieve method's signature
        var signature = new InjectLambdaSignature(property);

        // apply "Expr" convention for property "overloading"
        var method = metadata.Target is null ? property.Name + "Expr" : property.Name;

        // special treatment for super-heroic property getters
        return FixedLambdaFactory(metadata.Target ?? property.DeclaringType, metadata.Method ?? method, signature);
    }

    private static Func<Expression?, LambdaExpression?> FixedLambdaFactory(Type target, string method, InjectLambdaSignature signature)
    {
        // retrieve validated factory method once
        var factory = signature.FindFactory(target, method);

        if (factory.IsStatic)
        {
            // compile factory call for performance reasons :-)
            return Expression.Lambda<Func<Expression?, LambdaExpression>>(
                Expression.Convert(Expression.Call(factory), typeof(LambdaExpression)),
                Expression.Parameter(typeof(Expression))).Compile();
        }

        // call actual target object, compiles every time during execution... :-|
        return value => Expression.Lambda<Func<LambdaExpression>>(
            Expression.Convert(Expression.Call(value, factory), typeof(LambdaExpression))).Compile()();
    }

    private static Func<Expression?, LambdaExpression?> DynamicLambdaFactory(MethodInfo method, InjectLambdaSignature signature) => value =>
    {
        // retrieve actual target object, compiles every time and needs reflection too... :-(
        var targetObject = Expression.Lambda<Func<object>>(Expression.Convert(value!, typeof(object))).Compile()();

        // retrieve actual target type at runtime, whatever it may be
        var targetType = targetObject.GetType();

        // actual method may provide different information
        var concreteMethod = signature.FindMatch(targetType, method.Name, value!.Type)!;

        // configuration over convention, if any
        var metadata = InjectLambdaAttribute.Provider(concreteMethod) ?? InjectLambdaAttribute.None;

        // retrieve validated factory method
        var factoryMethod = signature.FindFactory(targetType, metadata.Method ?? method.Name, value.Type);

        var callExpression = factoryMethod.IsStatic
            ? Expression.Call(factoryMethod)
            : Expression.Call(Expression.Convert(value, targetType), factoryMethod);

        // finally call lambda factory *uff*
        return Expression.Lambda<Func<LambdaExpression>>(Expression.Convert(callExpression, typeof(LambdaExpression))).Compile()();
    };
}
