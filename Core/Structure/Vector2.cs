using MyEngine2D.Core.Math;
using SharpDX.Mathematics.Interop;

namespace MyEngine2D.Core.Structure
{
    public struct Vector2
    {
        public float X { get; set; }
        public float Y { get; set; }

        public static readonly Vector2 Zero = new(0, 0);
        public static readonly Vector2 One = new(1, 1);
        public static readonly Vector2 Left = new(-1, 0);
        public static readonly Vector2 Right = new(1, 0);
        public static readonly Vector2 Up = new(0, 1);
        public static readonly Vector2 Down = new(0, -1);

        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public Vector2(Vector2 vector)
        {
            X = vector.X;
            Y = vector.Y;
        }

        public readonly float Length()
        {
            return Math2D.Sqrt(X * X + Y * Y);
        }

        public readonly Vector2 Normalize()
        {
            var length = Length();
            if (length == 0)
                return Zero;

            return this / length;
        }

        public readonly override string ToString()
        {
            return $"X: {X}, Y: {Y}";
        }

        #region Operations

        public readonly Vector2 Rotate(float angle, bool radian = true)
        {
            angle = radian ? angle : Math2D.ToRadians(angle);

            var x = Math2D.Cos(angle) * X - Math2D.Sin(angle) * Y;
            var y = Math2D.Sin(angle) * X + Math2D.Cos(angle) * Y;

            return new Vector2(x, y);
        }

        public readonly Vector2 RotateAround(Vector2 origin, float angle, bool radian = true)
        {
            var rotatedVector = (this - origin).Rotate(angle, radian);
            return rotatedVector + origin;
        }

        public static float Distance(Vector2 first, Vector2 second)
        {
            var delta = first - second;
            return delta.Length();
        }

        public static float DotProduct(Vector2 first, Vector2 second)
        {
            return first.X * second.X + first.Y * second.Y;
        }

        public static float CrossProduct(Vector2 first, Vector2 second)
        {
            return first.X * second.Y - first.Y * second.X;
        }

        public static Vector2 CrossProduct(Vector2 first, float scalar)
        {
            return new Vector2(scalar * first.Y, -scalar * first.X);
        }

        public static Vector2 CrossProduct(float scalar, Vector2 first)
        {
            return new Vector2(-scalar * first.Y, scalar * first.X);
        }

        public static Vector2 MultiplyComponentwise(Vector2 first, Vector2 second)
        {
            return new Vector2(first.X * second.X, first.Y * second.Y);
        }

        #endregion

        #region Operators

        public static Vector2 operator +(Vector2 first, Vector2 second)
        {
            return new Vector2(first.X + second.X, first.Y + second.Y);
        }

        public static Vector2 operator -(Vector2 first, Vector2 second)
        {
            return new Vector2(first.X - second.X, first.Y - second.Y);
        }

        public static Vector2 operator -(Vector2 vector)
        {
            return new Vector2(-vector.X, -vector.Y);
        }

        public static Vector2 operator *(Vector2 vector, float multiplier)
        {
            return new Vector2(vector.X * multiplier, vector.Y * multiplier);
        }

        public static Vector2 operator *(float multiplier, Vector2 vector)
        {
            return vector * multiplier;
        }

        public static Vector2 operator /(Vector2 vector, float divider)
        {
            if (divider == 0)
                throw new InvalidOperationException($"{nameof(divider)} can't be zero.");

            return new Vector2(vector.X / divider, vector.Y / divider);
        }

        #endregion
    }
}