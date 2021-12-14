using System.Text;
using Microsoft.Toolkit.HighPerformance;
using Shared.Grid;
using Spectre.Console;

namespace DayThirteen2021;

public class Challenge : ChallengeSync
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2021, 12, 13), "Transparent Origami");

    public override void PartOne(IInput input, IOutput output)
    {
        var (paper, folds) = input.Lines.Parse();

        var after1 = ApplyFold(paper, folds[0]);

        var count = 0;
        foreach (var c in after1.AsSpan())
        {
            if (c is '#')
                count++;
        }

        output.WriteProperty("Number of dots", count);
    }

    public override void PartTwo(IInput input, IOutput output)
    {
        var (paper, folds) = input.Lines.Parse();

        var result = folds.Aggregate(paper, (paper, fold) => ApplyFold(paper, fold));

        output.WriteBlock(() =>
        {
            return new Panel(Print(result))
            {
                Header = new("After Folding")
            };
        });
    }

    static char[,] ApplyFold(char[,] input, Fold fold)
    {
        var span = input.AsSpan2D();

        return fold.Axis switch
        {
            Axis.X => FoldX(span, fold.Value),
            Axis.Y => FoldY(span, fold.Value),
            _ => throw new InvalidOperationException("Unhandled type of fold!")
        };

        static char[,] FoldY(Span2D<char> input, int foldValue)
        {
            var result = new char[foldValue, input.Width];
            var resultSpan = result.AsSpan2D();

            var top = input[..foldValue, ..];
            var bottom = ArrayHelpers.FlipVertical(input[(foldValue + 1).., ..]);

            for (var y = 0; y < resultSpan.Height; y++)
            {
                var row = resultSpan.GetRowSpan(y);
                for (var x = 0; x < row.Length; x++)
                {
                    row[x] = top[y, x] == '#' || bottom[y, x] == '#'
                        ? '#'
                        : '.';
                }
            }

            return result;
        }

        static char[,] FoldX(Span2D<char> input, int foldValue)
        {
            var result = new char[input.Height, foldValue];
            var resultSpan = result.AsSpan2D();

            var left = input[.., ..foldValue];
            var right = ArrayHelpers.FlipHorizontal(input[.., (foldValue + 1)..]);

            for (var y = 0; y < resultSpan.Height; y++)
            {
                var row = resultSpan.GetRowSpan(y);
                for (var x = 0; x < row.Length; x++)
                {
                    row[x] = left[y, x] == '#' || right[y, x] == '#'
                        ? '#'
                        : '.';
                }
            }

            return result;
        }
    }

    static string Print(char[,] grid)
    {
        var builder = new StringBuilder();

        for (var y = 0; y < grid.GetLength(0); y++)
        {
            for (var x = 0; x < grid.GetLength(1); x++)
            {
                builder.Append(grid[y, x]);
            }

            builder.AppendLine();
        }

        return builder.ToString().Trim();
    }
}

internal static class ParseExtensions
{
    private static readonly PcreRegex regex = new(@"fold along (x|y)=(\d*)");

    public static (char[,] Points, ImmutableArray<Fold> Folds) Parse(this IInputLines lines)
    {
        var paragraphs = lines.AsParagraphs();

        var points = ImmutableHashSet.CreateBuilder<Point2d>();
        foreach (var line in paragraphs[0].Span)
        {
            points.Add(Point2d.Parse(new string(line.Span)));
        }

        var area = Area2d.Create(points);
        var grid = area.As2dArray<char>();
        var span = grid.AsSpan2D();
        span.Fill('.');

        foreach (var point in points)
        {
            span[point.Y, point.X] = '#';
        }

        var folds = ParseFolds(paragraphs[1].Span);

        return (grid, folds);

        static ImmutableArray<Fold> ParseFolds(ReadOnlySpan<ReadOnlyMemory<char>> paragraph)
        {
            var builder = ImmutableArray.CreateBuilder<Fold>();

            foreach (var line in paragraph)
            {
                var match = regex.Match(line.Span);
                var axis = Enum.Parse<Axis>(match.Groups[1].Value, true);
                var value = int.Parse(match.Groups[2].Value);

                builder.Add(new Fold(axis, value));
            }

            return builder.ToImmutable();
        }
    }
}

enum Axis
{
    X,
    Y
}


readonly record struct Fold(Axis Axis, int Value);
