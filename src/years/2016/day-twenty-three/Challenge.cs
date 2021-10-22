using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NodaTime;
using PCRE;
using Shared;

namespace DayTwentyThree2016
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2016, 12, 23), "Safe Cracking");

        public override void PartOne(IInput input, IOutput output)
        {
            var commands = AssembunnyParser.BuildParser()
                .ParseCommands(ApplyOptimizations(input.Content.AsString()))
                .ToImmutableArray();

            var registers = Program.EmptyRegisters;
            registers['a'] = 7;

            var program = new Program(commands, registers, 0)
                .Run();

            foreach (var (register, val) in program.Registers)
            {
                output.WriteProperty($"Register '{register}'", val);
            }
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var commands = AssembunnyParser.BuildParser()
                .ParseCommands(ApplyOptimizations(input.Content.AsString()))
                .ToImmutableArray();

            var registers = Program.EmptyRegisters;
            registers['a'] = 12;

            var program = new Program(commands, registers, 0)
                .Run();

            foreach (var (register, val) in program.Registers)
            {
                output.WriteProperty($"Register '{register}'", val);
            }
        }

        private static IEnumerable<ReadOnlyMemory<char>> ApplyOptimizations(string lines)
        {
            var replacements = new[]
            {
                (
                    Pattern: @$"cpy ([a-d]) ([a-d]){Environment.NewLine}inc ([a-d]){Environment.NewLine}dec \2{Environment.NewLine}jnz \2 -2{Environment.NewLine}dec ([a-d]){Environment.NewLine}jnz \4 -5",
                    Replacement: @$"mul $1 $4 $3{Environment.NewLine}cpy 0 $2{Environment.NewLine}cpy 0 $4{Environment.NewLine}nop{Environment.NewLine}nop{Environment.NewLine}nop"
                )
            };

            foreach (var (pattern, replacement) in replacements)
            {
                lines = PcreRegex.Replace(lines, pattern, replacement);
            }

            return lines
                .Split(Environment.NewLine)
                .Select(line => line.AsMemory());
        }
    }
}
