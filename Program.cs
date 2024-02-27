using MyEngine2D.Core;
using MyEngine2D.Core.Entity;

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
            var testLevel = new SceneLevel("Test Level");
            var testGameObject = testLevel.Instantiate("Test Object");
            testGameObject.AddComponent<TestLogComponent>();

            var levels = new List<SceneLevel>()
            {
                testLevel
            };

            var game = Game.Instance;
            game.Initialize(levels);

            return game;
        }

        public class TestLogComponent : Component
        {
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