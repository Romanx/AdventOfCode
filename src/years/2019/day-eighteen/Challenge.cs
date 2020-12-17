using System;
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

            output.WriteProperty("Number of Steps", overallPath.Length);
        }

        public override void PartTwo(IInput input, IOutput output)
        {
        }

        private static ImmutableArray<Point2d> FindPathToAllKeys(Map map, Cell start, ImmutableArray<Key> allKeys)
        {
            var overallPath = ImmutableArray.CreateBuilder<Point2d>();

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

        private static Func<Cell, Point2d[]> BuildShortestPathToKeyFunction(Map map, Cell start, ImmutableArray<char> keyChain)
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
                    return Array.Empty<Point2d>();
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

    record Cell(CellType CellType, Point2d Point);

    record Key(char Id, Point2d Point) : Cell(CellType.Key, Point);

    record Door(char DoorId, char KeyId, Point2d Point) : Cell(CellType.Door, Point);
}
