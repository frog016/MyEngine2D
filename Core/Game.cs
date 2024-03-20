using MyEngine2D.Core.Input;
using MyEngine2D.Core.Level;

namespace MyEngine2D.Core;

public sealed class Game
{
    public CancellationToken StoppedCancellationToken => _stopLoopSource.Token;

    private readonly Time _time;
    private readonly GameLevelManager _levelManager;
    private readonly InputSystem _inputSystem;
    private readonly CancellationTokenSource _stopLoopSource;

    internal Game(Time time, GameLevelManager levelManager, InputSystem inputSystem)
    {
        _time = time;
        _levelManager = levelManager;
        _inputSystem = inputSystem;

        _stopLoopSource = new CancellationTokenSource();
    }

    public void Run()
    {
        _time.Initialize();

        foreach (var gameObject in _levelManager.CurrentLevel.GameObjects)
        {
            gameObject.Start();
        }

        while (_stopLoopSource.IsCancellationRequested == false)
        {
            _time.Tick();

            HandleInput();
            Update(_time.DeltaTime);
            FixedUpdate();
            //Render();
        }
    }

    public void Stop()
    {
        _stopLoopSource.Cancel();
        ServiceLocator.Instance.Dispose();
    }

    private void HandleInput()
    {
        _inputSystem.UpdateInput();
    }

    private void Update(double deltaTime)
    {
        foreach (var gameObject in _levelManager.CurrentLevel.GameObjects)
        {
            gameObject.UpdateObject((float)deltaTime);
        }
    }

    private void FixedUpdate()
    {
        while (_time.NeedToCatchUpLag())
        {
            foreach (var gameObject in _levelManager.CurrentLevel.GameObjects)
            {
                gameObject.FixedUpdateObject(Time.FixedUpdateTimestamp);
            }

            _time.CatchUpLag();
        }
    }
}