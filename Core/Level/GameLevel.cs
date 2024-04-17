using MyEngine2D.Core.Entity;
using MyEngine2D.Core.Structure;

namespace MyEngine2D.Core.Level;

public sealed class GameLevel
{
    public readonly string Name;
    public IReadOnlyObservable<GameObject> GameObjects => _gameObjects;

    private readonly ObservableCollection<GameObject> _gameObjects = new();

    public GameLevel(string name)
    {
        Name = name;
    }

    public GameObject Instantiate(string name, Vector2 position = default, float rotation = default)
    {
        var gameEntity = new GameObject(name, position, rotation);
        _gameObjects.Add(gameEntity);

        return gameEntity;
    }

    public void Destroy(GameObject gameObject)
    {
        _gameObjects.Remove(gameObject);
        gameObject.Dispose();
    }
}