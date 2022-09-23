using System;

namespace Shared
{
    public static class IntExtensions
    {
        /// <summary>
        /// Returns the specific digit of a number read from right to left, this is without any sign.
        /// </summary>
        /// <param name="number">The number to read the digit from</param>
        /// <param name="index">The index of the number</param>
        /// <returns>The number at the specific index</returns>
        public static int DigitAt(this int number, int index)
        {
            var digits = NumberOfDigits(number);

            if (index > digits)
            {
                throw new InvalidOperationException($"'{number}' has '{digits}' digits so cannot get item at '{index}' index");
            }

            var modifier = index * 10;
            return Math.Abs(number % modifier);
        }

        public static int NumberOfDigits(int n) =>
            n == 0 ? 1 : (n > 0 ? 1 : 2) + (int)Math.Log10(Math.Abs((double)n));
    }
}
