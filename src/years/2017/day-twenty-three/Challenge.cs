﻿using MoreLinq;
using Shared.Parser;

namespace DayTwentyThree2017
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2017, 12, 23), "Coprocessor Conflagration");

        public void PartOne(IInput input, IOutput output)
        {
            var instructions = input.Lines.ParseInstructions();
            var program = new Program
            {
                Registers = "abcdefgh".ToCharArray().ToDictionary(k => k, v => 0L)
            };

            var multiplyCalls = 0;

            while (program.Pointer >= 0 && program.Pointer < instructions.Length)
            {
                var instruction = instructions[program.Pointer];

                if (instruction is MultiplyRegister)
                {
                    multiplyCalls++;
                }

                instruction.Apply(program);
                program.Pointer++;
            }

            output.WriteProperty("Number of calls to Multiply", multiplyCalls);
        }

        public void PartTwo(IInput input, IOutput output)
        {
            var instructions = input.Lines.ParseInstructions();

            var b = instructions
                .OfType<SetRegister>()
                .First(sr => sr.Target == 'b');

            var bValue = b.Source.GetValue();

            var start = bValue * 100 + 100_000;

            var result = MoreEnumerable.Sequence(start, start + 17000, 17)
                .Count(i => MathHelper.IsPrime(i) is false);

            output.WriteProperty("Program result", result);
        }
    }

    internal static class ParseExtensions
    {
        public static ImmutableArray<Instruction> ParseInstructions(this IInputLines lines)
        {
            return new CommandParser<Instruction>()
                .AddDerivedTypes<Instruction>()
                .ParseCommands(lines)
                .ToImmutableArray();
        }
    }
}
