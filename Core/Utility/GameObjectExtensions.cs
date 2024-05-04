using MyEngine2D.Core.Entity;

namespace MyEngine2D.Core.Utility;

public static class GameObjectExtensions
{
    public static T FindByType<T>(this IEnumerable<GameObject> collection) where T : Component
    {
        foreach (var gameObject in collection)
        {
            if (gameObject.TryGetComponent<T>(out var component))
            {
                return component;
            }
        }

        throw new ArgumentException();
    }
}