using MyEngine2D.Core.Math;

namespace MyEngine2D.Core.Graphic;

public sealed class Animation : IDisposable
{
    public float Speed { get; set; }
    public bool Looped { get; set; }
    public bool IsPlaying { get; private set; }

    private readonly AnimationFrame[] _frames;

    private int _currentFrameIndex;
    private float _currentFrameTime;
    private float _nextFrameTime;
    private float _maxFrameTime;

    private SpriteRenderer _spriteRenderer;

    public Animation(AnimationFrame[] frames, float speed, bool looped)
    {
        _frames = frames;
        Speed = speed;
        Looped = looped;

        _currentFrameIndex = -1;
        _currentFrameTime = 0;
        _nextFrameTime = 0;
        _maxFrameTime = 0;
    }

    public void Dispose()
    {
        Stop();

        foreach (var frame in _frames)
        {
            frame.Sprite.Dispose();
        }
    }

    internal void Play(SpriteRenderer renderer)
    {
        if (_frames.Length == 0)
        {
            return;
        }

        IsPlaying = true;
        _spriteRenderer = renderer;

        InitializeFirstFrame();
    }

    internal void Stop()
    {
        IsPlaying = false;
    }

    internal void Update(float deltaTime)
    {
        if (IsPlaying == false)
        {
            return;
        }

        if (_currentFrameTime >= _nextFrameTime)
        {
            PlayCurrentFrame();
        }

        _currentFrameTime = Math2D.Clamp(_currentFrameTime + Speed * deltaTime, 0, _maxFrameTime);
        if (Math2D.Abs(_currentFrameTime - _maxFrameTime) < 1e-8)
        {
            TryLoopAnimation();
        }
    }

    private void InitializeFirstFrame()
    {
        _currentFrameTime = 0;
        _currentFrameIndex = _frames[0].FrameTime == 0 ? 0 : -1;

        if (_currentFrameIndex == -1)
        {
            _nextFrameTime = GetNextFrameTime();
        }
        else
        {
            PlayCurrentFrame();
        }

        _maxFrameTime = _frames[^1].FrameTime;
    }

    private void PlayCurrentFrame()
    {
        _currentFrameIndex = GetNextFrameIndex();

        var currentFrame = _frames[_currentFrameIndex];
        _spriteRenderer.SetSprite(currentFrame.Sprite);

        _nextFrameTime = GetNextFrameTime();
    }

    private void TryLoopAnimation()
    {
        IsPlaying = Looped;

        if (Looped)
        {
            InitializeFirstFrame();
        }
    }

    private int GetNextFrameIndex()
    {
        return (_currentFrameIndex + 1) % _frames.Length;
    }

    private float GetNextFrameTime()
    {
        var nextFrameIndex = GetNextFrameIndex();
        return _frames[nextFrameIndex].FrameTime;
    }
}