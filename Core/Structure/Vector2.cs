using MyEngine2D.Core.Math;

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

        public float Length()
        {
            return Math2D.Sqrt(X * X + Y * Y);
        }

        public Vector2 Normalize()
        {
            return this / Length();
        }

        public override string ToString()
        {
            return $"X: {X}, Y: {Y}";
        }

        #region Operations

        public static float ScalarProduct(Vector2 first, Vector2 second)
        {
            return first.X * second.X + first.Y * second.Y;
        }

        public static float CrossProduct(Vector2 first, Vector2 second)
        {
            return first.X * second.Y - first.Y * second.X;
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
