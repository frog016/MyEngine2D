namespace MyEngine2D.Core.Level;

public abstract class GameLevelConfigurator
{
    public Func<GameLevel> CreateLevel()
    {
        return CreateConfiguredLevel;
    }

    protected abstract GameLevel CreateConfiguredLevel();
}