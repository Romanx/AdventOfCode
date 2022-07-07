using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;

namespace Shared
{
    public static class CollectionHelpers
    {
        /// <summary>
        /// Merges any overlapping and contigious ranges
        /// </summary>
        public static ICollection<NumberRange> MergeOverlapping(this IList<NumberRange> ranges)
        {
            if (ranges.Count == 0)
            {
                return ranges;
            }

            var arr = ranges.ToArray();
            Array.Sort(arr, Compare);

            var stack = new Stack<NumberRange>();
            stack.Push(arr[0]);

            for (var i = 0; i < arr.Length; i++)
            {
                var top = stack.Peek();

                // If the current intervals end is just after our current. Merge
                if (top.End + 1 == arr[i].Start)
                {
                    var next = new NumberRange(top.Start, arr[i].End);
                    stack.Pop();
                    stack.Push(next);
                }
                // if current interval is not overlapping with stack top,
                // Push it to the stack
                else if (top.End < arr[i].Start)
                {
                    stack.Push(arr[i]);
                }
                else if (top.End < arr[i].End)
                {
                    var next = new NumberRange(top.Start, arr[i].End);
                    stack.Pop();
                    stack.Push(next);
                }
            }

            return stack.Reverse().ToArray();

            static int Compare(NumberRange left, NumberRange right)
            {
                if (left.Start == right.Start)
                {
                    return left.End.CompareTo(right.End);
                }

                return left.Start.CompareTo(right.Start);
            }
        }

        public static LinkedListNode<T> At<T>(this LinkedList<T> list, int index)
        {
            if (index < 0 || index >= list.Count)
                throw new InvalidOperationException("Outside of list!");

            var current = list.First!;
            for (var i = 0; i < index; i++)
            {
                current = current.Next!;
            }

            return current!;
        }

        public static bool None<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            return !source.Any(predicate);
        }

        public static IExtremaEnumerable<TSource> MinBySet<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
            => MoreLinq.Extensions.MinByExtension.MinBy(source, selector);

        public static IExtremaEnumerable<TSource> MaxBySet<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
            => MoreLinq.Extensions.MaxByExtension.MaxBy(source, selector);
    }
}
