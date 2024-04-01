using MyEngine2D.Core;
using MyEngine2D.Core.Entity;
using MyEngine2D.Core.Level;
using MyEngine2D.Core.Math;
using MyEngine2D.Core.Physic;
using MyEngine2D.Core.Structure;

namespace MyEngine2D
{
    public class Program
    {
        private const float MinSize = 10f;
        private const float MaxSize = 25;

        [STAThread]
        public static void Main()
        {
            using var game = CreateTestGame();
            game.Run();
        }

        private static Game CreateTestGame()
        {
            var testLevel = new GameLevel("Test Level");

            var game = new GameBuilder()
                .WithCustomLevels(testLevel)
                .WithInputActions()
                .Build();

            CreateGroundRectangle(testLevel);

            var startPosition = new Vector2(100, 1080 / 2f);
            for (var index = 0; index < 20; )
            {
                var currentPosition = NextPosition(index);
                CreateTestCircle(testLevel, currentPosition, index);
                index++;

                currentPosition = NextPosition(index);
                CreateTestRectangle(testLevel, currentPosition, index);
                index++;
            }

            return game;

            Vector2 NextPosition(int index)
            {
                return startPosition + Vector2.Right * index * MaxSize;
            }
        }

        private static void CreateTestCircle(GameLevel testLevel, Vector2 position, int index)
        {
            var gameObject = testLevel.Instantiate($"Circle Object: {index}.", position);
            var rigidBody = gameObject.AddComponent<RigidBody>();

            var randomRadius = GetRandomSize() / 2f;
            var shape = new CirclePhysicShape(rigidBody, randomRadius);

            var ironDensity = 7874f;
            var ironMaterial = new PhysicMaterial(ironDensity, 0.2f, 0.1f, 0.75f);

            rigidBody.Initialize(shape, ironMaterial);
        }

        private static void CreateTestRectangle(GameLevel testLevel, Vector2 position, int index)
        {
            var gameObject = testLevel.Instantiate($"Rect Object: {index}.", position);
            var rigidBody = gameObject.AddComponent<RigidBody>();

            var randomSize = new Vector2(GetRandomSize(), GetRandomSize());
            var randomRotation = GetRandomSize(0, 360);

            rigidBody.Rotation = randomRotation;
            var shape = new RectanglePhysicShape(rigidBody, randomSize);

            var ironDensity = 7874f;
            var ironMaterial = new PhysicMaterial(ironDensity, 0.2f, 0.1f, 0.75f);

            rigidBody.Initialize(shape, ironMaterial);
        }

        private static void CreateGroundRectangle(GameLevel testLevel)
        {
            var position = new Vector2(1920 / 2f, 1080 / 10f);
            var size = new Vector2(1920, 100);

            var testGameObject = testLevel.Instantiate($"Test Ground.", position);
            var body = testGameObject.AddComponent<RigidBody>();

            var shape = new RectanglePhysicShape(body, size);

            var ironDensity = 7874f;
            var ironMaterial = new PhysicMaterial(ironDensity, 0.2f, 0.1f, 0.75f);

            body.Initialize(shape, ironMaterial, gravityScale: 0, isStatic: true);
        }

        private static float GetRandomSize(float min = MinSize, float max = MaxSize)
        {
            var unClampedSize = Random.Shared.NextSingle() * max;
            return Math2D.Clamp(unClampedSize, min, max);
        }
    }

    public class DebugComponent : Component
    {
        public DebugComponent(GameObject gameObject) : base(gameObject)
        {
        }

        public override void FixedUpdate(float fixedDeltaTime)
        {
            base.FixedUpdate(fixedDeltaTime);

            Console.WriteLine($"Object: {GameObject.Name} in position: {GameObject.Transform.Position}.");
        }
    }
}