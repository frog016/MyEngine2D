using MyEngine2D.Core.Entity;

namespace MyEngine2D.Core.Factory
{
    public class ComponentFactory
    {
        public static T CreateComponent<T>(GameObject context) where T : Component
        {
            var component = Activator.CreateInstance<T>();
            component.Initialize(context);
            component.Create();

            return component;
        }
    }
}
