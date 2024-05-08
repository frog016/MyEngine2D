using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using SharpDX;

using Rectangle = System.Drawing.Rectangle;

namespace MyEngine2D.Core.Graphic;

public sealed class Sprite : IDisposable
{
    public int PixelsPerUnit { get; private set; }
    public Structure.Vector2 Size { get; private set; }
    public Structure.Vector2 TextureSize { get; private set; }

    internal SharpDX.Direct2D1.Bitmap DirectBitmap;

    private readonly Bitmap _bitmap;

    internal Sprite(Bitmap bitmap)
    {
        _bitmap = bitmap;
    }

    internal void InitializeInternal(SharpDX.Direct2D1.RenderTarget renderTarget, int pixelsPerUnit)
    {
        PixelsPerUnit = pixelsPerUnit;
        Size = new Structure.Vector2(_bitmap.Width, _bitmap.Height) / PixelsPerUnit;
        TextureSize = new Structure.Vector2(_bitmap.Width, _bitmap.Height);

        DirectBitmap = CreateBitmap(renderTarget, _bitmap);
    }

    public void Dispose()
    {
        DirectBitmap.Dispose();
        _bitmap.Dispose();
    }

    private static SharpDX.Direct2D1.Bitmap CreateBitmap(SharpDX.Direct2D1.RenderTarget renderTarget, Bitmap bitmap)
    {
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

        for (var y = bitmap.Height - 1; y >= 0; y--)
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