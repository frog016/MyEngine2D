namespace MyEngine2D.Core.Entity;

public abstract class Component
{
    public GameObject GameObject { get; private set; }
    public Transform Transform => GameObject.Transform;

    protected Component(GameObject gameObject)
    {
        GameObject = gameObject;
    }

    public virtual void Start() { }
    public virtual void OnDestroy() { }
    public virtual void Update(float deltaTime) { }
    public virtual void FixedUpdate(float fixedDeltaTime) { }
}