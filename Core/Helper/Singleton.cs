namespace MyEngine2D.Core.Helper
{
    public abstract class Singleton<T> where T : Singleton<T>, new()
    {
        public static T Instance => LazyInstance.Value;

        private static readonly Lazy<T> LazyInstance = new(() => new T());
    }
}
