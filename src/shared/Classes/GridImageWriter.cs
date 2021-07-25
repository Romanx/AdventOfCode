using System;
using System.Collections.Generic;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Shared
{
    public abstract class GridImageWriter<T>
    {
        protected readonly IReadOnlyDictionary<Point2d, T> _map;

        public GridImageWriter(IReadOnlyDictionary<Point2d, T> map)
        {
            _map = map;
        }

        public Image Generate()
        {
            var dimensions = GridDimensions.Create(_map.Keys);
            var (xRange, yRange) = dimensions;
            var (xOffset, yOffset) = CalculateOffsets(xRange.Min, yRange.Min);

            var imageWidth = xRange.Max - xOffset + 1;
            var imageHeight = yRange.Max - yOffset + 1;
            var image = new Image<Rgba32>(imageWidth, imageHeight);

            for (var y = yRange.Min; y <= yRange.Max; y++)
            {
                Span<Rgba32> pixelRowSpan = image.GetPixelRowSpan(y - yOffset);

                for (var x = xRange.Min; x <= xRange.Max; x++)
                {
                    var point = new Point2d(x, y);
                    var offsetX = x - xOffset;

                    pixelRowSpan[offsetX] = GetColorForPoint(point);
                }
            }

            image.Mutate(x => x.Resize(image.Width * 10, image.Height * 10).Pixelate(10));

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
