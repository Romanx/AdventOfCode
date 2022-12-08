using System;
using System.Threading.Tasks;
using CommunityToolkit.HighPerformance;
using CommunityToolkit.HighPerformance.Helpers;
using Shared.Grid;

using static MoreLinq.Extensions.IndexExtension;

namespace DayEight2022;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2022, 12, 8), "Treetop Tree House");

    public void PartOne(IInput input, IOutput output)
    {
        var trees = input.ParseTrees();

        output.WriteProperty("Visible Trees", trees.Count(t => t.Visible));
    }

    public void PartTwo(IInput input, IOutput output)
    {
        var trees = input.ParseTrees();

        var bestTree = trees.MaxBy(t => t.ScenicScore);

        output.WriteProperty("Best Tree Scenic Score", bestTree.ScenicScore);
    }
}

internal static class ParseExtensions
{
    public static ImmutableArray<Tree> ParseTrees(this IInput input)
    {
        ReadOnlySpan2D<int> arr = input.Lines
            .As2DArray(i => i - '0')
            .AsSpan2D();

        var area = Area2d.Create(arr);

        var trees = ImmutableArray.CreateBuilder<Tree>(arr.Width * arr.Height);

        for (var y = 0; y < arr.Height; y++)
        {
            for (var x = 0; x < arr.Width; x++)
            {
                var point = new Point2d(x, y);
                var current = arr[y, x];

                var tree = CalculateTreeVisibility(point, current, area, arr);
                trees.Add(tree);
            }
        }

        return trees.MoveToImmutable();

        static Tree CalculateTreeVisibility(
            Point2d point,
            int current,
            Area2d area,
            ReadOnlySpan2D<int> arr)
        {
            if (point.X == 0 ||
                point.X == (arr.Width - 1) ||
                point.Y == 0 ||
                point.Y == (arr.Height - 1))
            {
                return new Tree(point, 0, true);
            }

            var scenicScore = 1;

            Span<bool> visibility = stackalloc bool[4];

            foreach (var (idx, direction) in Directions.CardinalDirections.Index())
            {
                visibility[idx] = true;

                var score = 0;
                var next = point + direction;
                while (area.Contains(next))
                {
                    var target = arr[next.Y, next.X];
                    score++;

                    if (target >= current)
                    {
                        visibility[idx] = false;
                        break;
                    }

                    next += direction;
                }

                scenicScore *= score;
            }

            return new Tree(point, scenicScore, visibility.Any(true));
        }
    }
}

readonly record struct Tree(
    Point2d Position,
    int ScenicScore,
    bool Visible);
