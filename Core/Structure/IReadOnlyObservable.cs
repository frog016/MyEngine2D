namespace MyEngine2D.Core.Structure;

public interface IReadOnlyObservable<out T> : IReadOnlyCollection<T>
{
    event Action<T> Added;
    event Action<T> Removed;
}