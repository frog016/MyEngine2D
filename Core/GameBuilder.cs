using MyEngine2D.Core.Level;
using MyEngine2D.Core.Utility;

namespace MyEngine2D.Core;

public sealed class GameBuilder
{
    private readonly List<GameLevel> _levels = new();

    private static readonly Type[] LevelConfiguratorTypes;

    static GameBuilder()
    {
        var type = typeof(GameLevelConfigurator);
        LevelConfiguratorTypes = type
            .GetAllChild()
            .ToArray();
    }

    public GameBuilder WithConfiguredLevels()
    {
        var levelConfigurators = LevelConfiguratorTypes
            .Select(Activator.CreateInstance)
            .Cast<GameLevelConfigurator>();

        var levels = levelConfigurators
            .Select(configurator => configurator.CreateLevel());

        _levels.AddRange(levels);
        return this;
    }

    public GameBuilder WithCustomLevels(IEnumerable<GameLevel> levels)
    {
        _levels.AddRange(levels);
        return this;
    }

    public Game Build()
    {
        var time = new Time();

        var levels = _levels.ToArray();
        var levelLoader = new GameLevelManager(levels);

        Clear();
        return new Game(time, levelLoader);
    }

    private void Clear()
    {
        _levels.Clear();
    }
}