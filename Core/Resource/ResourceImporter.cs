namespace MyEngine2D.Core.Resource;

public abstract class ResourceImporter
{
}

public abstract class ResourceImporter<TResource> : ResourceImporter
{
    public abstract TResource Load(string fullPath);
    public abstract Task<TResource> LoadAsync(string fullPath);
}