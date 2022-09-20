using System;

namespace Shared
{
    public static class MathHelper
    {
        public static bool IsPrime(int number)
        {
            if (number <= 1) return false;
            if (number == 2) return true;
            if (number % 2 == 0) return false;

            var boundary = (int)Math.Floor(Math.Sqrt(number));

            for (var i = 3; i <= boundary; i += 2)
            {
                if (number % i == 0)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Find the Greatest common divisor between two numbers
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static long GreatestCommonDivisor(long a, long b)
        {
            return b == 0 ? a : GreatestCommonDivisor(b, a % b);
        }

        /// <summary>
        /// Returns the lowest common multiple of two numbers
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static long LowestCommonMultiple(long a, long b)
        {
            return a * b / GreatestCommonDivisor(a, b);
        }
    }
}
