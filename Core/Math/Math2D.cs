namespace MyEngine2D.Core.Math
{
    public static class Math2D
    {
        public static float Sqrt(float value)
        {
            return (float)System.Math.Sqrt(value);
        }

        public static float Clamp(float value, float min, float max)
        {
            return System.Math.Clamp(value, min, max);
        }

        public static int Clamp(int value, int min, int max)
        {
            return System.Math.Clamp(value, min, max);
        }
    }
}
