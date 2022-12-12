﻿using Shared.Graph;

namespace DayTwelve2022;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2022, 12, 12), "Hill Climbing Algorithm");

    public void PartOne(IInput input, IOutput output)
    {
        var (start, end, map) = input.ParseMap();
        var graph = map.ToGridGraph(IsValidNeigbour);

        var steps = Algorithms
            .AStarSearch(graph, start, end, Point2d.ManhattanDistance, false);

        output.WriteProperty("Number of steps to target", steps.Length);
    }

    public void PartTwo(IInput input, IOutput output)
    {
        var (_, end, map) = input.ParseMap();
        var graph = map.ToGridGraph(IsValidNeigbour);

        var candidates = map
            .Where(kvp => kvp.Value is 'a')
            .Select(kvp => kvp.Key)
            .ToArray();

        var best = candidates
            .Select(point =>
            {
                return Algorithms
                    .AStarSearch(graph, point, end, Point2d.ManhattanDistance, false);
            })
            .Where(steps => steps.Length > 0)
            .MinBy(steps => steps.Length);

        output.WriteProperty("Number of steps to target", best.Length);
    }

    private static bool IsValidNeigbour(Point2d current, char currentValue, Point2d neighbour, char neighbourValue)
        => neighbourValue <= currentValue + 1;
}

internal static class ParseExtensions
{
    public static HeightMap ParseMap(this IInput input)
    {
        var builder = ImmutableDictionary.CreateBuilder<Point2d, char>();
        builder.AddRange(input.As2DPoints().Select(x => KeyValuePair.Create(x.Point, x.Character)));

        var start = builder.Single(kvp => kvp.Value is 'S').Key;
        var end = builder.Single(kvp => kvp.Value is 'E').Key;

        builder[start] = 'a';
        builder[end] = 'z';

        return new HeightMap(start, end, builder.ToImmutable());
    }
}

readonly record struct HeightMap(
    Point2d Start,
    Point2d End,
    ImmutableDictionary<Point2d, char> Map);
