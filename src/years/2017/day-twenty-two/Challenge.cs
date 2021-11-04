using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NodaTime;
using Shared;
using Shared.Grid;

namespace DayTwentyTwo2017
{
    public class Challenge : ChallengeSync
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2017, 12, 22), "Sporifica Virus");

        public override void PartOne(IInput input, IOutput output)
        {
            const int Iterations = 10_000;

            var grid = input.ParseInfectedNodes()
                .ToDictionary(k => k.Key, v => v.Value);

            var virus = new Virus(Point2d.Origin, GridDirection.Up);
            var infections = 0;

            for (var i = 1; i <= Iterations; i++)
            {
                Direction direction;

                if (grid.GetValueOrDefault(virus.Position, TileState.Clean) == TileState.Clean)
                {
                    infections += 1;
                    grid[virus.Position] = TileState.Infected;
                    direction = virus.Direction.Left();
                }
                else
                {
                    grid[virus.Position] = TileState.Clean;
                    direction = virus.Direction.Right();
                }

                virus = virus with
                {
                    Direction = direction,
                    Position = virus.Position + direction
                };
            }

            output.WriteProperty($"Number of infections after {Iterations}", infections);
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            const int Iterations = 10_000_000;

            var grid = input.ParseInfectedNodes()
                .ToDictionary(k => k.Key, v => v.Value);
            var virus = new Virus(Point2d.Origin, GridDirection.Up);
            var infections = 0;

            for (var i = 1; i <= Iterations; i++)
            {
                var currentTile = grid.GetValueOrDefault(virus.Position, TileState.Clean);

                (TileState NextState, Func<Direction, Direction> DirectionFunc) thing = currentTile switch
                {
                    TileState.Clean => (TileState.Weakened, DirectionActions.TurnLeft),
                    TileState.Weakened => (TileState.Infected, DirectionActions.None),
                    TileState.Infected => (TileState.Flagged, DirectionActions.TurnRight),
                    TileState.Flagged => (TileState.Clean, DirectionActions.Reverse),
                    _ => throw new NotImplementedException(),
                };

                if (thing.NextState is TileState.Infected)
                {
                    infections++;
                }

                grid[virus.Position] = thing.NextState;
                var nextDirection = thing.DirectionFunc(virus.Direction);
                virus = virus with
                {
                    Direction = nextDirection,
                    Position = virus.Position + nextDirection
                };
            }

            output.WriteProperty($"Number of infections after {Iterations}", infections);
        }
    }

    enum TileState
    {
        Clean,
        Infected,
        Weakened,
        Flagged,
    }

    internal static class ParseExtensions
    {
        public static ImmutableDictionary<Point2d, TileState> ParseInfectedNodes(this IInput input)
        {
            var arr = input.Lines.As2DArray();
            var area = Area2d.Create(arr);
            var infected = ImmutableDictionary.CreateBuilder<Point2d, TileState>();

            foreach (var (original, updated) in area.Recentre(Point2d.Origin))
            {
                infected.Add(updated, arr[original.X, original.Y] switch
                {
                    '#' => TileState.Infected,
                    '.' => TileState.Clean,
                    _ => throw new NotImplementedException()
                });
            }

            return infected.ToImmutable();
        }
    }

    record Virus(Point2d Position, Direction Direction);

    public static class AreaExtensions
    {
        public static IEnumerable<(Point2d Original, Point2d New)> Recentre(this Area2d area, Point2d center)
        {
            var offset = area.Middle - center;

            foreach (var position in area.Items)
            {
                yield return (position, position - offset);
            }
        }
    }
}
