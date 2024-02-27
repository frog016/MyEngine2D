using MyEngine2D.Core.Helper;

namespace MyEngine2D.Core;

public sealed class Game : Singleton<Game>
{
    public SceneLevel CurrentLevel { get; private set; }

    private readonly List<SceneLevel> _levels = new();

    private CancellationTokenSource _stopLoopSource;

    private const double FixedUpdateTimestamp = 1 / 30d;

    public void Initialize(List<SceneLevel> levels)
    {
        CurrentLevel = levels[0];
        _levels.AddRange(levels);
    }

    public void Start()
    {
        _stopLoopSource = new CancellationTokenSource();

        var startTime = DateTime.Now;
        var previousTime = 0d;
        var accumulator = 0d;

        while (_stopLoopSource.IsCancellationRequested == false)
        {
            var currentTime = GetCurrentTime(startTime);
            var deltaTime = currentTime - previousTime;
            accumulator += deltaTime;

            //ProcessInput();
            Update(deltaTime);

            while (accumulator >= FixedUpdateTimestamp)
            {
                FixedUpdate();
                accumulator -= FixedUpdateTimestamp;
            }

            //Render();
            previousTime = currentTime;
        }
    }

    public void Stop()
    {
        _stopLoopSource.Cancel();
    }

    public void LoadLevel(string name)
    {
        CurrentLevel = _levels.First(level => level.Name == name);
    }

    private void Update(double deltaTime)
    {
        foreach (var gameObject in CurrentLevel.GameObjects)
            gameObject.UpdateObject((float)deltaTime);
    }

    private void FixedUpdate()
    {
        foreach (var gameObject in CurrentLevel.GameObjects)
            gameObject.FixedUpdateObject((float)FixedUpdateTimestamp);
    }

    private static double GetCurrentTime(DateTime startTime)
    {
        return DateTime.Now.Subtract(startTime).TotalSeconds;
    }
}