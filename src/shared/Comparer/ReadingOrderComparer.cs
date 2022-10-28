using System.Collections.Generic;
using System.Diagnostics;

namespace Shared
{
    public sealed class ReadingOrderComparer : IComparer<Point2d>
    {
        public static IComparer<Point2d> Instance { get; } = new ReadingOrderComparer();

        public int Compare(Point2d x, Point2d y)
        {
            return (x.Column, x.Row).CompareTo((y.Column, y.Row));
        }
    }
}
