namespace MyEngine2D.Core.Structure;

public abstract class Singleton<T> where T : class, new()
{
    public static T Instance => LazyInstance.Value;

    private static readonly Lazy<T> LazyInstance = new(() => new T());
}