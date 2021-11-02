using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NodaTime;
using Shared;

namespace DayTwentyThree2015
{
    public class Challenge : ChallengeSync
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2015, 12, 23), "Opening the Turing Lock");

        public override void PartOne(IInput input, IOutput output)
        {
            var commands = input.Parse();
            var program = new Program(commands);
            var es = program.Run();

            output.WriteTable(es.Registers.ToDictionary(k => $"Register '{k.Key}'", v => $"{v.Value}"));
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var commands = input.Parse();
            var program = new Program(commands);
            var es = program.Run(new Dictionary<string, uint>
            {
                ["a"] = 1,
                ["b"] = 0,
            });

            output.WriteTable(es.Registers.ToDictionary(k => $"Register '{k.Key}'", v => $"{v.Value}"));
        }
    }

    internal static class ParseExtensions
    {
        public static ImmutableArray<Command> Parse(this IInput input)
        {
            var builder = ImmutableArray.CreateBuilder<Command>();

            foreach (var line in input.Lines.AsMemory())
            {
                builder.Add(ParseCommand(line.Span));
            }

            return builder.ToImmutableArray();
        }

        private static Command ParseCommand(ReadOnlySpan<char> span)
        {
            var command = span[..3];
            var rest = span[3..];

            if (command.SequenceEqual("hlf"))
            {
                return new Half(rest.Trim().ToString());
            }
            else if (command.SequenceEqual("tpl"))
            {
                return new Triple(rest.Trim().ToString());
            }
            else if (command.SequenceEqual("inc"))
            {
                return new Increment(rest.Trim().ToString());
            }
            else if (command.SequenceEqual("jmp"))
            {
                return new Jump(int.Parse(rest));
            }
            else if (command.SequenceEqual("jie"))
            {
                var before = rest[..rest.IndexOf(',')];
                var after = rest[(rest.IndexOf(',') + 1)..];

                return new JumpIfEven(before.Trim().ToString(), int.Parse(after));
            }
            else if (command.SequenceEqual("jio"))
            {
                var before = rest[..rest.IndexOf(',')];
                var after = rest[(rest.IndexOf(',') + 1)..];

                return new JumpIfOne(before.Trim().ToString(), int.Parse(after));
            }

            throw new NotImplementedException();
        }
    }

    record ExecutionState(int Pointer, ImmutableDictionary<string, uint> Registers)
    {
        public ExecutionState Next(ImmutableDictionary<string, uint> registers) => this with
        {
            Registers = registers,
            Pointer = Pointer + 1
        };

        public ExecutionState Next() => this with
        {
            Registers = Registers,
            Pointer = Pointer + 1
        };

        public ExecutionState SetPointer(int pointer) => this with
        {
            Registers = Registers,
            Pointer = pointer
        };
    }

    abstract record Command(string Name)
    {
        public abstract ExecutionState Run(ExecutionState state);
    }

    record Half(string Register) : Command("hlf")
    {
        public override ExecutionState Run(ExecutionState state)
        {
            var value = state.Registers[Register];
            return state.Next(state.Registers.SetItem(Register, value / 2));
        }
    }

    record Triple(string Register) : Command("tpl")
    {
        public override ExecutionState Run(ExecutionState state)
        {
            var value = state.Registers[Register];
            return state.Next(state.Registers.SetItem(Register, value * 3));
        }
    }

    record Increment(string Register) : Command("inc")
    {
        public override ExecutionState Run(ExecutionState state)
        {
            var value = state.Registers[Register];
            return state.Next(state.Registers.SetItem(Register, value + 1));
        }
    }

    record Jump(int Offset) : Command("jmp")
    {
        public override ExecutionState Run(ExecutionState state)
        {
            return state.SetPointer(state.Pointer + Offset);
        }
    }

    record JumpIfEven(string Register, int Offset) : Command("jie")
    {
        public override ExecutionState Run(ExecutionState state)
        {
            var value = state.Registers[Register];
            return value % 2 == 0
                ? state.SetPointer(state.Pointer + Offset)
                : state.Next();
        }
    }

    record JumpIfOne(string Register, int Offset) : Command("jio")
    {
        public override ExecutionState Run(ExecutionState state)
        {
            var value = state.Registers[Register];
            return value == 1
                ? state.SetPointer(state.Pointer + Offset)
                : state.Next();
        }
    }
}
