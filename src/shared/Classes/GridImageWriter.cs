using System.Collections.Generic;
using Shared.Grid;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Shared
{
    public sealed class GridImageWriter : GridImageWriter<Color>
    {
        public GridImageWriter(IReadOnlyDictionary<Point2d, Color> map)
            : base(map)
        {
        }

        protected override Color GetColorForPoint(Point2d point)
            => _map.TryGetValue(point, out var color)
                ? color
                : Color.Black;
    }

    public abstract class GridImageWriter<T>
    {
        protected readonly IReadOnlyDictionary<Point2d, T> _map;

        public GridImageWriter(IReadOnlyDictionary<Point2d, T> map)
        {
            _map = map;
        }

        public Image Generate(int scale = 8)
        {
            var (xRange, yRange) = Area2d.Create(_map.Keys);
            var (xOffset, yOffset) = CalculateOffsets(xRange.Min, yRange.Min);

            var imageWidth = xRange.Max - xOffset + 1;
            var imageHeight = yRange.Max - yOffset + 1;
            var image = new Image<Rgba32>(imageWidth * scale, imageHeight * scale);

            image.ProcessPixelRows(pixelAccessor =>
            {
                for (var y = 0; y < pixelAccessor.Height; y++)
                {
                    var pixelRowSpan = pixelAccessor.GetRowSpan(y);
                    var yPoint = (y / scale) + yOffset;

                    for (var x = 0; x < pixelAccessor.Width; x++)
                    {
                        var xPoint = (x / scale) + xOffset;
                        var point = new Point2d(xPoint, yPoint);

                        pixelRowSpan[x] = GetColorForPoint(point);
                    }
                }
            });

            return image;

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

        protected abstract Color GetColorForPoint(Point2d point);
    }
}
