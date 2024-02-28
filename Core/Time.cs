namespace MyEngine2D.Core;

public sealed class Time
{
    public float CurrentTime { get; private set; }
    public float DeltaTime { get; private set; }

    internal float LagFixedTime { get; private set; }

    private DateTime _startTime;
    private float _previousTickTime;

    public const float FixedUpdateTimestamp = 1 / 30f;

    internal void Initialize()
    {
        _startTime = DateTime.Now;
    }

    internal void Tick()
    {
        CurrentTime = GetCurrentTime();
        DeltaTime = CurrentTime - _previousTickTime;

        LagFixedTime += DeltaTime;

        _previousTickTime = CurrentTime;
    }

    internal void CatchUpLag()
    {
        LagFixedTime -= FixedUpdateTimestamp;
    }

    private float GetCurrentTime()
    {
        return (float)DateTime.Now.Subtract(_startTime).TotalSeconds;
    }
}