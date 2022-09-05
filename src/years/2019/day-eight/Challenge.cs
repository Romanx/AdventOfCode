using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Spectre.Console;
using Color = SixLabors.ImageSharp.Color;

namespace DayEight2019;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2019, 12, 8), "Space Image Format");

    public void PartOne(IInput input, IOutput output)
    {
        var image = input.Parse();

        var layer = image.Layers
            .ToArray()
            .Select(layer =>
            {
                var result = new int[] { 0, 0, 0 };
                foreach (var value in layer.Pixels.Span)
                {
                    result[value] = result[value] + 1;
                }

                return result;
            })
            .MinBy(results => results[0])!;

        var ones = layer[1];
        var twos = layer[2];

        output.WriteProperty("One Digits", ones);
        output.WriteProperty("Two Digits", twos);
        output.WriteProperty("Results", ones * twos);
    }

    public void PartTwo(IInput input, IOutput output)
    {
        var image = input.Parse();
        var sqashed = image.Squash();

        using var outImage = new Image<Rgba32>(sqashed.Width, sqashed.Height);
        var layer = sqashed.Layers[0];

        for (var y = 0; y < sqashed.Height; y++)
        {
            for (var x = 0; x < sqashed.Width; x++)
            {
                outImage[x, y] = layer.Pixels.Span[y, x] switch
                {
                    0 => Color.Black,
                    1 => Color.White,
                    2 => Color.Transparent,
                    _ => throw new NotImplementedException(),
                };
            }
        }

        output.WriteImage(outImage);
    }
}

internal static class ParseExtensions
{
    public static SpaceImage Parse(this IInput input)
        => SpaceImage.Parse(input.Content.AsSpan(), 25, 6);
}
