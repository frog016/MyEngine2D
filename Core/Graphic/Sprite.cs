using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using SharpDX;

using Rectangle = System.Drawing.Rectangle;

namespace MyEngine2D.Core.Graphic;

public sealed class Sprite : IDisposable
{
    public Structure.Vector2 Size => new(Bitmap.Size.Width, Bitmap.Size.Height);

    internal SharpDX.Direct2D1.Bitmap Bitmap;

    private readonly string _pathToFile;

    public Sprite(string file)  //  TODO: сделать конструктор internal и создатвать их в spriteRenderer.
    {
        _pathToFile = file;
    }

    public void Dispose()
    {
        Bitmap.Dispose();
    }

    internal void InitializeSprite(SharpDX.Direct2D1.RenderTarget renderTarget)
    {
        Bitmap = CreateBitmap(renderTarget, _pathToFile);
    }

    private static SharpDX.Direct2D1.Bitmap CreateBitmap(SharpDX.Direct2D1.RenderTarget renderTarget, string file)
    {
        using var bitmap = (Bitmap)Image.FromFile(file);    //  TODO: Поменять загрузку изображения на загрузку через ResourceManager.

        var bitmapRectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
        var bitmapProperties = new SharpDX.Direct2D1.BitmapProperties(renderTarget.PixelFormat);
        var bitmapSize = new Size2(bitmap.Width, bitmap.Height);

        var stride = bitmap.Width * sizeof(int);
        using var tempStream = new DataStream(bitmap.Height * stride, true, true);

        ConvertBGRAToRGBA(bitmap, bitmapRectangle, tempStream);
        tempStream.Position = 0;

        return new SharpDX.Direct2D1.Bitmap(renderTarget, bitmapSize, tempStream, stride, bitmapProperties);
    }

    private static void ConvertBGRAToRGBA(Bitmap bitmap, Rectangle sourceArea, DataStream tempStream)
    {
        var bitmapData = bitmap.LockBits(sourceArea, ImageLockMode.ReadOnly, PixelFormat.Format32bppPArgb);

        for (var y = 0; y < bitmap.Height; y++)
        {
            var offset = bitmapData.Stride * y;
            for (var x = 0; x < bitmap.Width; x++)
            {
                var b = Marshal.ReadByte(bitmapData.Scan0, offset++);
                var g = Marshal.ReadByte(bitmapData.Scan0, offset++);
                var r = Marshal.ReadByte(bitmapData.Scan0, offset++);
                var a = Marshal.ReadByte(bitmapData.Scan0, offset++);
                var rgba = r | (g << 8) | (b << 16) | (a << 24);

                tempStream.Write(rgba);
            }
        }

        bitmap.UnlockBits(bitmapData);
    }
}