using System;
using System.Collections.Generic;

namespace Shared.Helpers
{
    public static class ArrayHelpers
    {
        public static T[,] CreateRectangularArray<T>(this IList<T[]> arrays)
        {
            // TODO: Validation and special-casing for arrays.Count == 0
            var minorLength = arrays[0].Length;
            var ret = new T[arrays.Count, minorLength];
            for (int i = 0; i < arrays.Count; i++)
            {
                var array = arrays[i];
                if (array.Length != minorLength)
                {
                    throw new ArgumentException
                        ("All arrays must be the same length");
                }
                for (var j = 0; j < minorLength; j++)
                {
                    ret[i, j] = array[j];
                }
            }
            return ret;
        }
    }
}
