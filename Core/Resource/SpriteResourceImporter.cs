using MyEngine2D.Core.Graphic;

namespace MyEngine2D.Core.Resource;

public sealed class SpriteResourceImporter : ResourceImporter<Sprite>
{
    public override Sprite Load(string fullPath)
    {
        var bitmap = (Bitmap)Image.FromFile(fullPath);
        return new Sprite(bitmap);
    }

    public override async Task<Sprite> LoadAsync(string fullPath)
    {
        await using var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
        var byteBuffer = new byte[stream.Length];
        var byteCount = await stream.ReadAsync(byteBuffer, 0, byteBuffer.Length);

        using var memoryStream = new MemoryStream(byteBuffer, 0, byteCount);
        var bitmap = (Bitmap)Image.FromStream(memoryStream);
        return new Sprite(bitmap);
    }
}