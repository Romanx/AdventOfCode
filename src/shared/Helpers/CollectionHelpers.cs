using System.Collections.Generic;
using System.Collections.Immutable;

namespace Shared.Helpers
{
    public static class CollectionHelpers
    {
        public static ImmutableArray<ImmutableArray<T>> FindSubsetsOfSize<T>(T[] source, int subsetSize)
        {
            var allSubsets = ImmutableArray.CreateBuilder<ImmutableArray<T>>();
            FindSubsets(source, subsetSize, 0, new List<T>(), allSubsets);

            return allSubsets.ToImmutable();

            static void FindSubsets(T[] source, int subsetSize, int currentIndex, List<T> current, ImmutableArray<ImmutableArray<T>>.Builder allSubsets)
            {
                if (current.Count == subsetSize)
                {
                    allSubsets.Add(current.ToImmutableArray());
                    return;
                }

                for (var i = currentIndex; i < source.Length; i++)
                {
                    var item = source[i];
                    current.Add(item);
                    FindSubsets(source, subsetSize, i + 1, current, allSubsets);
                    current.RemoveAt(current.Count - 1);
                }
            }
        }
    }
}
