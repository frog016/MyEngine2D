using MyEngine2D.Core.Entity;

namespace MyEngine2D.Core.Factory
{
    public class ComponentFactory
    {
        public static T CreateComponent<T>(GameObject context) where T : Component
        {
            var componentType = typeof(T);
            var component = Activator.CreateInstance(componentType, context) as T;

            return component ?? throw new InvalidOperationException($"Can't create {nameof(Component)} of type {componentType}.");
        }
    }
}
