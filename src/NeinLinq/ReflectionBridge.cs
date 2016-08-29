using System;
using System.Reflection;

namespace NeinLinq
{
    static class ReflectionBridge
    {

#if NET40

        public static Type GetTypeInfo(this Type type)
        {
            return type;
        }

        public static T GetCustomAttribute<T>(this MemberInfo element)
            where T : Attribute
        {
            return (T)Attribute.GetCustomAttribute(element, typeof(T));
        }

        public static MethodInfo GetRuntimeMethod(this Type type, string name, Type[] parameters)
        {
            return type.GetMethod(name, parameters);
        }

        public static PropertyInfo GetRuntimeProperty(this Type type, string name)
        {
            return type.GetProperty(name);
        }

        public static MethodInfo GetMethod(this PropertyInfo element)
        {
            return element.GetGetMethod(true);
        }

        public static MethodInfo SetMethod(this PropertyInfo element)
        {
            return element.GetSetMethod(true);
        }

        public static bool IsConstructedGenericType(this Type type)
        {
            return type.IsGenericType && !type.IsGenericTypeDefinition;
        }

        public static Type[] GenericTypeArguments(this Type type)
        {
            return type.GetGenericArguments();
        }

#else

        public static MethodInfo GetMethod(this PropertyInfo element)
        {
            return element.GetMethod;
        }

        public static MethodInfo SetMethod(this PropertyInfo element)
        {
            return element.SetMethod;
        }

        public static bool IsConstructedGenericType(this Type type)
        {
            return type.IsConstructedGenericType;
        }

        public static Type[] GenericTypeArguments(this Type type)
        {
            return type.GenericTypeArguments;
        }

#endif

    }
}
