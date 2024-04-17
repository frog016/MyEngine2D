﻿using MyEngine2D.Core.Entity;
using MyEngine2D.Core.Math;
using MyEngine2D.Core.Resource;
using MyEngine2D.Core.Structure;
using MyEngine2D.Core.Utility;
using SharpDX;
using SharpDX.Mathematics.Interop;

using DX2D1 = SharpDX.Direct2D1;
using Vector2 = MyEngine2D.Core.Structure.Vector2;

namespace MyEngine2D.Core.Graphic;

public sealed class SpriteRenderer : Component
{
    public Sprite? Sprite { get; private set; }
    public Vector2 Scale { get; set; }
    public Vector2 Offset { get; set; }

    public Layer Layer
    {
        get => _layer;
        set
        {
            var oldLayer = _layer;
            _layer = value;
            LayerChanged.Invoke(this, oldLayer, _layer);
        }
    }

    public float Opacity
    {
        get => _opacity; 
        set => _opacity = Math2D.Clamp(value, 0, 1);
    }

    public Vector2 ScaledSpriteSize => Sprite == null
        ? Vector2.Zero
        : Vector2.MultiplyComponentwise(Scale, Sprite.Size);

    public event Action<SpriteRenderer, Layer, Layer> LayerChanged = delegate { }; 

    private readonly ResourceManager _resourceManager;
    private readonly DX2D1.RenderTarget _renderTarget;

    private Layer _layer;
    private float _opacity;

    public SpriteRenderer(GameObject gameObject, ResourceManager resourceManager, GraphicRender graphicRender) : base(gameObject)
    {
        _resourceManager = resourceManager;
        _renderTarget = graphicRender.RenderTarget;

        Scale = Vector2.One;
        Offset = Vector2.Zero;
        Layer = 0;
        Opacity = 1.0f;
    }

    public void Initialize(SpriteLoadData spriteData, Vector2? scale = null, Vector2? offset = null, Layer? layer = null, float? opacity = null)
    {
        Sprite = LoadSpriteFrom(spriteData);

        Scale = scale ?? Scale;
        Offset = offset ?? Offset;
        Layer = layer ?? Layer;
        Opacity = opacity ?? Opacity;
    }

    public void SetSprite(SpriteLoadData spriteData)
    {
        var sprite = LoadSpriteFrom(spriteData);

        Sprite?.Dispose();
        Sprite = sprite;
    }

    public override void OnDestroy()
    {
        Sprite?.Dispose();
    }

    internal void Render(DX2D1.RenderTarget renderTarget)
    {
        if (Sprite == null)
        {
            return;
        }

        var destinationRectangle = GetRenderTargetRectangle();

        var transformationMatrix = renderTarget.Transform;
        renderTarget.Transform = Matrix3x2.Rotation(GameObject.Transform.Rotation, GameObject.Transform.Position.ToDXVector2());
        renderTarget.DrawBitmap(Sprite.DirectBitmap, destinationRectangle, Opacity, DX2D1.BitmapInterpolationMode.Linear);
        renderTarget.Transform = transformationMatrix;
    }

    private RawRectangleF GetRenderTargetRectangle()
    {
        var center = GameObject.Transform.Position + Offset;
        var halfSize = ScaledSpriteSize / 2f;

        var leftBottom = center - halfSize;
        var rightTop = center + halfSize;

        return new RawRectangleF(leftBottom.X, rightTop.Y, rightTop.X, leftBottom.Y);
    }

    private Sprite LoadSpriteFrom(SpriteLoadData spriteData)
    {
        var sprite = _resourceManager.LoadResource<Sprite>(spriteData.FileRelativePath);
        sprite.InitializeInternal(_renderTarget, spriteData.PixelsPerUnit);

        return sprite;
    }
}