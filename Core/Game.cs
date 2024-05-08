using MyEngine2D.Core.Entity;
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

    private bool _runned;

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
        _levelManager.CurrentLevel.GameObjects.Added -= StartGameObjectDelayed;

        _runned = false;
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

        _levelManager.CurrentLevel.GameObjects.Added += StartGameObjectDelayed;
    }

    private void RunLoop()
    {
        _runned = true;
        while (_stopLoopSource.IsCancellationRequested == false)
        {
            _time.Tick();

            HandleInput();
            Update();
            FixedUpdate();
            Render();

            HandleDestroyRequest();
        }
    }

    private void HandleInput()
    {
        _inputSystem.UpdateInput();
    }

    private void Update()
    {
        var deltaTime = _time.DeltaTime;

        foreach (var gameObject in _levelManager.CurrentLevel.GameObjects)
        {
            gameObject.UpdateObject(deltaTime);
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

    private void HandleDestroyRequest()
    {
        var gameObjects = _levelManager.CurrentLevel.GameObjects.ToArray();
        foreach (var gameObject in gameObjects)
        {
            if (gameObject.DestroyRequest)
            {
                _levelManager.CurrentLevel.Destroy(gameObject);
            }
        }
    }

    private void StartGameObjectDelayed(GameObject gameObject)
    {
        if (_runned)
        {
            gameObject.Start();
        }
    }
}