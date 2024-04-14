﻿using MyEngine2D.Core.Level;
using MyEngine2D.Core.Physic;
using MyEngine2D.Core.Utility;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using SharpDX.Windows;
using DX2D1 = SharpDX.Direct2D1;

namespace MyEngine2D.Core.Graphic;

internal sealed class GraphicRender : IDisposable
{
    private readonly GameLevelManager _levelManager;
    private readonly RenderForm _window;
    private readonly RenderLoop _renderLoop;
    private readonly RenderTarget _renderTarget;
    private readonly DX2D1.Factory _factory;
    private readonly DX2D1.Brush _brush;

    private readonly DX2D1.Brush _debugBrush;

    private static readonly SharpDX.Color DefaultBackgroundColor = new(32, 103, 176);
    private static readonly SharpDX.Color DefaultShapeColor = new(30, 30, 30);
    private static readonly SharpDX.Color DefaultDebugColor = new(255, 0, 0);

    private const float DebugPointSize = 4f;

    internal GraphicRender(GameLevelManager levelManager, GraphicWindowDescription description)
    {
        _levelManager = levelManager;
        _window = new RenderForm(description.Header)
        {
            ClientSize = new Size(description.Width, description.Height),
            AllowUserResizing = false
        };

        _renderLoop = new RenderLoop(_window);
        _factory = new DX2D1.Factory();

        _renderTarget = InitializeRenderingDevice();
        _brush = new SolidColorBrush(_renderTarget, DefaultShapeColor);
        _debugBrush = new SolidColorBrush(_renderTarget, DefaultDebugColor);
    }

    internal void Run()
    {
        _window.Show();

        foreach (var gameObject in _levelManager.CurrentLevel.GameObjects)
        {
            if (gameObject.TryGetComponent<SpriteRenderer>(out var spriteRenderer))
            {
                spriteRenderer.InitializeRender(_renderTarget);
            }
        }
    }

    internal void Render()  //  TODO: Добавить буффер для отрисовки кадров (SwapChain). Понять, как он работает для 2д.
    {
        if (_renderLoop.NextFrame())
        {
            _renderTarget.BeginDraw();
            _renderTarget.Clear(DefaultBackgroundColor);

            //TestDrawGameObjects();
            RenderGameObjects();

            _renderTarget.EndDraw();
        }
    }

    internal void DrawDebugPoint(Structure.Vector2 point, float radius = DebugPointSize)    //  TODO: Улучшить сисетму Debug отрисовки.
    {
        _renderTarget.BeginDraw();
        _renderTarget.DrawEllipse(new Ellipse(point.ToRawVector2(), radius, radius), _debugBrush);
        _renderTarget.EndDraw();
    }

    internal void DrawDebugLine(Structure.Vector2 start, Structure.Vector2 end, float width = DebugPointSize)
    {
        _renderTarget.BeginDraw();
        _renderTarget.DrawLine(start.ToRawVector2(), end.ToRawVector2(), _debugBrush, width);
        _renderTarget.EndDraw();
    }

    public void Dispose()
    {
        _window.Dispose();
        _renderLoop.Dispose();
        _renderTarget.Dispose();
        _factory.Dispose();
        _brush.Dispose();
        _debugBrush.Dispose();
    }

    private RenderTarget InitializeRenderingDevice()
    {
        var renderTargetProperties = new RenderTargetProperties(
            new PixelFormat(Format.R8G8B8A8_UNorm, DX2D1.AlphaMode.Premultiplied));

        var hwndRenderTargetProperties = new HwndRenderTargetProperties()
        {
            Hwnd = _window.Handle,
            PixelSize = new Size2(_window.Width, _window.Height),
        };

        var renderTarget = new WindowRenderTarget(_factory, renderTargetProperties, hwndRenderTargetProperties);
        renderTarget.Transform =
            Matrix3x2.Scaling(1, -1) *
            Matrix3x2.Translation(0, _window.Height);

        return renderTarget;
    }

    private void RenderGameObjects()
    {
        foreach (var gameObject in _levelManager.CurrentLevel.GameObjects)
        {
            if (gameObject.TryGetComponent<SpriteRenderer>(out var spriteRenderer))
            {
                spriteRenderer.Render(_renderTarget);
            }
        }
    }

    [Obsolete("Test realization for physic. Replace this after testing.")]  //  TODO:
    private void TestDrawGameObjects()
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
        var rectHalfSize = rectangleShape.Size / 2f;

        var rectangle = new RawRectangleF(
            center.X - rectHalfSize.X,
            center.Y + rectHalfSize.Y,
            center.X + rectHalfSize.X,
            center.Y - rectHalfSize.Y);

        using var rectGeometry = new RectangleGeometry(_factory, rectangle);

        var rotationMatrix = Matrix3x2.Rotation(rectangleShape.Rotation, center.ToDXVector2());
        using var transformedRect = new TransformedGeometry(_factory, rectGeometry, rotationMatrix);

        _renderTarget.FillGeometry(transformedRect, _brush);
    }
}