using MyEngine2D.Core;

namespace MyEngine2D;

public abstract class OverviewPreset
{
    public void Start()
    {
        using var game = CreateOverviewGame();
        game.Run();
    }

    protected abstract Game CreateOverviewGame();
}