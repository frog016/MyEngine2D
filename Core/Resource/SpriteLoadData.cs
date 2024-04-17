namespace MyEngine2D.Core.Resource;

public readonly struct SpriteLoadData
{
    public readonly string FileRelativePath;
    public readonly int PixelsPerUnit;

    public const int DefaultPixelsPerUnit = 100;

    public SpriteLoadData(string fileRelativePath)
    {
        FileRelativePath = fileRelativePath;
        PixelsPerUnit = DefaultPixelsPerUnit;
    }

    public SpriteLoadData(string fileRelativePath, int pixelsPerUnit)
    {
        FileRelativePath = fileRelativePath;
        PixelsPerUnit = pixelsPerUnit;
    }
}