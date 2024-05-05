using MyEngine2D.Core.Resource;

namespace MyEngine2D.Core.Graphic;

public readonly struct AnimationFrameLoadData
{
    public readonly SpriteLoadData SpriteLoadData;
    public readonly float FrameTime;

    public AnimationFrameLoadData(SpriteLoadData spriteLoadData, float frameTime)
    {
        FrameTime = frameTime;
        SpriteLoadData = spriteLoadData;
    }
}