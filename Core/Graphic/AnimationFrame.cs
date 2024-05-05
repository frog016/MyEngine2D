namespace MyEngine2D.Core.Graphic;

public readonly struct AnimationFrame
{
    public readonly Sprite Sprite;
    public readonly float FrameTime;

    public AnimationFrame(Sprite sprite, float frameTime)
    {
        Sprite = sprite;
        FrameTime = frameTime;
    }
}