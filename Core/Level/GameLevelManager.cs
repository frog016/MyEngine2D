namespace MyEngine2D.Core.Level;

public sealed class GameLevelManager
{
    public GameLevel CurrentLevel { get; private set; }

    private readonly Dictionary<string, GameLevel> _levels;

    public GameLevelManager(IEnumerable<GameLevel> levels)
    {
        _levels = levels.ToDictionary(key => key.Name, value => value);
        CurrentLevel = _levels.First().Value;
    }

    public void LoadLevel(string name)
    {
        if (_levels.TryGetValue(name, out var level) == false)
            throw new ArgumentException($"Level: {name} not found.");

        CurrentLevel = level;
    }
}