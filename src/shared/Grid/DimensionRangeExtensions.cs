namespace Shared.Grid
{
    public static class DimensionRangeExtensions
    {
        public static DimensionRange Pad(this DimensionRange range, int value)
        {
            return range with
            {
                Min = range.Min - value < 0
                    ? range.Min
                    : range.Min - value,
                Max = range.Max + value,
            };
        }
    }
}
