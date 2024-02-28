namespace MyEngine2D.Core.Utility;

internal static class ReflectionUtility
{
    public static IEnumerable<Type> GetAllChild(this Type type)
    {
        return type.Assembly
            .GetTypes()
            .Where(t => type.IsAssignableFrom(t) && t.IsClass && t.IsAbstract == false);
    }
}