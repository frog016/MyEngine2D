using MyEngine2D.Core;
using MyEngine2D.Core.Entity;
using MyEngine2D.Core.Graphic;
using MyEngine2D.Core.Input;
using MyEngine2D.Core.Level;
using MyEngine2D.Core.Math;
using MyEngine2D.Core.Resource;
using MyEngine2D.Core.Structure;
using MyEngine2D.Core.Utility;
using SharpDX.DirectInput;

namespace MyEngine2D;

public class GraphicOverviewPreset : OverviewPreset
{
    private const float ScreenX = 1920;
    private const float ScreenY = 1080;

    protected override Game CreateOverviewGame()
    {
        var testLevel = new GameLevel("Graphic Level");

        var game = new GameBuilder()
            .WithCustomLevels(testLevel)
            .WithInputActions(
                new WKeyInput(), new SKeyInput(), new AKeyInput(), new DKeyInput(),
                new QKeyInput(), new EKeyInput(), new ZKeyInput(), new XKeyInput())
            .Build();

        var camera = Camera.CreateDefault(testLevel);
        camera.GameObject.AddComponent<CameraMovement>();

        var position = Vector2.Zero;
        CreateHero(testLevel, position);

        CreateObject(testLevel, position - new Vector2(ScreenX / 4f, 0), Math2D.PI / 3);
        CreateObject(testLevel, position + new Vector2(ScreenX / 4f, 0), 2.15f);

        CreateBackground(testLevel, position);

        return game;
    }

    private static void CreateHero(GameLevel level, Vector2 position)
    {
        var testObject = level.Instantiate("Hero", position);

        var spriteRender = testObject.AddComponent<SpriteRenderer>();
        var spriteData = new SpriteLoadData("HeroKnight_Idle_0.png");
        spriteRender.Initialize(
            spriteData, 
            scale: Vector2.One * SpriteLoadData.DefaultPixelsPerUnit, 
            layer: 1,
            opacity: 1);

        var spriteAnimator = testObject.AddComponent<SpriteAnimator>();
        var animationsData = CreateHeroAnimationsData();
        spriteAnimator.Initialize(animationsData);

        testObject.AddComponent<HeroController>();
    }

    private static void CreateObject(GameLevel level, Vector2 position, float rotation)
    {
        var testObject = level.Instantiate("Test Object", position, rotation);
        testObject.AddComponent<RotationComponent>().Initialize(Math2D.PI / 4f);
        testObject.AddComponent<MoveComponent>().Initialize(300, 500);

        var spriteRender = testObject.AddComponent<SpriteRenderer>();
        var spriteData = new SpriteLoadData("Ball.png");
        spriteRender.Initialize(
            spriteData,
            scale: Vector2.One * SpriteLoadData.DefaultPixelsPerUnit,
            opacity: 1);
    }

    private static void CreateBackground(GameLevel level, Vector2 position)
    {
        var testBackground = level.Instantiate("Test Background", position);
        var spriteRenderer = testBackground.AddComponent<SpriteRenderer>();

        var spriteData = new SpriteLoadData("Background.png");
        spriteRenderer.Initialize(
            spriteData,
            scale: 3 * Vector2.One * SpriteLoadData.DefaultPixelsPerUnit,
            layer: -1);
    }

    private static AnimationLoadData[] CreateHeroAnimationsData()
    {
        const int idleFramesCount = 8;
        const float frameDuration = 0.25f;

        var animationFrames = Enumerable
            .Range(0, idleFramesCount)
            .Select(index => new AnimationFrameLoadData(new SpriteLoadData($"HeroKnight_Idle_{index}.png"), frameDuration * index))
            .ToArray();

        var animationData = new AnimationLoadData("Idle", 1, true, animationFrames);
        return new AnimationLoadData[] { animationData };
    }
}

public class HeroController : Component
{
    public HeroController(GameObject gameObject) : base(gameObject)
    {
    }

    public override void Start()
    {
        GameObject.GetComponent<SpriteAnimator>()?.PlayAnimation("Idle");
    }
}

public class RotationComponent : Component
{
    private float _rotationSpeedRadians;

    public RotationComponent(GameObject gameObject) : base(gameObject)
    {
    }

    public void Initialize(float rotationSpeedRadians)
    {
        _rotationSpeedRadians = rotationSpeedRadians;
    }

    public override void FixedUpdate(float fixedDeltaTime)
    {
        GameObject.Transform.Rotation += _rotationSpeedRadians * fixedDeltaTime;
    }
}

public class MoveComponent : Component
{
    private float _moveSpeed;
    private float _maxMoveDistance;

    private Vector2 _targetPosition;
    private Vector2 _direction;

    public MoveComponent(GameObject gameObject) : base(gameObject)
    {
    }

    public void Initialize(float moveSpeed, float maxMoveDistance)
    {
        _moveSpeed = moveSpeed;
        _maxMoveDistance = maxMoveDistance;

        _direction = Vector2.Up;
        _targetPosition = GameObject.Transform.Position + _direction * _maxMoveDistance;
    }

    public override void FixedUpdate(float fixedDeltaTime)
    {
        var distance = (_targetPosition - GameObject.Transform.Position);
        if (distance.Length() <= 1e-2)
        {
            _direction = -_direction;
            _targetPosition = GameObject.Transform.Position + _direction * _maxMoveDistance;
        }

        GameObject.Transform.Position += _direction * _moveSpeed * fixedDeltaTime;
    }
}

public class CameraMovement : Component
{
    private readonly Camera _camera;
    private readonly InputSystem _inputSystem;
    private readonly Time _time;

    private readonly float _speed = 100;
    private readonly float _torque = Math2D.PI / 18;
    private readonly float _zoom = 2;

    public CameraMovement(GameObject gameObject, GameLevelManager levelManager, InputSystem inputSystem, Time time) : base(gameObject)
    {
        _camera = levelManager.CurrentLevel.GameObjects.FindByType<Camera>();
        _inputSystem = inputSystem;
        _time = time;
    }

    public override void Start()
    {
        _inputSystem.SubscribeInputListener<WKeyInput>(OnWPressed);
        _inputSystem.SubscribeInputListener<SKeyInput>(OnSPressed);
        _inputSystem.SubscribeInputListener<AKeyInput>(OnAPressed);
        _inputSystem.SubscribeInputListener<DKeyInput>(OnDPressed);
        _inputSystem.SubscribeInputListener<QKeyInput>(OnQPressed);
        _inputSystem.SubscribeInputListener<EKeyInput>(OnEPressed);
        _inputSystem.SubscribeInputListener<ZKeyInput>(OnZPressed);
        _inputSystem.SubscribeInputListener<XKeyInput>(OnXPressed);
    }

    public override void OnDestroy()
    {
        _inputSystem.UnsubscribeInputListener<WKeyInput>(OnWPressed);
        _inputSystem.UnsubscribeInputListener<SKeyInput>(OnSPressed);
        _inputSystem.UnsubscribeInputListener<AKeyInput>(OnAPressed);
        _inputSystem.UnsubscribeInputListener<DKeyInput>(OnDPressed);
        _inputSystem.UnsubscribeInputListener<QKeyInput>(OnQPressed);
        _inputSystem.UnsubscribeInputListener<EKeyInput>(OnEPressed);
        _inputSystem.UnsubscribeInputListener<ZKeyInput>(OnZPressed);
        _inputSystem.UnsubscribeInputListener<XKeyInput>(OnXPressed);
    }

    private void OnWPressed()
    {
        _camera.Transform.Position += Vector2.Up * _speed * _time.DeltaTime;
    }

    private void OnSPressed()
    {
        _camera.Transform.Position += Vector2.Down * _speed * _time.DeltaTime;
    }

    private void OnAPressed()
    {
        _camera.Transform.Position += Vector2.Left * _speed * _time.DeltaTime;
    }

    private void OnDPressed()
    {
        _camera.Transform.Position += Vector2.Right * _speed * _time.DeltaTime;
    }

    private void OnQPressed()
    {
        _camera.Transform.Rotation += _torque * _time.DeltaTime;
    }

    private void OnEPressed()
    {
        _camera.Transform.Rotation -= _torque * _time.DeltaTime;
    }

    private void OnZPressed()
    {
        _camera.OrthoSize += _zoom * _time.DeltaTime;
    }

    private void OnXPressed()
    {
        _camera.OrthoSize -= _zoom * _time.DeltaTime;
    }
}

public class WKeyInput : KeyboardInputAction
{
    protected override Key TriggeredKey => Key.W;
}

public class SKeyInput : KeyboardInputAction
{
    protected override Key TriggeredKey => Key.S;
}

public class AKeyInput : KeyboardInputAction
{
    protected override Key TriggeredKey => Key.A;
}

public class DKeyInput : KeyboardInputAction
{
    protected override Key TriggeredKey => Key.D;
}

public class QKeyInput : KeyboardInputAction
{
    protected override Key TriggeredKey => Key.Q;
}

public class EKeyInput : KeyboardInputAction
{
    protected override Key TriggeredKey => Key.E;
}

public class ZKeyInput : KeyboardInputAction
{
    protected override Key TriggeredKey => Key.Z;
}

public class XKeyInput : KeyboardInputAction
{
    protected override Key TriggeredKey => Key.X;
}