using MyEngine2D.Core;
using MyEngine2D.Core.Entity;
using MyEngine2D.Core.Level;

namespace MyEngine2D
{
    public class Program
    {
        public static void Main()
        {
            var game = CreateTestGame();
            game.Start();
        }

        private static Game CreateTestGame()
        {
            var testLevel = new GameLevel("Test Level");
            var testGameObject = testLevel.Instantiate("Test Object");
            testGameObject.AddComponent<TestLogComponent>();

            var levels = new List<GameLevel>()
            {
                testLevel
            };

            return new GameBuilder()
                .WithCustomLevels(levels)
                .Build();
        }

        public class TestLogComponent : Component
        {
            public TestLogComponent(GameObject gameObject) : base(gameObject)
            {
            }

            public override void Update(float deltaTime)
            {
                Console.WriteLine($"Update for {GameObject.Name} - deltaTime: {deltaTime}.");
            }

            public override void FixedUpdate(float fixedDeltaTime)
            {
                Console.WriteLine($"FixedUpdate for {GameObject.Name} - fixedDeltaTime: {fixedDeltaTime}.");
            }
        }
    }
}