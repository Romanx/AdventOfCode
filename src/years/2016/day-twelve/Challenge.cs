using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NodaTime;
using PCRE;
using Shared;
using Spectre.Console;
using static Shared.AlphabetHelper;

namespace DayTwelve2016
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2016, 12, 12), "Leonardo's Monorail");

        public override void PartOne(IInput input, IOutput output)
        {
            var instructions = input.Parse();
            var program = Program.New()
                .RunInstructions(instructions);

            foreach (var (register, val) in program.Registers)
            {
                output.WriteProperty($"Register '{register}'", val);
            }
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var instructions = input.Parse();

            var registers = Program.Empty
                .SetItem('c', 1);

            var program = new Program(registers, 0)
                .RunInstructions(instructions);

            foreach (var (register, val) in program.Registers)
            {
                output.WriteProperty($"Register '{register}'", val);
            }
        }
    }

    record Program(ImmutableDictionary<char, int> Registers, int Pointer)
    {
        public static ImmutableDictionary<char, int> Empty { get; } = new Dictionary<char, int>
        {
            ['a'] = 0,
            ['b'] = 0,
            ['c'] = 0,
            ['d'] = 0
        }.ToImmutableDictionary();

        public static Program New() => new(Empty, 0);

        public Program RunInstructions(ImmutableArray<Instruction> instructions)
        {
            var program = this;

            while (program.Pointer < instructions.Length)
            {
                var instruction = instructions[program.Pointer];

                program = instruction.Apply(program);
            }

            return program;
        }
    }

    internal static class ParseExtensions
    {
        private static readonly PcreRegex regex = new(@"^(?<InstructionType>[a-z]{3}) (?<First>[a-z]+|-?[0-9]+)\s*(?<Second>[a-z]+|-?[0-9]+)?$");

        public static ImmutableArray<Instruction> Parse(this IInput input)
        {
            var builder = ImmutableArray.CreateBuilder<Instruction>();

            foreach (var line in input.AsLines())
            {
                var match = regex.Match(line.Span);

                var instruction = match.Groups["InstructionType"].Value;
                var first = match.Groups["First"].Value;

                if (instruction.Equals("cpy", StringComparison.OrdinalIgnoreCase))
                {
                    object value = int.TryParse(first, out var res)
                        ? res
                        : first[0].ToString();

                    var second = match.Groups["Second"].Value;

                    builder.Add(new Copy(second[0], value));
                }
                else if (instruction.Equals("inc", StringComparison.OrdinalIgnoreCase))
                {
                    builder.Add(new Increment(first[0]));
                }
                else if (instruction.Equals("dec", StringComparison.OrdinalIgnoreCase))
                {
                    builder.Add(new Decrement(first[0]));
                }
                else if (instruction.Equals("jnz", StringComparison.OrdinalIgnoreCase))
                {
                    object target = int.TryParse(first, out var res)
                        ? res
                        : first[0].ToString();

                    var second = match.Groups["Second"].Value;
                    builder.Add(new JumpNotZero(target, int.Parse(second)));
                }
                else
                {
                    throw new InvalidOperationException($"Unrecognised instruction '{instruction.ToString()}'");
                }
            }

            return builder.ToImmutable();
        }
    }
}
