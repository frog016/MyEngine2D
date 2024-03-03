using MyEngine2D.Core.Input;
using MyEngine2D.Core.Level;

namespace MyEngine2D.Core;

public sealed class Game
{
    public readonly Time Time;
    public readonly GameLevelManager LevelManager;
    public readonly InputSystem InputSystem;
    public CancellationToken StoppedCancellationToken => _stopLoopSource.Token;

    private readonly CancellationTokenSource _stopLoopSource;

    internal Game(Time time, GameLevelManager levelManager, InputSystem inputSystem)
    {
        Time = time;
        LevelManager = levelManager;
        InputSystem = inputSystem;

        _stopLoopSource = new CancellationTokenSource();
    }

    public void Run()
    {
        Time.Initialize();

        foreach (var gameObject in LevelManager.CurrentLevel.GameObjects)
            gameObject.Start();

        while (_stopLoopSource.IsCancellationRequested == false)
        {
            Time.Tick();

            HandleInput();
            Update(Time.DeltaTime);
            FixedUpdate();
            //Render();
        }
    }

    public void Stop()
    {
        _stopLoopSource.Cancel();
    }

    private void HandleInput()
    {
        InputSystem.UpdateInput();
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