using System.Reflection;

namespace MyEngine2D.Core.Utility;

internal static class ReflectionUtility
{
    private static readonly Dictionary<Type, FieldInfo[]> FieldsMap = new();
    private static readonly Dictionary<Type, PropertyInfo[]> PropertiesMap = new();
    private static readonly Dictionary<Type, MethodInfo[]> MethodsMap = new();
    private static readonly Dictionary<MethodBase, ParameterInfo[]> ParametersMap = new();

    public const BindingFlags BindingAttributes = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    public static FieldInfo[] GetFieldsCashed(Type type)
    {
        if (FieldsMap.TryGetValue(type, out var fields))
            return fields;

        fields = type.GetFields(BindingAttributes);
        FieldsMap.Add(type, fields);

        return fields;
    }

    public static PropertyInfo[] GetPropertiesCashed(Type type)
    {
        if (PropertiesMap.TryGetValue(type, out var properties))
            return properties;

        properties = type.GetProperties(BindingAttributes);
        PropertiesMap.Add(type, properties);

        return properties;
    }

    public static MethodInfo[] GetMethodsCashed(Type type)
    {
        if (MethodsMap.TryGetValue(type, out var methods))
            return methods;

        methods = type.GetMethods(BindingAttributes);
        MethodsMap.Add(type, methods);

        return methods;
    }

    public static ParameterInfo[] GetParametersCached(MethodBase method)
    {
        if (ParametersMap.TryGetValue(method, out var parameters))
            return parameters;

        parameters = method.GetParameters();
        ParametersMap.Add(method, parameters);

        return parameters;
    }
}