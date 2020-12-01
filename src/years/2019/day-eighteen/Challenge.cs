﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NodaTime;
using Shared;

namespace DayEighteen2019
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2019, 12, 18), "Many-Worlds Interpretation");

        public override void PartOne(IInput input, IOutput output)
        {
            var map = input.Parse();
            var allKeys = map.Cells.Values.OfType<Key>().ToImmutableArray();
            var start = map.Cells.Values.Single(cell => cell is { CellType: CellType.Entrance });

            var overallPath = FindPathToAllKeys(map, start, allKeys);

            output.WriteProperty("Number of Steps", overallPath.Length.ToString());
        }

        public override void PartTwo(IInput input, IOutput output)
        {
        }

        private static ImmutableArray<Point> FindPathToAllKeys(Map map, Cell start, ImmutableArray<Key> allKeys)
        {
            var overallPath = ImmutableArray.CreateBuilder<Point>();

            var keyChain = ImmutableArray<char>.Empty;
            bool hasAllKeys = false;

            while (hasAllKeys is false)
            {
                var missingKeys = allKeys.Where(k => keyChain.Contains(k.Id) is false);
                if (missingKeys.Any() is false)
                {
                    hasAllKeys = true;
                    break;
                }

                var func = BuildShortestPathToKeyFunction(map, start, keyChain);

                var (key, keyPath) = missingKeys
                    .Select(key => (Key: key, Path: func(key)))
                    .Where(kvp => kvp.Path.Length > 0)
                    .OrderBy(kvp => kvp.Path.Length)
                    .First();

                keyChain = keyChain.Add(key.Id);
                overallPath.AddRange(keyPath);
                start = key;
            }

            return overallPath.ToImmutable();
        }

        private static Func<Cell, Point[]> BuildShortestPathToKeyFunction(Map map, Cell start, ImmutableArray<char> keyChain)
        {
            var previous = new Dictionary<Cell, Cell>();
            var queue = new Queue<Cell>();
            queue.Enqueue(start);

            while (queue.Count > 0)
            {
                var vertex = queue.Dequeue();

                var neighbours = map.Adjacent(vertex, keyChain);
                foreach (var neighbour in neighbours)
                {
                    if (previous.ContainsKey(neighbour))
                        continue;

                    previous[neighbour] = vertex;
                    queue.Enqueue(neighbour);
                }
            }

            return (target) =>
            {
                if (previous.ContainsKey(target) is false)
                {
                    return Array.Empty<Point>();
                }

                var path = new List<Cell>();

                var current = target;
                while (current != start)
                {
                    path.Add(current);
                    current = previous[current];
                }

                path.Reverse();
                return path.Select(p => p.Point).ToArray();
            };
        }
    }

    enum CellType { Wall, Key, Door, Entrance, Empty }

    record Point(int X, int Y)
    {
        public static Point operator +(Point left, Point right)
            => new(left.X + right.X, left.Y + right.Y);

        public static Point operator +(Point point, Direction direction) => direction.DirectionType switch
        {
            DirectionType.North => point + (0, 1),
            DirectionType.East => point + (1, 0),
            DirectionType.South => point + (0, -1),
            DirectionType.West => point + (-1, 0),
            _ => point
        };

        public static implicit operator Point((int X, int Y) i) => new(i.X, i.Y);
    }

    record Cell(CellType CellType, Point Point);

    record Key(char Id, Point Point) : Cell(CellType.Key, Point);

    record Door(char DoorId, char KeyId, Point Point) : Cell(CellType.Door, Point);
}
