using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using MoreLinq;
using NodaTime;
using Shared;
using Spectre.Console;

namespace DayEighteen2018
{
    public class Challenge : ChallengeSync
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2018, 12, 18), "Settlers of The North Pole");

        public override void PartOne(IInput input, IOutput output)
        {
            var map = input.Parse();
            var file = output.File("output.txt");
            var writer = new StreamWriter(file, leaveOpen: true);

            writer.WriteLine("Initial state:");
            GridPrinter.Write(map, writer);
            writer.WriteLine();

            for (var i = 1; i <= 10; i++)
            {
                map = Scan.Step(map);
                writer.WriteLine($"After {i} minute{(i == 1 ? "" : "s")}:");
                GridPrinter.Write(map, writer);
                writer.WriteLine();
            }
            writer.Flush();

            PrintValue(output, map);
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            const int Target = 1_000_000_000;

            var map = input.Parse();
            var seen = new Dictionary<int, int>();
            var generation = 0;

            var firstRepeating = GrowTrees(map).SkipWhile(map =>
            {
                var hash = Scan.HashCode(map);
                generation++;
                if (seen.ContainsKey(hash))
                {
                    return false;
                }
                else
                {
                    seen[hash] = generation;
                    return true;
                }
            }).First();

            var hash = Scan.HashCode(firstRepeating);
            var cycleLength = generation - seen[hash];
            var remainingSteps = (Target - generation) % cycleLength;

            var targetGeneration = GrowTrees(firstRepeating).Skip(remainingSteps - 1).First();

            output.WriteProperty("Length of Cycle", cycleLength);
            PrintValue(output, targetGeneration);

            static IEnumerable<ImmutableDictionary<Point2d, CellType>> GrowTrees(ImmutableDictionary<Point2d, CellType> initial)
            {
                var current = initial;
                while (true)
                {
                    current = Scan.Step(current);
                    yield return current;
                }
            }
        }

        static void PrintValue(IOutput output, ImmutableDictionary<Point2d, CellType> map)
        {
            var counts = new Dictionary<CellType, int>(map.CountBy(v => v.Value));
            output.WriteProperty("Number of Lumberyards", counts[CellType.Lumberyard]);
            output.WriteProperty("Number of Wooded Acres", counts[CellType.Trees]);
            output.WriteProperty("Total Resource Value", counts[CellType.Lumberyard] * counts[CellType.Trees]);
        }

        static class Scan
        {
            public static ImmutableDictionary<Point2d, CellType> Step(ImmutableDictionary<Point2d, CellType> map)
            {
                var builder = map.ToBuilder();
                foreach (var (point, type) in map)
                {
                    builder[point] = CalculateNextCellType(type, GetNeighbours(point, map));
                }

                return builder.ToImmutable();

                static CellType CalculateNextCellType(CellType type, IEnumerable<(Point2d Position, CellType Type)> neighbours)
                {
                    var neighbourCounts = new Dictionary<CellType, int>(neighbours.CountBy(n => n.Type));
                    neighbourCounts.TryGetValue(CellType.Trees, out var treeCount);
                    neighbourCounts.TryGetValue(CellType.Lumberyard, out var lumberyardCount);

                    if (type is CellType.OpenAcre)
                    {
                        return treeCount >= 3
                            ? CellType.Trees
                            : CellType.OpenAcre;
                    }
                    else if (type is CellType.Trees)
                    {
                        return lumberyardCount >= 3
                            ? CellType.Lumberyard
                            : CellType.Trees;
                    }
                    else if (type is CellType.Lumberyard)
                    {
                        return lumberyardCount >= 1 && treeCount >= 1
                            ? CellType.Lumberyard
                            : CellType.OpenAcre;
                    }

                    throw new InvalidOperationException($"Not sure how we got here, bad type? {type}");
                }

                static IEnumerable<(Point2d Position, CellType type)> GetNeighbours(Point2d point, ImmutableDictionary<Point2d, CellType> map)
                {
                    foreach (var neighbour in point.GetAllNeighbours())
                    {
                        if (map.TryGetValue(neighbour, out var type))
                        {
                            yield return (neighbour, type);
                        }
                    }
                }
            }

            public static int HashCode(ImmutableDictionary<Point2d, CellType> map)
            {
                HashCode hashCode = default;
                foreach ((var key, var value) in map)
                {
                    hashCode.Add(key);
                    hashCode.Add(value);
                }

                return hashCode.ToHashCode();
            }
        }
    }

    enum CellType
    {
        [Display(Name = ".")]
        OpenAcre,

        [Display(Name = "|")]
        Trees,

        [Display(Name = "#")]
        Lumberyard
    }

    internal static class ParseExtensions
    {
        public static ImmutableDictionary<Point2d, CellType> Parse(this IInput input)
        {
            var builder = ImmutableDictionary.CreateBuilder<Point2d, CellType>();
            var points = input.As2DPoints();
            foreach ((var point, var c) in points)
            {
                builder.Add(point, EnumHelpers.FromDisplayName<CellType>($"{c}"));
            }

            return builder.ToImmutable();
        }
    }
}
