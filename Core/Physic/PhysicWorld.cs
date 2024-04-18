using MyEngine2D.Core.Level;
using MyEngine2D.Core.Math;

namespace MyEngine2D.Core.Physic;

public sealed class PhysicWorld
{
    private readonly GameLevelManager _levelManager;

    private static readonly HashSet<(RigidBody, RigidBody)> CollidingBodyPairCache = new();

    private ICollisionResolutionMethod _collisionResolution = new ImpulseCollisionResolutionMethod();

    public PhysicWorld(GameLevelManager levelManager)
    {
        _levelManager = levelManager;
    }

    public void SetCollisionMethod(ICollisionResolutionMethod collisionResolution)
    {
        _collisionResolution = collisionResolution;
    }

    internal void UpdatePhysic(float fixedDeltaTime)
    {
        //  find collisions
        var physicObjects = GetWorldPhysicObjects().ToArray();  //  TODO: добавить кэширование. превратить список объектов сцены в обсервабле и при его изменении изменять текущий список
        var collidingObjectPairs = GetPotentialCollidingObjectPairs(physicObjects);
        var collisionContacts = GetObjectCollisionContacts(collidingObjectPairs).ToArray();

        //  solve collisions
        foreach (var collisionContact in collisionContacts)
        {
            ResolveCollision(collisionContact);
        }

        //  update rigid body states
        foreach (var body in physicObjects)
        {
            body.UpdateBodyPhysic(fixedDeltaTime);
        }

        //  correct positions of collided bodies
        foreach (var collisionContact in collisionContacts)
        {
            CorrectCollisionContactPositions(collisionContact);
        }

        //  reset forces
        foreach (var body in physicObjects)
        {
            body.ResetForces();
        }
    }

    private IEnumerable<RigidBody> GetWorldPhysicObjects()
    {
        foreach (var gameObject in _levelManager.CurrentLevel.GameObjects)
        {
            if (gameObject.TryGetComponent<RigidBody>(out var body))
            {
                yield return body;
            }
        }
    }

    private void ResolveCollision(Contact contact)
    {
        _collisionResolution.ResolveCollision(contact);
    }

    //  Broad phase
    private static IEnumerable<(RigidBody first, RigidBody second)> GetPotentialCollidingObjectPairs(RigidBody[] physicBodies)
    {
        foreach (var body in physicBodies)
        {
            if (body.IsStatic)
            {
                continue;
            }

            foreach (var otherBody in physicBodies)
            {
                if (body == otherBody)
                {
                    continue;
                }

                var firstBoundingBox = body.Shape.GetBoundingBox();
                var secondBoundingBox = otherBody.Shape.GetBoundingBox();

                if (firstBoundingBox.Intersect(secondBoundingBox))
                {
                    yield return (body, otherBody);
                }
            }
        }
    }

    //  Narrow phase
    private static IEnumerable<Contact> GetObjectCollisionContacts(IEnumerable<(RigidBody first, RigidBody second)> pairs)
    {
        CollidingBodyPairCache.Clear();

        foreach (var (first, second) in pairs)
        {
            if (IsPairAlreadyCollided(first, second))
            {
                continue;
            }

            var firstShape = first.Shape;
            var secondShape = second.Shape;

            var manifold = firstShape.IntersectWith(secondShape);
            if (manifold.HasValue)
            {
                yield return new Contact(first, second, manifold.Value);
            }
        }
    }

    private static bool IsPairAlreadyCollided(RigidBody first, RigidBody second)
    {
        var pair = (first, second);
        var reversPair = (second, first);

        if (CollidingBodyPairCache.Contains(pair) || CollidingBodyPairCache.Contains(reversPair))
            return true;

        CollidingBodyPairCache.Add(pair);
        return false;
    }

    private static void CorrectCollisionContactPositions(Contact contact)
    {
        const float allowanceDepth = 0.05f;
        const float correctDepthPercent = 0.5f;

        var first = contact.First;
        var second = contact.Second;
        var manifold = contact.Manifold;

        var depth = Math2D.Max(manifold.Depth - allowanceDepth, 0);
        var correction = depth / (first.InverseMass + second.InverseMass) * manifold.Normal * correctDepthPercent;

        first.CorrectContactPosition(-correction);
        second.CorrectContactPosition(correction);
    }
}