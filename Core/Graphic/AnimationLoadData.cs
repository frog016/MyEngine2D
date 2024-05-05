namespace MyEngine2D.Core.Graphic;

public readonly struct AnimationLoadData
{
    public readonly string Name;
    public readonly float Speed;
    public readonly bool Looped;
    public readonly AnimationFrameLoadData[] AnimationFrames;

    public AnimationLoadData(string name, float speed, bool looped, AnimationFrameLoadData[] animationFrames)
    {
        Name = name;
        AnimationFrames = animationFrames;
        Speed = speed;
        Looped = looped;
    }
}