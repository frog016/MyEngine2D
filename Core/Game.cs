using MyEngine2D.Core.Input;
using MyEngine2D.Core.Level;
using MyEngine2D.Core.Physic;

namespace MyEngine2D.Core;

public sealed class Game
{
    public CancellationToken StoppedCancellationToken => _stopLoopSource.Token;

    private readonly Time _time;
    private readonly GameLevelManager _levelManager;
    private readonly InputSystem _inputSystem;
    private readonly PhysicWorld _physicWorld;

    private readonly CancellationTokenSource _stopLoopSource;

    internal Game(Time time, GameLevelManager levelManager, InputSystem inputSystem, PhysicWorld physicWorld)
    {
        _time = time;
        _levelManager = levelManager;
        _inputSystem = inputSystem;
        _physicWorld = physicWorld;

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
            _physicWorld.UpdatePhysic(Time.FixedUpdateTimestamp);

            foreach (var gameObject in _levelManager.CurrentLevel.GameObjects)
            {
                gameObject.FixedUpdateObject(Time.FixedUpdateTimestamp);
            }

            _time.CatchUpLag();
        }
    }
}