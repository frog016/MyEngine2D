using MyEngine2D.Core.Factory;
using MyEngine2D.Core.Structure;

namespace MyEngine2D.Core.Entity
{
    public sealed class GameObject : IDisposable
    {
        public string Name { get; set; }

        public readonly Transform Transform;

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

        public T? GetComponent<T>() where T : Component
        {
            return _components.FirstOrDefault(component => component.GetType() == typeof(T)) as T;
        }

        public bool TryGetComponent<T>(out T component) where T : Component
        {
            component = GetComponent<T>();
            return component != null;
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
