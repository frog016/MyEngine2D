using MyEngine2D.Core.Graphic;
using MyEngine2D.Core.Input;
using MyEngine2D.Core.Level;
using MyEngine2D.Core.Physic;

namespace MyEngine2D.Core;

public sealed class Game : IDisposable
{
    public CancellationToken StoppedCancellationToken => _stopLoopSource.Token;

    private readonly Time _time;
    private readonly GameLevelManager _levelManager;
    private readonly InputSystem _inputSystem;
    private readonly PhysicWorld _physicWorld;
    private readonly GraphicRender _graphicRender;

    private readonly CancellationTokenSource _stopLoopSource;

    internal Game(
        Time time, GameLevelManager levelManager, InputSystem inputSystem,
        PhysicWorld physicWorld, GraphicRender graphicRender)
    {
        _time = time;
        _levelManager = levelManager;
        _inputSystem = inputSystem;
        _physicWorld = physicWorld;
        _graphicRender = graphicRender;

        _stopLoopSource = new CancellationTokenSource();
    }

    public void Run()
    {
        InitializeGame();
        RunLoop();
    }

    public void Dispose()   //  TODO: Добавить вызов Dispose
    {
        _stopLoopSource.Cancel();
        ServiceLocator.Instance.Dispose();
    }

    private void InitializeGame()
    {
        _time.Initialize();
        _graphicRender.Run();

        foreach (var gameObject in _levelManager.CurrentLevel.GameObjects)
        {
            gameObject.Start();
        }
    }

    private void RunLoop()
    {
        while (_stopLoopSource.IsCancellationRequested == false)
        {
            _time.Tick();

            HandleInput();
            Update(_time.DeltaTime);
            FixedUpdate();
            Render();
        }
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

    private void Render()
    {
        _graphicRender.Render();
    }
}