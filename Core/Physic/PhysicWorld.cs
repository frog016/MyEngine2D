using MyEngine2D.Core.Level;

namespace MyEngine2D.Core.Physic;

public sealed class PhysicWorld
{
    private readonly GameLevelManager _levelManager;

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

    //  Broad phase
    private static IEnumerable<(RigidBody first, RigidBody second)> GetPotentialCollidingObjectPairs(RigidBody[] physicBodies)
    {
        foreach (var body in physicBodies)
        {
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
        foreach (var (first, second) in pairs)
        {
            var firstShape = first.Shape;
            var secondShape = second.Shape;

            var manifold = firstShape.IntersectWith(secondShape);
            if (manifold.HasValue)
            {
                yield return new Contact(first, second, manifold.Value);
            }
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

    private static void CorrectCollisionContactPositions(Contact contact)
    {
        const float depthCorrectPercent = 0.25f;

        var first = contact.First;
        var second = contact.Second;
        var manifold = contact.Manifold;

        var correction = manifold.Depth / (first.InverseMass + second.InverseMass) * manifold.Normal * depthCorrectPercent;

        first.Position -= correction * first.InverseMass;
        second.Position += correction * second.InverseMass;
    }
}