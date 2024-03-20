using MyEngine2D.Core.Math;

namespace MyEngine2D.Core;

public sealed class Time
{
    public float CurrentTime { get; private set; }
    public float DeltaTime { get; private set; }

    private DateTime _startTime;
    private float _lagFixedTime;
    private float _previousTickTime;

    public const float FixedUpdateTimestamp = 1 / 30f;

    private const float MaxLagTime = FixedUpdateTimestamp * 10;

    internal void Initialize()
    {
        _startTime = DateTime.Now;
    }

    internal void Tick()
    {
        CurrentTime = GetCurrentTime();
        DeltaTime = CurrentTime - _previousTickTime;

        var accumulatedLag = _lagFixedTime + DeltaTime;
        _lagFixedTime = Math2D.Clamp(accumulatedLag, 0, MaxLagTime);

        _previousTickTime = CurrentTime;
    }

    internal bool NeedToCatchUpLag()
    {
        return _lagFixedTime >= FixedUpdateTimestamp;
    }

    internal void CatchUpLag()
    {
        _lagFixedTime -= FixedUpdateTimestamp;
    }

    private float GetCurrentTime()
    {
        return (float)DateTime.Now.Subtract(_startTime).TotalSeconds;
    }
}