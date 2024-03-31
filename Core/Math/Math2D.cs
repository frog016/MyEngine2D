namespace MyEngine2D.Core.Math
{
    public static class Math2D
    {
        public const float PI = (float)System.Math.PI;

        public static float ToRadians(float angleDegrees)
        {
            return angleDegrees * PI / 180;
        }

        public static float ToDegrees(float angleRadians)
        {
            return angleRadians * 180 / PI;
        }

        public static float Sqrt(float value)
        {
            return (float)System.Math.Sqrt(value);
        }

        public static float Sqr(float value)
        {
            return (float)System.Math.Pow(value, 2);
        }

        public static float Min(float first, float second)
        {
            return System.Math.Min(first, second);
        }

        public static float Max(float first, float second)
        {
            return System.Math.Max(first, second);
        }

        public static float Abs(float value)
        {
            return System.Math.Abs(value);
        }

        public static float Clamp(float value, float min, float max)
        {
            return System.Math.Clamp(value, min, max);
        }

        public static int Clamp(int value, int min, int max)
        {
            return System.Math.Clamp(value, min, max);
        }

        public static float Sin(float angle)
        {
            return (float)System.Math.Sin(angle);
        }

        public static float Cos(float angle)
        {
            return (float)System.Math.Cos(angle);
        }

        public static int RoundToInt(float value)
        {
            return (int)System.Math.Round(value);
        }
    }
}
