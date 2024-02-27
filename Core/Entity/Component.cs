namespace MyEngine2D.Core.Entity;

public abstract class Component
{
    public GameObject GameObject { get; private set; }

    public void Initialize(GameObject gameObject)
    {
        GameObject = gameObject;
    }

    public virtual void Create() { }
    public virtual void Start() { }
    public virtual void Delete() { }
    public virtual void Update(float deltaTime) { }
    public virtual void FixedUpdate(float fixedDeltaTime) { }
}