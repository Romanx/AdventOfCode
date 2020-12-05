using System;

namespace Shared.Helpers
{
    public static class RangeExtensions
    {
        public static Range FirstHalf(this Range range)
            => new(range.Start, range.MidPoint());

        public static Range LastHalf(this Range range)
            => new(range.MidPoint() + 1, range.End);

        public static int MidPoint(this Range range)
            => (range.End.Value + range.Start.Value) / 2;
    }
}
