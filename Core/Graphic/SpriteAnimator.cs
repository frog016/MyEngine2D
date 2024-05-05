using MyEngine2D.Core.Entity;
using MyEngine2D.Core.Resource;

namespace MyEngine2D.Core.Graphic;

public sealed class SpriteAnimator : SpriteComponent
{
    private readonly Dictionary<string, Animation> _animations;

    private Animation? _currentAnimation;
    private SpriteRenderer _spriteRenderer;

    public SpriteAnimator(GameObject gameObject, ResourceManager resourceManager, GraphicRender graphicRender) 
        : base(gameObject, resourceManager, graphicRender)
    {
        _animations = new Dictionary<string, Animation>();
        _currentAnimation = null;
    }

    public void Initialize(AnimationLoadData[] animationsData)
    {
        foreach (var animationLoadData in animationsData)
        {
            _animations[animationLoadData.Name] = LoadAnimationFrom(animationLoadData);
        }

        _spriteRenderer = GameObject.GetComponent<SpriteRenderer>();
    }

    public void PlayAnimation(string animationName)
    {
        _currentAnimation?.Stop();
        _currentAnimation = _animations[animationName];
        _currentAnimation.Play(_spriteRenderer);
    }

    public override void Update(float deltaTime)
    {
        _currentAnimation?.Update(deltaTime);
    }

    public override void OnDestroy()
    {
        foreach (var animation in _animations.Values)
        {
            animation.Dispose();
        }
    }

    private Animation LoadAnimationFrom(AnimationLoadData animationLoadData)
    {
        var animationFrames = animationLoadData.AnimationFrames
            .Select(frame => new AnimationFrame(LoadSpriteFrom(frame.SpriteLoadData), frame.FrameTime))
            .ToArray();

        return new Animation(animationFrames, animationLoadData.Speed, animationLoadData.Looped);
    }
}