namespace MyEngine2D.Core.Resource;

public sealed class ResourceManager : ResourceImporterRepository, IDisposable
{
    public static readonly string ResourceFolder = "Content";
    public static readonly string PersistentPath = Path.Combine(AppContext.BaseDirectory, ResourceFolder);

    private readonly Dictionary<string, object> _loadedResources = new();

    public TResource LoadResource<TResource>(string relativePath)
    {
        var fullPath = GetResourceFullPath(relativePath);

        return TryGetCachedResource(fullPath, out TResource resource) 
            ? resource 
            : ImportResource<TResource>(fullPath);
    }

    public async Task<TResource> LoadResourceAsync<TResource>(string relativePath)
    {
        var fullPath = GetResourceFullPath(relativePath);
        return TryGetCachedResource(fullPath, out TResource resource)
            ? resource
            : await ImportResourceAsync<TResource>(fullPath);
    }

    public void Dispose()
    {
        foreach (var resource in _loadedResources.Values)
        {
            if (resource is IDisposable disposableResource)
            {
                disposableResource.Dispose();
            }
        }

        _loadedResources.Clear();
    }

    private bool TryGetCachedResource<TResource>(string fullPath, out TResource resource)
    {
        if (_loadedResources.TryGetValue(fullPath, out var resourceObject))
        {
            resource = (TResource)resourceObject;
            return true;
        }

        resource = default;
        return false;
    }

    private TResource ImportResource<TResource>(string fullPath)
    {
        var importer = ProvideImporter<TResource>();

        var resource = importer.Load(fullPath);
        _loadedResources.Add(fullPath, resource ?? throw new ArgumentNullException(nameof(resource)));

        return resource;
    }

    private Task<TResource> ImportResourceAsync<TResource>(string fullPath)
    {
        var importer = ProvideImporter<TResource>();

        var resource = importer.LoadAsync(fullPath);
        _loadedResources.Add(fullPath, resource ?? throw new ArgumentNullException(nameof(resource)));

        return resource;
    }

    private static string GetResourceFullPath(string relativePath)
    {
        return Path.Combine(PersistentPath, relativePath);
    }
}