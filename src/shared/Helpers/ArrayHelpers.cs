using System;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.HighPerformance;

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

        public static T[,] FlipHorizontal<T>(T[,] span)
            => FlipHorizontal(span.AsSpan2D());

        public static T[,] FlipHorizontal<T>(Span2D<T> span)
        {
            var flipped = new T[span.Height, span.Width];

            for (var col = 0; col < span.Height; col++)
            {
                var source = span.GetRowSpan(col);
                var target = flipped.GetRowSpan(col);

                source.CopyTo(target);
                target.Reverse();
            }

            return flipped;
        }

        public static Span2D<T> FlipVertical<T>(Span2D<T> span)
        {
            var flipped = new T[span.Height, span.Width];

            for (var col = span.Height - 1; col >= 0; col--)
            {
                var source = span.GetRowSpan(col);
                var target = flipped.GetRowSpan((span.Height - 1) - col);

                source.CopyTo(target);
            }

            return flipped;
        }
    }
}
