using MyEngine2D.Core.Entity;
using MyEngine2D.Core.Structure;

namespace MyEngine2D.Core
{
    public class SceneLevel
    {
        public readonly string Name;
        public readonly List<GameObject> GameObjects = new();

        public SceneLevel(string name)
        {
            Name = name;
        }

        public GameObject Instantiate(string name, Vector2 position = default, float rotation = default)
        {
            var gameEntity = new GameObject(name, position, rotation);
            GameObjects.Add(gameEntity);

            return gameEntity;
        }

        public void Destroy(GameObject gameObject)
        {
            GameObjects.Remove(gameObject);
            gameObject.Dispose();
        }
    }
}
