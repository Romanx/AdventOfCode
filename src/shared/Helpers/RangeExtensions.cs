namespace System
{
    public static class RangeExtensions
    {
        public static Range FirstHalf(this Range range)
            => new(range.Start, range.MidPoint());

        public static Range LastHalf(this Range range)
            => new(range.MidPoint() + 1, range.End);

        public static int MidPoint(this Range range)
            => (range.End.Value + range.Start.Value) / 2;

        public static bool Contains(this Range range, int value) => value >= range.Start.Value && value <= range.End.Value;
    }
}
