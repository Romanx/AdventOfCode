using System.Text.RegularExpressions;
using MoreLinq;

namespace DayFourteen2020
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2020, 12, 14), "Docking Data");

        public void PartOne(IInput input, IOutput output)
        {
            var commands = input.ParseProgram();
            var memory = Decoder.DecodeCommandsV1(commands);
            var result = memory.Sum();

            output.WriteProperty("Sum of Memory Positions", result);
        }

        public void PartTwo(IInput input, IOutput output)
        {
            var commands = input.ParseProgram();
            var memory = Decoder.DecodeCommandsV2(commands);
            var result = memory.Sum();

            output.WriteProperty("Sum of Memory Positions", result);
        }
    }

    internal static class Decoder
    {
        public static ImmutableArray<long> DecodeCommandsV1(ImmutableArray<Command> commands)
        {
            var maskToOn = 0L;
            var maskToOff = 0L;

            var memory = new Dictionary<long, long>();

            foreach (var command in commands)
            {
                if (command is UpdateMask maskCommand)
                {
                    maskToOff = Convert.ToInt64(maskCommand.Mask.Replace('X', '0'), 2);
                    maskToOn = Convert.ToInt64(maskCommand.Mask.Replace('X', '1'), 2);
                }
                else if (command is WriteValueToMemory writeCommand)
                {
                    var value = (writeCommand.Value & maskToOn) | maskToOff;

                    memory[writeCommand.Position] = value;
                }
            }

            return memory.Values.ToImmutableArray();
        }

        public static ImmutableArray<long> DecodeCommandsV2(ImmutableArray<Command> commands)
        {
            string mask = string.Empty;

            var memory = new Dictionary<long, long>();

            foreach (var command in commands)
            {
                if (command is UpdateMask maskCommand)
                {
                    mask = maskCommand.Mask;
                }
                else if (command is WriteValueToMemory writeCommand)
                {
                    var baseMask = mask.Replace('X', '0');
                    var maskAsLong = Convert.ToInt64(baseMask, 2);
                    var masked = maskAsLong | writeCommand.Position;
                    var padded = Convert.ToString(masked, 2).PadLeft(36, '0');

                    var floatingPositions = mask.ToCharArray()
                        .Index()
                        .Where(c => c.Value == 'X')
                        .Select(f => f.Key)
                        .ToArray();

                    var positionMask = SetPositionsToValue(floatingPositions, padded, '0');

                    var subsets = floatingPositions
                        .Subsets();

                    foreach (var set in subsets)
                    {
                        var str = SetPositionsToValue(set, positionMask, '1');
                        var newPos = Convert.ToInt64(str, 2);
                        memory[newPos] = writeCommand.Value;
                    }
                }
            }

            return memory.Values.ToImmutableArray();

            static string SetPositionsToValue(IEnumerable<int> indexes, string mask, char value)
            {
                return string.Create(mask.Length, (indexes, value, mask), (c, state) =>
                {
                    var (indexes, val, mask) = state;
                    mask.AsSpan().CopyTo(c);
                    foreach (var index in indexes)
                    {
                        c[index] = val;
                    }
                });
            }
        }
    }

    internal static class ParseExtensions
    {
        internal static readonly Regex MemoryRegex = new("mem\\[(\\d+)\\] = (\\d+)");

        internal static ImmutableArray<Command> ParseProgram(this IInput input)
        {
            var builder = ImmutableArray.CreateBuilder<Command>();

            foreach (var line in input.Lines.AsMemory())
            {
                if (line.Span.Contains("mask = ", StringComparison.OrdinalIgnoreCase))
                {
                    builder.Add(new UpdateMask(line.Span[(line.Span.IndexOf('=') + 2)..].ToString()));
                }
                else
                {
                    var matched = MemoryRegex.Match(line.ToString());
                    builder.Add(new WriteValueToMemory(long.Parse(matched.Groups[1].Value), long.Parse(matched.Groups[2].Value)));
                }
            }

            return builder.ToImmutable();
        }
    }

    record Command();

    record UpdateMask(string Mask) : Command();

    record WriteValueToMemory(long Position, long Value) : Command();
}
