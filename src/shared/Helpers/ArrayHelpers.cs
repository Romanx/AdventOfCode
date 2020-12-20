using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Toolkit.HighPerformance.Extensions;

namespace Shared.Helpers
{
    public static class ArrayHelpers
    {
        public static T[,] CreateRectangularArray<T>(this IList<T[]> arrays)
        {
            var minorLength = arrays[0].Length;
            var ret = new T[arrays.Count, minorLength];
            for (var i = 0; i < arrays.Count; i++)
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


        public static T[,] RotateRight<T>(T[,] array)
        {
            var size = array.GetLength(0);
            var rotated = new T[size, size];

            for (var i = 0; i < size; ++i)
            {
                for (var j = 0; j < size; ++j)
                {
                    rotated[i, j] = array[size - j - 1, i];
                }
            }

            return rotated;
        }

        public static T[,] FlipHorizontal<T>(T[,] array)
        {
            var columns = array.GetLength(0);
            var flipped = new List<T[]>(columns);

            for (var col = 0; col < columns; col++)
            {
                var row = array.GetRow(col);

                flipped.Add(row.ToArray().Reverse().ToArray());
            }

            return CreateRectangularArray(flipped);
        }
    }
}
