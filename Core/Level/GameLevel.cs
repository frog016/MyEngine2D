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

    public GameObject Instantiate(string name, Vector2 position = default, float rotation = default, params Action<GameObject>[] addComponentActions)
    {
        var gameObject = new GameObject(name, position, rotation);
        foreach (var component in addComponentActions)
        {
            component.Invoke(gameObject);
        }

        _gameObjects.Add(gameObject);
        return gameObject;
    }

    internal void Destroy(GameObject gameObject)
    {
        if (gameObject.DestroyRequest == false)
        {
            throw new InvalidOperationException($"Can't destroy object: {gameObject}. Call {nameof(GameObject.Destroy)} before.");
        }

        _gameObjects.Remove(gameObject);
        gameObject.Dispose();
    }
}