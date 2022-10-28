using Shared.Grid;

namespace System.Collections.Immutable
{
    public static class CollectionExtensions
    {
        public static void Deconstruct(this ImmutableArray<DimensionRange> values, out DimensionRange Item1, out DimensionRange Item2)
        {
            Item1 = values[0];
            Item2 = values[1];
        }

        public static void Deconstruct(this ImmutableArray<DimensionRange> values, out DimensionRange Item1, out DimensionRange Item2, out DimensionRange Item3)
        {
            Item1 = values[0];
            Item2 = values[1];
            Item3 = values[2];
        }

        public static void Deconstruct(this ImmutableArray<DimensionRange> values, out DimensionRange Item1, out DimensionRange Item2, out DimensionRange Item3, out DimensionRange Item4)
        {
            Item1 = values[0];
            Item2 = values[1];
            Item3 = values[2];
            Item4 = values[3];
        }
    }
}
