using CommunityToolkit.HighPerformance;

namespace DayEight2019
{
    public readonly ref struct SpaceImage
    {
        private SpaceImage(ReadOnlySpan<ImageLayer> layers, int width, int height)
        {
            Layers = layers;
            Width = width;
            Height = height;
        }

        public ReadOnlySpan<ImageLayer> Layers { get; }
        public int Width { get; }
        public int Height { get; }

        public SpaceImage Squash()
        {
            Memory2D<int> layerPixels = new int[Height, Width];
            var pixelsSpan = layerPixels.Span;
            pixelsSpan.Fill(0);

            for (var i = Layers.Length - 1; i >= 0; i--)
            {
                var layer = Layers[i];
                for (var y = 0; y < Height; y++)
                {
                    for (var x = 0; x < Width; x++)
                    {
                        pixelsSpan[y, x] = layer.Pixels.Span[y, x] switch
                        {
                            0 => 0,
                            1 => 1,
                            2 => layerPixels.Span[y, x],
                            _ => throw new InvalidOperationException("Invalid Pixel Combination"),
                        };
                    }
                }
            }

            var layers = new[] { new ImageLayer(layerPixels) };
            return new SpaceImage(layers, Width, Height);
        }

        public static SpaceImage Parse(ReadOnlySpan<char> input, int width, int height)
        {
            var length = input.Length / (width * height);
            var layers = new ImageLayer[length];

            var count = 0;
            while (input.IsEmpty is false)
            {
                var layerPixels = new int[height, width];
                for (var y = 0; y < height; y++)
                {
                    for (var x = 0; x < width; x++)
                    {
                        layerPixels[y, x] = input[x + (y * width)] - '0';
                    }
                }

                layers[count] = new ImageLayer(layerPixels);
                input = input[(width * height)..];
                count++;
            }

            return new SpaceImage(layers, width, height);
        }
    }

    public readonly record struct ImageLayer(Memory2D<int> Pixels);
}
