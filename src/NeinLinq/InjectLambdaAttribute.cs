using System.Reflection;

namespace NeinLinq;

/// <summary>
/// Delegate for custom attribute providers.
/// </summary>
/// <param name="memberInfo">The MemberInfo to get the attribute for.</param>
/// <returns>The InjectLambdaAttribute if found, otherwise null.</returns>
public delegate InjectLambdaAttribute? InjectLambdaAttributeProvider(MemberInfo memberInfo);

/// <summary>
/// Marks a method as injectable.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false)]
public sealed class InjectLambdaAttribute : Attribute
{
    /// <summary>
    /// Gets the current attribute provider. Default is the standard reflection-based provider.
    /// </summary>
    public static InjectLambdaAttributeProvider Provider { get; private set; } = memberInfo
        => (InjectLambdaAttribute?)GetCustomAttribute(memberInfo, typeof(InjectLambdaAttribute));

    internal static InjectLambdaAttribute None { get; } = new InjectLambdaAttribute();

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
        if (target is null)
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
        if (target is null)
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

    /// <summary>
    /// Sets a custom attribute provider.
    /// </summary>
    /// <param name="provider">The custom attribute provider to set.</param>
    /// <exception cref="ArgumentNullException">Thrown if provider is null.</exception>
    public static void SetAttributeProvider(InjectLambdaAttributeProvider provider)
    {
        if (provider is null)
            throw new ArgumentNullException(nameof(provider));

        Provider = provider;
    }
}
