﻿namespace DayTwelve2016
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2016, 12, 12), "Leonardo's Monorail");

        public void PartOne(IInput input, IOutput output)
        {
            var instructions = AssembunnyParser.BuildParser()
                .ParseCommands(input.Lines)
                .ToImmutableArray();

            var program = Program.New(instructions)
                .Run();

            foreach (var (register, val) in program.Registers)
            {
                output.WriteProperty($"Register '{register}'", val);
            }
        }

        public void PartTwo(IInput input, IOutput output)
        {
            var instructions = AssembunnyParser.BuildParser()
                .ParseCommands(input.Lines)
                .ToImmutableArray();

            var registers = Program.EmptyRegisters;
            registers['c'] = 1;

            var program = new Program(instructions, registers, 0)
                .Run();

            foreach (var (register, val) in program.Registers)
            {
                output.WriteProperty($"Register '{register}'", val);
            }
        }
    }
}
