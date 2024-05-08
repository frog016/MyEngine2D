using MyEngine2D.Core.Factory;
using MyEngine2D.Core.Structure;

namespace MyEngine2D.Core.Entity
{
    public sealed class GameObject : IDisposable
    {
        public string Name { get; set; }

        public readonly Transform Transform;

        internal bool DestroyRequest;

        private readonly List<Component> _components;

        private bool _isStarted;

        public GameObject(string name, Vector2 position = default, float rotation = default, params Component[] components)
        {
            Name = name;
            _components = components.ToList();

            Transform = ComponentFactory.CreateComponent<Transform>(this);
            Transform.Position = position;
            Transform.Rotation = rotation;

            _components.Add(Transform);
        }

        public void Start()
        {
            foreach (var component in _components)
                component.Start();

            _isStarted = true;
        }

        public void UpdateObject(float deltaTime)
        {
            foreach (var component in _components)
                component.Update(deltaTime);
        }

        public void FixedUpdateObject(float fixedDeltaTime)
        {
            foreach (var component in _components)
                component.FixedUpdate(fixedDeltaTime);
        }

        public void Destroy()
        {
            DestroyRequest = true;
        }

        public T AddComponent<T>() where T : Component
        {
            var component = ComponentFactory.CreateComponent<T>(this);
            _components.Add(component);

            if (_isStarted)
                component.Start();

            return component;
        }

        public void RemoveComponent<T>() where T : Component
        {
            if (TryGetComponent<T>(out var component))
                return;

            _components.Remove(component);
            component.OnDestroy();
        }

        public T GetComponent<T>() where T : Component
        {
            var component = _components.FirstOrDefault(component => component is T);
            return component as T ?? throw new NullReferenceException($"{this} doesn't contains component: {typeof(T).Name}.");
        }

        public bool TryGetComponent<T>(out T? component) where T : Component
        {
            component = null;
            if (ContainsComponent<T>() == false)
            {
                return false;
            }

            component = GetComponent<T>();
            return true;
        }

        public bool ContainsComponent<T>() where T : Component
        {
            return _components.Any(component => component is T);
        }

        public void Dispose()
        {
            foreach (var component in _components)
                component.OnDestroy();

            _components.Clear();
        }

        public override string ToString()
        {
            return $"GameObject: {Name}";
        }
    }
}
