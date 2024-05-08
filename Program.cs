using MyEngine2D.Core;
using MyEngine2D.Core.Entity;
using MyEngine2D.Core.Graphic;
using MyEngine2D.Core.Input;
using MyEngine2D.Core.Level;
using MyEngine2D.Core.Math;
using MyEngine2D.Core.Physic;
using MyEngine2D.Core.Resource;

namespace MyEngine2D
{
    public class Program
    {
        [STAThread]
        public static void Main()
        {
            var game = new GameBuilder()
                .WithConfiguredLevels(new SpaceLevelConfigurator())
                .WithInputActions(GetInputActions())
                .Build();

            game.Run();
        }

        private static InputActionBase[] GetInputActions()
        {
            return new InputActionBase[]
            {
            };
        }
    }

    public class ShipEngine : Component
    {
        private readonly InputSystem _inputSystem;

        private RigidBody _rigidBody;
        private float _thrustSpeed = 1f;
        private float _rotationSpeed = 0.1f;
        private bool _needThrusting;
        private float _turnDirection;

        public ShipEngine(GameObject gameObject, InputSystem inputSystem) : base(gameObject)
        {
            _inputSystem = inputSystem;
        }

        public void Initialize(float thrustSpeed, float rotationSpeed)
        {
            _thrustSpeed = thrustSpeed;
            _rotationSpeed = rotationSpeed;
        }

        public override void Start()
        {
            _rigidBody = GameObject.GetComponent<RigidBody>();
        }

        public override void Update(float deltaTime)
        {
        }

        public override void FixedUpdate(float fixedDeltaTime)
        {
            if (_needThrusting)
            {
                _rigidBody.ApplyForce(Transform.GetForwardVector() * _thrustSpeed);
            }

            if (_turnDirection != 0f)
            {
                _rigidBody.ApplyTorque(_rotationSpeed * _turnDirection);
            }
        }
    }

    public class SpaceLevelConfigurator : GameLevelConfigurator
    {
        private const string LevelName = "SpaceLevel";
        private const string PlayerName = "Player_Ship";

        private readonly float _playerShapeRadius = 1f;
        private readonly PhysicMaterial _defaultMaterial = new(2200f, 0.2f, 0.1f, 0.8f);

        protected override GameLevel CreateConfiguredLevel()
        {
            var level = new GameLevel(LevelName);

            CreatePlayerShip(level);
            CreateBackground(level);
            var camera = Camera.CreateDefault(level);
            camera.OrthoSize = 3;

            return level;
        }

        private void CreatePlayerShip(GameLevel level)
        {
            var shipObject = level.Instantiate(PlayerName);

            var rigidBody = shipObject.AddComponent<RigidBody>();
            var shape = new CirclePhysicShape(rigidBody, _playerShapeRadius);
            rigidBody.Initialize(shape, _defaultMaterial, 1, ComputeMassMode.Manually, gravityScale: 0);

            var spriteRenderer = shipObject.AddComponent<SpriteRenderer>();
            var spriteLoadData = new SpriteLoadData("Ship/Ship_Idle.png");
            spriteRenderer.Initialize(spriteLoadData, layer: 1);

            var shipEngine = shipObject.AddComponent<ShipEngine>();
            shipEngine.Initialize(2, Math2D.PI / 9);
        }

        private static void CreateBackground(GameLevel level)
        {
            var background = level.Instantiate("Background");

            var spriteRenderer = background.AddComponent<SpriteRenderer>();
            var spriteLoadData = new SpriteLoadData("Space_Background.png");
            spriteRenderer.Initialize(spriteLoadData, layer: -1);
        }
    }
}