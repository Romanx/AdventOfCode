using System;

namespace Shared
{
    public interface IPoint
    {
        public static abstract int DimensionCount { get; }

        public void GetDimensions(Span<int> dimensions);

        int this[int index]
        {
            get;
        }
    }
}
