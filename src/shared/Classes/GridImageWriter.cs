using System;
using System.Collections.Generic;
using Shared.Grid;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
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
            var (xRange, yRange) = Area2d.Create(_map.Keys);
            var (xOffset, yOffset) = CalculateOffsets(xRange.Min, yRange.Min);

            var imageWidth = xRange.Max - xOffset + 1;
            var imageHeight = yRange.Max - yOffset + 1;
            var image = new Image<Rgba32>(imageWidth, imageHeight);

            image.ProcessPixelRows(pixelAccessor =>
            {
                for (var y = 0; y < imageHeight; y++)
                {
                    var pixelRowSpan = pixelAccessor.GetRowSpan(y);

                    for (var x = 0; x < imageWidth; x++)
                    {
                        var point = new Point2d(
                            x + xOffset,
                            y + yOffset);
                        var offsetX = x - xOffset;

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
