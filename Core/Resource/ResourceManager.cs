namespace MyEngine2D.Core.Resource;

public sealed class ResourceManager
{
    public static readonly string ResourceFolder = "Content";
    public static readonly string PersistentPath = Path.Combine(AppContext.BaseDirectory, ResourceFolder);

    private readonly Dictionary<string, object> _loadedResources = new();

    //    TODO: Добавить проверку на соответсвие типу ресурса, если он уже сохранен
    //    TODO: Если ресурс не загруже, загрузить с помощью реализации ResourceHolder
    //    TODO: Добавить ассинхронную версию загрузки
    //    TODO: Добавить очистку загруженных ресурсов
    public TResource LoadResource<TResource>(string relativePath)
    {
        var fullPath = GetResourceFullPath(relativePath);
        if (_loadedResources.ContainsKey(fullPath, out var resource))
            
    }

    private string GetResourceFullPath(string relativePath)
    {
        return Path.Combine(PersistentPath, relativePath);
    }
}

public abstract class ResourceHolder<T>
{
    public abstract T Load();
    public abstract Task<T> LoadAsync();
}