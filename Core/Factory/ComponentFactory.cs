using System.Reflection;
using MyEngine2D.Core.Assertion;
using MyEngine2D.Core.Entity;
using MyEngine2D.Core.Utility;

namespace MyEngine2D.Core.Factory
{
    public class ComponentFactory
    {
        private static readonly Type GameObjectType = typeof(GameObject);

        public static T CreateComponent<T>(GameObject context) where T : Component
        {
            var componentType = typeof(T);
            var component = CreateInstanceWithReflection(componentType, context);

            return component as T ?? throw new InvalidOperationException($"Can't create {nameof(Component)} of type {componentType}.");
        }

        private static object CreateInstanceWithReflection(Type componentType, GameObject context)
        {
            const int requiredObjectParameterPosition = 1;

            var constructors = componentType.GetConstructors(ReflectionUtility.BindingAttributes);
            if (constructors.Length > 1)
                throw new MultipleConstructorException(componentType);

            var constructor = constructors[0];
            var constructorArguments = ReflectionUtility.GetParametersCached(constructor);

            if (constructorArguments.Length == 0)
                throw new RequiredParameterNotFoundException(GameObjectType);

            if (constructorArguments.Length > 1 && constructorArguments[0].ParameterType != GameObjectType)
                throw new PositionalRequiredParameterNotFoundException(GameObjectType, requiredObjectParameterPosition);

            return CreateComponentInstance(constructor, constructorArguments, context);
        }

        private static object CreateComponentInstance(ConstructorInfo constructor, ParameterInfo[] constructorArguments, GameObject context)
        {
            var componentDependencies = GetComponentConstructorParameterValues(constructorArguments.Skip(1));
            var componentConstructorArguments = new object[] { context }
                .Concat(componentDependencies)
                .ToArray();

            return constructor.Invoke(componentConstructorArguments);
        }

        private static object[] GetComponentConstructorParameterValues(IEnumerable<ParameterInfo> parameters)
        {
            return parameters
                .Select(parameterInfo => ServiceLocator.Instance.Get(parameterInfo.ParameterType))
                .ToArray();
        }
    }
}
