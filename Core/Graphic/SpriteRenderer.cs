using MyEngine2D.Core.Entity;
using MyEngine2D.Core.Utility;
using SharpDX;
using SharpDX.Mathematics.Interop;
using DX2D1 = SharpDX.Direct2D1;
using Color = System.Drawing.Color;
using Vector2 = MyEngine2D.Core.Structure.Vector2;

namespace MyEngine2D.Core.Graphic;

public sealed class SpriteRenderer : Component
{
    public Sprite? Sprite { get; private set; }
    public Vector2 Scale { get; set; }
    public Vector2 Offset { get; set; }
    public Color Color { get; set; }

    private DX2D1.RenderTarget _renderTarget;

    public SpriteRenderer(GameObject gameObject) : base(gameObject)
    {
        Scale = Vector2.One;
        Offset = Vector2.Zero;
        Color = Color.White;
    }

    public void Initialize(Sprite sprite, Vector2? scale = null, Vector2? offset = null, Color? color = null)
    {
        Sprite = sprite;
        Scale = scale ?? Scale;
        Offset = offset ?? Offset;
        Color = color ?? Color;
    }

    public void SetSprite(Sprite sprite)
    {
        Sprite?.Dispose();
        Sprite = sprite;
        Sprite.InitializeSprite(_renderTarget);
    }

    internal void InitializeRender(DX2D1.RenderTarget renderTarget)
    {
        _renderTarget = renderTarget;
        Sprite?.InitializeSprite(renderTarget);
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
        renderTarget.DrawBitmap(Sprite.Bitmap, destinationRectangle, 1f, DX2D1.BitmapInterpolationMode.Linear);
        renderTarget.Transform = transformationMatrix;
    }

    private RawRectangleF GetRenderTargetRectangle()
    {
        var center = GameObject.Transform.Position + Offset;
        var halfSize = Sprite.Size / 2f;

        var leftBottom = Vector2.MultiplyComponentwise(center - halfSize, Scale);
        var rightTop = Vector2.MultiplyComponentwise(center + halfSize, Scale);

        return new RawRectangleF(leftBottom.X, rightTop.Y, rightTop.X, leftBottom.Y);
    }

    public override void OnDestroy()
    {
        Sprite?.Dispose();
    }
}