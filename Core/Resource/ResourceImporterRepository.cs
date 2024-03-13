namespace MyEngine2D.Core.Resource;

public class ResourceImporterRepository
{
    private readonly Dictionary<Type, ResourceImporter> _importers = new();

    public void BindImporter<TResource>(ResourceImporter<TResource> resourceImporter)
    {
        var resourceType = typeof(TResource);
        if (_importers.TryAdd(resourceType, resourceImporter)) 
            return;

        var bindImporter = _importers[resourceType];
        throw new InvalidOperationException($"Can't bind {resourceImporter}. Type: {resourceType} already have bind Importer: {bindImporter}.");
    }

    public void RemoveImporter<TResource>()
    {
        var resourceType = typeof(TResource);
        if (_importers.Remove(resourceType))
            return;

        throw new KeyNotFoundException($"Importer not found. Resource Type: {resourceType}.");
    }

    public ResourceImporter<TResource> ProvideImporter<TResource>()
    {
        var resourceType = typeof(TResource);
        if (_importers.TryGetValue(resourceType, out var resourceImporter) == false)
            throw new KeyNotFoundException($"Importer not found. Resource Type: {resourceType}. Bind it before using.");

        return (ResourceImporter<TResource>)resourceImporter;
    }
}