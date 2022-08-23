using System.ComponentModel.DataAnnotations;
using MoreLinq;
using Spectre.Console;

namespace DayTwentyTwo2016
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2016, 12, 22), "Grid Computing");

        public void PartOne(IInput input, IOutput output)
        {
            var nodes = input.Parse();

            var count = ViablePairs(nodes).Count();

            output.WriteProperty("Viable Pairs", count);

            static IEnumerable<(Node A, Node B)> ViablePairs(ImmutableArray<Node> nodes)
            {

                foreach (var subsets in nodes.Subsets(2).SelectMany(s => s.Permutations()))
                {
                    var a = subsets[0];
                    var b = subsets[1];

                    if (a.Used != 0 && a.Used <= b.Available)
                    {
                        yield return (a, b);
                    }
                }
            }
        }

        public void PartTwo(IInput input, IOutput output)
        {
            var nodes = input.Parse();
            var map = nodes.ToImmutableDictionary(k => k.Position, v =>
            {
                if (v.Used == 0)
                    return Display.Empty;

                if (v.Used > 100)
                    return Display.Blocked;

                return Display.Valid;
            });

            output.WriteBlock(() =>
            {
                return new Panel(GridPrinter.Print(map));
            });

            var result = 23 + 25 + 23 + (36 * 5) + 1;

            output.WriteProperty("Moves", result);
        }
    }

    enum Display
    {
        [Display(Name = "_")]
        Empty,
        [Display(Name = ".")]
        Valid,
        [Display(Name = "#")]
        Blocked
    }

    internal static class ParseExtensions
    {
        private static readonly PcreRegex regex = new(@"\/dev\/grid\/node-x(?<X>[0-9]+)-y(?<Y>[0-9]+)\s*(?<Size>[0-9]+)T\s*(?<Used>[0-9]+)T\s*(?<Avail>[0-9]+)T\s*(?<Use>[0-9]+)%");

        public static ImmutableArray<Node> Parse(this IInput input)
        {
            var builder = ImmutableArray.CreateBuilder<Node>();

            foreach (var line in input.Lines.AsMemory().Skip(2))
            {
                builder.Add(ParseLine(line.Span));
            }

            return builder.ToImmutable();

            static Node ParseLine(ReadOnlySpan<char> line)
            {
                var match = regex.Match(line);

                return new Node(
                    new Point2d(int.Parse(match["X"]), int.Parse(match["Y"])),
                    uint.Parse(match["Size"]),
                    uint.Parse(match["Used"]),
                    uint.Parse(match["Avail"]),
                    byte.Parse(match["Use"]));
            }
        }
    }

    record Node(Point2d Position, uint Size, uint Used, uint Available, byte UsedPercentage);
}
