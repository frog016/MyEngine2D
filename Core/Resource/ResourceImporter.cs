namespace MyEngine2D.Core.Resource;

public abstract class ResourceImporter
{
    public abstract Type ResourceType { get; }
}

public abstract class ResourceImporter<TResource> : ResourceImporter
{
    public override Type ResourceType => typeof(TResource);

    public abstract TResource Load(string fullPath);
    public abstract Task<TResource> LoadAsync(string fullPath);
}