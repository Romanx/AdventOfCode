using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using NodaTime;
using Shared;
using Shared.Graph;
using Shared.Helpers;

namespace DayFourteen2017
{
    public class Challenge : ChallengeSync
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2017, 12, 14), "Disk Defragmentation");

        public override void PartOne(IInput input, IOutput output)
        {
            var key = input.Content.AsString();

            var used = 0;
            for (var y = 0; y < 128; y++)
            {
                var row = GenerateRow(key, y);
                used += row.Count(cell => cell.Type is CellType.Used);
            }

            output.WriteProperty("Used Memory", used);
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var key = input.Content.AsString();
            var graph = Build(key);

            var usedCells = graph.Grid
                .Where(c => c.Value is CellType.Used)
                .Select(c => c.Key)
                .ToImmutableHashSet();

            var regions = 0;
            while (usedCells.Count > 0)
            {
                var cell = usedCells.First();

                var connected = graph.FindConnected(cell);
                regions++;

                usedCells = usedCells.Except(connected);
            }

            output.WriteProperty("Connected Regions", regions);

            static Graph Build(string key)
            {
                var grid = ImmutableDictionary.CreateBuilder<Point2d, CellType>();

                for (var y = 0; y < 128; y++)
                {
                    var row = GenerateRow(key, y);
                    grid.AddRange(row.Select(row => KeyValuePair.Create(row.Point, row.Type)));
                }

                return new Graph(grid.ToImmutable());
            }
        }

        private static IEnumerable<(Point2d Point, CellType Type)> GenerateRow(string key, int y)
        {
            var hash = KnotHasher.Hash(Encoding.ASCII.GetBytes($"{key}-{y}"));
            var hex = Convert.ToHexString(hash);
            var binary = BinaryHelpers.HexStringToBinary(hex);

            for (var x = 0; x < binary.Length; x++)
            {
                var c = binary[x];
                var point = new Point2d(x, y);
                yield return c switch
                {
                    '0' => (point, CellType.Available),
                    '1' => (point, CellType.Used),
                    _ => throw new NotImplementedException(),
                };
            }
        }
    }

    public static class BinaryHelpers
    {
        private static readonly Dictionary<char, string> hexCharacterToBinary = new()
        {
            { '0', "0000" },
            { '1', "0001" },
            { '2', "0010" },
            { '3', "0011" },
            { '4', "0100" },
            { '5', "0101" },
            { '6', "0110" },
            { '7', "0111" },
            { '8', "1000" },
            { '9', "1001" },
            { 'A', "1010" },
            { 'B', "1011" },
            { 'C', "1100" },
            { 'D', "1101" },
            { 'E', "1110" },
            { 'F', "1111" }
        };

        public static string HexStringToBinary(string hex)
        {
            return string.Create(hex.Length * 4, hex, static (span, state) =>
            {
                var remaining = span;

                foreach (var c in state)
                {
                    hexCharacterToBinary[c].AsSpan().CopyTo(remaining);
                    remaining = remaining[4..];
                }
            });
        }
    }

    enum CellType
    {
        [Display(Name = "#")]
        Used,
        [Display(Name = ".")]
        Available
    }

    class Graph : IGraph<Point2d>
    {
        public Graph(ImmutableDictionary<Point2d, CellType> grid)
        {
            Grid = grid;
        }

        public ImmutableDictionary<Point2d, CellType> Grid { get; }

        public IEnumerable<Point2d> Neigbours(Point2d node)
        {
            foreach (var neighbour in PointHelpers.GetDirectNeighbours(node))
            {
                if (Grid.TryGetValue(neighbour, out var type) && type is CellType.Used)
                {
                    yield return neighbour;
                }
            }
        }
    }
}
