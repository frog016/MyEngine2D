using MyEngine2D.Core.Entity;
using MyEngine2D.Core.Level;
using MyEngine2D.Core.Math;
using MyEngine2D.Core.Structure;
using MyEngine2D.Core.Utility;
using SharpDX;

namespace MyEngine2D.Core.Graphic;

public sealed class Camera : Component
{
    public float OrthoSize { get; set; }

    public const int PixelsPerUnitOrthoSize = 100;

    private readonly GraphicRender _graphicRender;

    public Camera(GameObject gameObject, GraphicRender graphicRender) : base(gameObject)
    {
        _graphicRender = graphicRender;
    }

    public void Initialize(float orthoSize)
    {
        OrthoSize = orthoSize;
    }

    public Matrix3x2 GetViewMatrix()
    {
        var screenSize = _graphicRender.ScreenSize;
        var scaleFactor = Math2D.Min(screenSize.X, screenSize.Y) / OrthoSize;

        return Matrix3x2.Scaling(scaleFactor) *
               Matrix3x2.Rotation(Transform.Rotation) *
               Matrix3x2.Translation((screenSize / 2f - scaleFactor * Transform.Position).ToDXVector2());
    }

    public AxisAlignedBoundingBox GetViewRectangle()
    {
        var center = Transform.Position;
        var halfSize = _graphicRender.ScreenSize / PixelsPerUnitOrthoSize / 2;

        return new AxisAlignedBoundingBox(center - halfSize, center + halfSize);
    }

    public static Camera CreateDefault(GameLevel level)
    {
        const string defaultName = "Camera";
        const float defaultZoom = 5f;

        var cameraObject = level.Instantiate(defaultName, Structure.Vector2.Zero);
        var camera = cameraObject.AddComponent<Camera>();
        camera.Initialize(defaultZoom);

        return camera;
    }
}