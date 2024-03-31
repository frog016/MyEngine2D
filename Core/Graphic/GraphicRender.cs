using MyEngine2D.Core.Level;
using MyEngine2D.Core.Physic;
using MyEngine2D.Core.Utility;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using SharpDX.Windows;
using System.Drawing;

using DX2D1 = SharpDX.Direct2D1;

namespace MyEngine2D.Core.Graphic;

internal sealed class GraphicRender : IDisposable
{
    private readonly GameLevelManager _levelManager;
    private readonly RenderForm _window;
    private readonly RenderLoop _renderLoop;
    private readonly RenderTarget _renderTarget;
    private readonly Brush _brush;

    private static readonly SharpDX.Color DefaultBackgroundColor = new(32, 103, 176);
    private static readonly SharpDX.Color DefaultShapeColor = new(30, 30, 30);

    internal GraphicRender(GameLevelManager levelManager, GraphicWindowDescription description)
    {
        _levelManager = levelManager;
        _window = new RenderForm(description.Header)
        {
            ClientSize = new Size(description.Width, description.Height),
            AllowUserResizing = false
        };

        _renderLoop = new RenderLoop(_window);
        _renderTarget = InitializeRenderingDevice();
        _brush = new SolidColorBrush(_renderTarget, DefaultShapeColor);
    }

    internal void Run()
    {
        _window.Show();
    }

    internal void Render()  //  TODO: Добавить буффер для отрисовки кадров (SwapChain). Понять, как он работает для 2д.
    {
        if (_renderLoop.NextFrame())
        {
            _renderTarget.BeginDraw();
            _renderTarget.Clear(DefaultBackgroundColor);

            TestDrawGameObjects();

            _renderTarget.EndDraw();
        }
    }

    public void Dispose()
    {
        _window.Dispose();
        _renderLoop.Dispose();

        _renderTarget.Dispose();
    }

    private RenderTarget InitializeRenderingDevice()
    {
        var renderTargetProperties = new RenderTargetProperties(
            new PixelFormat(Format.R8G8B8A8_UNorm, DX2D1.AlphaMode.Premultiplied));

        var hwndRenderTargetProperties = new HwndRenderTargetProperties()
        {
            Hwnd = _window.Handle,
            PixelSize = new Size2(_window.Width, _window.Height)
        };

        using var factory = new DX2D1.Factory();
        return new WindowRenderTarget(factory, renderTargetProperties, hwndRenderTargetProperties);
    }

    [Obsolete("Test realization for physic. Replace this after testing.")]  //  TODO:
    private void TestDrawGameObjects()  //  TODO: Нужно добавить переворот по Y. Сейчас при отрисовке Y растет вниз.
    {
        foreach (var gameObject in _levelManager.CurrentLevel.GameObjects)
        {
            if (gameObject.TryGetComponent<RigidBody>(out var body))
            {
                switch (body.Shape)
                {
                    case CirclePhysicShape circle:
                        DrawCircle(circle);
                        break;
                    case RectanglePhysicShape rectangle:
                        DrawRectangle(rectangle);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }

    private void DrawCircle(CirclePhysicShape circleShape)
    {
        var circle = new Ellipse(circleShape.Center.ToRawVector2(), circleShape.Radius, circleShape.Radius);
        _renderTarget.FillEllipse(circle, _brush);
    }

    private void DrawRectangle(RectanglePhysicShape rectangleShape)
    {
        var center = rectangleShape.Center;

        var rotationMatrix = Matrix3x2.Rotation(rectangleShape.Rotation, center.ToDXVector2()); //  TODO: Этот поворот не работает. Найти другой способ рисовать повернутый прямоугольник.
        _renderTarget.Transform = rotationMatrix;

        var rectangle = new RawRectangleF(
            center.X - rectangleShape.Size.X / 2,
            center.Y + rectangleShape.Size.Y / 2,
            center.X + rectangleShape.Size.X / 2,
            center.Y - rectangleShape.Size.Y / 2);

        _renderTarget.FillRectangle(rectangle, _brush);
    }
}