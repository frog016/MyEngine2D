using MyEngine2D.Core.Level;

namespace MyEngine2D.Core;

public sealed class Game
{
    public readonly Time Time;
    public readonly GameLevelManager LevelManager;
    public CancellationToken StoppedCancellationToken => _stopLoopSource.Token;

    private readonly CancellationTokenSource _stopLoopSource;

    internal Game(Time time, GameLevelManager levelManager)
    {
        Time = time;
        LevelManager = levelManager;

        _stopLoopSource = new CancellationTokenSource();
    }

    public void Start()
    {
        Time.Initialize();
        while (_stopLoopSource.IsCancellationRequested == false)
        {
            Time.Tick();

            //ProcessInput();
            Update(Time.DeltaTime);
            FixedUpdate();
            //Render();
        }
    }

    public void Stop()
    {
        _stopLoopSource.Cancel();
    }

    private void Update(double deltaTime)
    {
        foreach (var gameObject in LevelManager.CurrentLevel.GameObjects)
            gameObject.UpdateObject((float)deltaTime);
    }

    private void FixedUpdate()
    {
        while (Time.LagFixedTime >= Time.FixedUpdateTimestamp)
        {
            foreach (var gameObject in LevelManager.CurrentLevel.GameObjects)
            {
                gameObject.FixedUpdateObject(Time.FixedUpdateTimestamp);
            }

            Time.CatchUpLag();
        }
    }
}