using System.Text;
using Shared.Grid;

namespace DayTwenty2021;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2021, 12, 20), "Trench Map");

    public void PartOne(IInput input, IOutput output)
    {
        var (algorithm, image) = input.Lines.Parse();
        var refined = ApplyRefinements(2, image, algorithm);

        output.WriteProperty("Lit Pixels", refined.Points.Count);
    }

    public void PartTwo(IInput input, IOutput output)
    {
        var (algorithm, image) = input.Lines.Parse();
        var refined = ApplyRefinements(50, image, algorithm);

        output.WriteProperty("Lit Pixels", refined.Points.Count);
    }

    static Image ApplyRefinements(int number, Image input, ImageEnhancementAlgorithm algorithm)
    {
        var image = input;
        for (var i = 0; i < number; i++)
        {
            image = RefineImage(image, algorithm);
        }

        return image;

        static Image RefineImage(Image input, ImageEnhancementAlgorithm algorithm)
        {
            var newPoints = ImmutableHashSet.CreateBuilder<Point2d>();
            var newArea = input.Area.Pad(1);

            foreach (var point in newArea.Items)
            {
                var number = GetDecimalNumber(input, point);
                var lit = algorithm[number];
                if (lit)
                {
                    newPoints.Add(point);
                }
            }

            var newDefault = input.DefaultLitValue is false
                ? algorithm[0]
                : algorithm[^1];
            return new Image(newPoints.ToImmutable(), newDefault);
        }

        static int GetDecimalNumber(Image input, Point2d target)
        {
            var points = new[]
            {
            target + Direction.NorthWest,
            target + Direction.North,
            target + Direction.NorthEast,
            target + Direction.West,
            target,
            target + Direction.East,
            target + Direction.SouthWest,
            target + Direction.South,
            target + Direction.SouthEast,
        };

            Span<char> bits = new char[9];
            for (var i = 0; i < points.Length; i++)
            {
                var point = points[i];

                // If the original image doesn't contain this point then use the default lookup
                if (input.Area.Contains(point) is false)
                {
                    bits[i] = input.DefaultLitValue
                        ? '1'
                        : '0';
                }
                else
                {
                    bits[i] = input.Points.Contains(point)
                        ? '1'
                        : '0';
                }
            }

            return Convert.ToInt32(new string(bits), 2);
        }
    }
}

internal static class ParseExtensions
{
    public static (ImageEnhancementAlgorithm Algorithm, Image Image) Parse(this IInputLines lines)
    {
        var paragraphs = lines.AsParagraphs();

        var algorithm = new ImageEnhancementAlgorithm(paragraphs[0].Span[0]);
        var imageLines = paragraphs[1].Span;

        var image = ImmutableHashSet.CreateBuilder<Point2d>();

        for (var y = 0; y < imageLines.Length; y++)
        {
            var line = imageLines[y].Span;
            for (var x = 0; x < line.Length; x++)
            {
                if (line[x] is '#')
                {
                    image.Add(new Point2d(x, y));
                }
            }
        }

        return (algorithm, new Image(image.ToImmutable()));
    }
}

record Image
{
    public Image(ImmutableHashSet<Point2d> points, bool defaultLitValue = false)
    {
        Points = points;
        DefaultLitValue = defaultLitValue;
        Area = Area2d.Create(points);
    }

    public ImmutableHashSet<Point2d> Points { get; }

    public bool DefaultLitValue { get; }

    public Area2d Area { get; }

    internal string DisplayString()
    {
        var builder = new StringBuilder();
        var (xRange, yRange) = Area;

        for (var y = yRange.Min; y <= yRange.Max; y++)
        {
            for (var x = xRange.Min; x <= xRange.Max; x++)
            {
                builder.Append(Points.Contains((x, y)) switch
                {
                    true => '#',
                    false => '.',
                });
            }
            builder.AppendLine();
        }

        return builder.ToString().Trim();
    }
}

readonly record struct ImageEnhancementAlgorithm(ReadOnlyMemory<char> Input)
{
    public bool this[Index index] => Input.Span[index] is '#';
}
