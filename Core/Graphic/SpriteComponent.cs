using MyEngine2D.Core.Entity;
using MyEngine2D.Core.Resource;

namespace MyEngine2D.Core.Graphic;

public abstract class SpriteComponent : Component
{
    protected readonly ResourceManager ResourceManager;
    protected readonly GraphicRender GraphicRender;

    protected SpriteComponent(GameObject gameObject, ResourceManager resourceManager, GraphicRender graphicRender) : base(gameObject)
    {
        ResourceManager = resourceManager;
        GraphicRender = graphicRender;
    }

    protected Sprite LoadSpriteFrom(SpriteLoadData spriteData)
    {
        var sprite = ResourceManager.LoadResource<Sprite>(spriteData.FileRelativePath);
        sprite.InitializeInternal(GraphicRender.RenderTarget, spriteData.PixelsPerUnit);

        return sprite;
    }
}