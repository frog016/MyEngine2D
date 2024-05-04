using MyEngine2D.Core.Structure;
using SharpDX.Mathematics.Interop;

namespace MyEngine2D.Core.Utility;

internal static class Vector2Extensions
{
    internal static System.Numerics.Vector2 ToNumericVector2(this Vector2 vector)
    {
        return new System.Numerics.Vector2(vector.X, vector.Y);
    }

    internal static System.Numerics.Vector3 ToNumericVector3(this Vector2 vector)
    {
        return new System.Numerics.Vector3(vector.X, vector.Y, 0);
    }

    internal static Vector2 ToVector2(this SharpDX.Size2F size)
    {
        return new Vector2(size.Width, size.Height);
    }

    internal static RawVector2 ToRawVector2(this Vector2 vector)
    {
        return new RawVector2(vector.X, vector.Y);
    }

    internal static SharpDX.Vector2 ToDXVector2(this Vector2 vector)
    {
        return new SharpDX.Vector2(vector.X, vector.Y);
    }

    internal static SharpDX.Vector2 MirrorY(this SharpDX.Vector2 dxVector)
    {
        return new SharpDX.Vector2(dxVector.X, -dxVector.Y);
    }

    internal static RawVector2 MirrorY(this RawVector2 dxVector)
    {
        return new RawVector2(dxVector.X, -dxVector.Y);
    }

    internal static Vector2 MirrorY(this Vector2 vector)
    {
        return new Vector2(vector.X, -vector.Y);
    }
}