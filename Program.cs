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

            for (var index = 0; index < 1; index++)
                CreateTestCircles(testLevel, index);

            return game;
        }

        private static void CreateTestCircles(GameLevel testLevel, int index = 0)
        {
            const float maxRadius = 50;
            var screenCenter = new Vector2(1920 / 10f, 1080 / 2f) + Vector2.Right * index * 2.5f * maxRadius;

            var testGameObject = testLevel.Instantiate($"Test Object: {index}.", screenCenter);
            var body = testGameObject.AddComponent<RigidBody>();

            var radius = Random.Shared.NextSingle() * maxRadius;
            var shape = new CirclePhysicShape(body, radius);

            var ironDensity = 7874f;
            var ironMaterial = new PhysicMaterial(ironDensity, 0.2f, 0.1f, 0.5f);

            body.Initialize(shape, ironMaterial);

            testGameObject.AddComponent<DebugComponent>();
        }

        private static void CreateGroundRectangle(GameLevel testLevel)
        {
            var position = new Vector2(1920 / 2f, 1080 / 10f);
            var size = new Vector2(1920, 100);

            var testGameObject = testLevel.Instantiate($"Test Ground.", position);
            var body = testGameObject.AddComponent<RigidBody>();
            
            var shape = new RectanglePhysicShape(body, size);

            var ironDensity = 7874f;
            var ironMaterial = new PhysicMaterial(ironDensity, 0.2f, 0.1f, 0.5f);

            body.Initialize(shape, ironMaterial, 0, true);
            
            //testGameObject.AddComponent<DebugComponent>();
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