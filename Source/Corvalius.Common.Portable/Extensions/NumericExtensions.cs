using System;

namespace Corvalius
{
    public static class NumericExtensions
    {
        public static bool IsZero(this float value, float precision = float.Epsilon)
        {
            return Math.Abs(value) <= precision;
        }

        public static bool IsZero(this double value, double precision = double.Epsilon)
        {
            return Math.Abs(value) <= precision;
        }

        public static bool AlmostEquals(this double double1, double double2, double precision)
        {
            return (Math.Abs(double1 - double2) <= precision);
        }

        public static bool AlmostEquals(this float float1, float float2, float precision)
        {
            return (Math.Abs(float1 - float2) <= precision);
        }

        public static bool AlmostEquals(this float float1, float float2, double precision)
        {
            return (Math.Abs(float1 - float2) <= precision);
        }
    }
}