namespace MyEngine2D.Core.Utility;

public static class ReflectionExtensions
{
    public static IEnumerable<Type> GetAllChild(this Type type)
    {
        return type.Assembly
            .GetTypes()
            .Where(assemblyType => type.IsAssignableFrom(assemblyType) && assemblyType is
            {
                IsClass: true, 
                IsAbstract: false
            });
    }
}