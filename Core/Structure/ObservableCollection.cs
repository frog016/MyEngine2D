using System.Collections;

namespace MyEngine2D.Core.Structure;

public class ObservableCollection<T> : IReadOnlyObservable<T>, ICollection<T>
{
    public int Count => _container.Count;
    public bool IsReadOnly => false;

    public event Action<T> Added;
    public event Action<T> Removed;

    private readonly List<T> _container = new();

    public void Add(T item)
    {
        _container.Add(item);

        Added?.Invoke(item);
    }

    public bool Remove(T item)
    {
        if (_container.Remove(item) == false)
            return false;

        Removed?.Invoke(item);
        return true;
    }

    public bool Contains(T item)
    {
        return _container.Contains(item);
    }

    public void Clear()
    {
        _container.Clear();
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        _container.CopyTo(array, arrayIndex);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _container.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}