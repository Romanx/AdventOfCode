using System;
using System.Collections.Immutable;
using Shared.Grid;
using Spectre.Console;
using Color = Spectre.Console.Color;

namespace Shared
{
    public sealed class ConsoleImagePrinter : GridScanner
    {
        private readonly Color[,] array;
        private readonly Canvas canvas;

        private ConsoleImagePrinter(Color[,] array, Area2d area)
            : base(area)
        {
            this.array = array;
            canvas = new Canvas(this.array.GetLength(1), this.array.GetLength(0));
        }

        public static Canvas Print(Color[,] array)
        {
            var area = Area2d.Create(array);
            var printer = new ConsoleImagePrinter(array, area);
            printer.Scan();
            return printer.canvas;
        }

        public static Canvas Print(ImmutableDictionary<Point2d, Color> dict)
        {
            var (xRange, yRange) = Area2d.Create(dict.Keys);
            var (xOffset, yOffset) = CalculateOffsets(
                xRange.Min,
                yRange.Min);

            var imageWidth = xRange.Max - xOffset + 1;
            var imageHeight = yRange.Max - yOffset + 1;

            var area = Area2d.Create(imageWidth, imageHeight);
            var array = area.As2dArray<Color>();

            foreach (var (key, val) in dict)
            {
                var x = key.X - xOffset;
                var y = key.Y - yOffset;
                array[y, x] = val;
            }

            var printer = new ConsoleImagePrinter(array, area);
            printer.Scan();
            return printer.canvas;
        }

        public override void OnEndOfRow() {}

        public override void OnPosition(Point2d point)
        {
            canvas.SetPixel(point.X, point.Y, array[point.Y, point.X]);
        }

        static (int xOffset, int yOffset) CalculateOffsets(int xMin, int yMin)
        {
            var xOffset = xMin == 0
                ? 0
                : xMin;

            var yOffset = yMin == 0
                ? 0
                : yMin;

            return (xOffset, yOffset);
        }
    }
}
