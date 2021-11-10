using System.Text.RegularExpressions;

namespace DaySeven2015
{
    public class Challenge : ChallengeSync
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2015, 12, 7), "Some Assembly Required");

        public override void PartOne(IInput input, IOutput output)
        {
            var instructions = input.Parse();
            var calculator = new Calculator(instructions);

            var result = calculator.Calculate("a");
            output.WriteProperty("wire 'a'", result);
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var instructions = input.Parse();
            var calculator = new Calculator(instructions, new Dictionary<string, ushort>
            {
                ["b"] = 46065,
            });

            var result = calculator.Calculate("a");
            output.WriteProperty("wire 'a'", result);
        }
    }

    class Calculator
    {
        private readonly ImmutableDictionary<string, Instruction> _instructions;
        private readonly Dictionary<string, ushort> _cache;

        public Calculator(ImmutableDictionary<string, Instruction> instructions)
            : this(instructions, new Dictionary<string, ushort>())
        {
        }

        public Calculator(
            ImmutableDictionary<string, Instruction> instructions,
            Dictionary<string, ushort> cache)
        {
            _instructions = instructions;
            _cache = cache;
        }

        public ushort Calculate(string identifier)
        {
            if (ushort.TryParse(identifier, out var value))
            {
                return value;
            }

            if (_cache.TryGetValue(identifier, out var result))
            {
                return result;
            }

            var instruction = _instructions[identifier];

            return _cache[identifier] = instruction.Calculate(this);
        }
    }

    internal static class ParseExtensions
    {
        private static readonly Regex regex = new(@"(?<Left>[a-z0-9]*) (?<Action>AND|OR|LSHIFT|RSHIFT)+ (?<Right>[a-z0-9]*)");

        public static ImmutableDictionary<string, Instruction> Parse(this IInput input)
        {
            var builder = ImmutableDictionary.CreateBuilder<string, Instruction>();

            foreach (var line in input.Lines.AsString())
            {
                var split = line.Split("->", StringSplitOptions.TrimEntries);

                builder.Add(split[1], ParseInstruction(split[0]));
            }

            return builder.ToImmutable();

            static Instruction ParseInstruction(string value)
            {
                if (ushort.TryParse(value, out var signal))
                {
                    return new Value(signal);
                }

                var match = regex.Match(value);
                if (match.Success)
                {
                    return match.Groups["Action"].Value switch
                    {
                        "AND" => new And(match.Groups["Left"].Value, match.Groups["Right"].Value),
                        "OR" => new Or(match.Groups["Left"].Value, match.Groups["Right"].Value),
                        "LSHIFT" => new LShift(match.Groups["Left"].Value, int.Parse(match.Groups["Right"].Value)),
                        "RSHIFT" => new RShift(match.Groups["Left"].Value, int.Parse(match.Groups["Right"].Value)),
                        _ => throw new NotImplementedException(),
                    };
                }

                return value.Contains("NOT")
                    ? new Not(value.Split("NOT", StringSplitOptions.TrimEntries)[1])
                    : new Assignment(value);
            }
        }
    }

    abstract record Instruction()
    {
        public abstract ushort Calculate(Calculator calculator);
    }

    record Value(ushort Signal) : Instruction
    {
        public override ushort Calculate(Calculator calculator)
        {
            return Signal;
        }
    }

    record Assignment(string Identifier) : Instruction
    {
        public override ushort Calculate(Calculator calculator) => calculator.Calculate(Identifier);
    }

    record And(string Left, string Right) : Instruction
    {
        public override ushort Calculate(Calculator calculator)
            => (ushort)(calculator.Calculate(Left) & calculator.Calculate(Right));
    }

    record Or(string Left, string Right) : Instruction
    {
        public override ushort Calculate(Calculator calculator)
            => (ushort)(calculator.Calculate(Left) | calculator.Calculate(Right));
    }

    record LShift(string Identifier, int ShiftBy) : Instruction
    {
        public override ushort Calculate(Calculator calculator)
            => (ushort)(calculator.Calculate(Identifier) << ShiftBy);
    }

    record RShift(string Identifier, int ShiftBy) : Instruction
    {
        public override ushort Calculate(Calculator calculator)
            => (ushort)(calculator.Calculate(Identifier) >> ShiftBy);
    }

    record Not(string Identifier) : Instruction
    {
        public override ushort Calculate(Calculator calculator)
            => (ushort)~calculator.Calculate(Identifier);
    }
}
