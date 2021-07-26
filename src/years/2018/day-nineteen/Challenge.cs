using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using NodaTime;
using Shared;
using static Shared.InstructionHelper;

namespace DayNineteen2018
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2018, 12, 19), "Go With The Flow");

        public override void PartOne(IInput input, IOutput output)
        {
            var device = input.Parse();
            device.RunUntilEnd();
            var registers = device.Registers;

            output.WriteProperty("Register Zero Value", registers[0]);
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            // Found by watching the registers until one is large and stable
            var largeNumber = 10_551_275;

            var factor = Factors(largeNumber).Sum();
            output.WriteProperty("Large Number", largeNumber);
            output.WriteProperty("Factor of Number", factor);

            static IEnumerable<long> Factors(int val) => Enumerable.Range(1, val + 1)
                .Select(i => (long)i)
                .Where(i => val % i == 0);
        }
    }

    class Device
    {
        private readonly ImmutableArray<Command> _commands;
        private readonly int _instructionPointerBinding;
        private readonly int[] _registers;

        public ImmutableArray<int> Registers => _registers.ToImmutableArray();

        public Device(int instructionPointerBinding, ImmutableArray<Command> commands)
        {
            _instructionPointerBinding = instructionPointerBinding;
            _commands = commands;
            _registers = new int[6];
            _registers.AsSpan().Fill(0);
        }

        public void RunUntilEnd()
        {
            var ip = _registers[_instructionPointerBinding];
            while ((0.._commands.Length).Contains(ip))
            {
                var command = _commands[ip];
                var instruction = InstructionMap[command.Instruction];
                _registers[_instructionPointerBinding] = ip;
                instruction.Action(_registers, command.A, command.B, command.C);
                ip = _registers[_instructionPointerBinding] + 1;
            }
        }

        public int RunUntilLargeNumber()
        {
            var ip = _registers[_instructionPointerBinding];
            while ((0.._commands.Length).Contains(ip))
            {
                var command = _commands[ip];
                var instruction = InstructionMap[command.Instruction];
                _registers[_instructionPointerBinding] = ip;
                instruction.Action(_registers, command.A, command.B, command.C);
                ip = _registers[_instructionPointerBinding] + 1;

                if (_registers.Any(i => i > int.MaxValue))
                {
                    return _registers.First(i => i > int.MaxValue);
                }
            }

            throw new InvalidOperationException("Shouldn't get here");
        }

        internal void SetRegisters(int[] registers) => registers.AsSpan().CopyTo(_registers.AsSpan());
    }

    record Command(string Instruction, int A, int B, int C)
    {
        public override string ToString() => $"{Instruction} {A} {B} {C}";
    }

    internal static class ParseExtensions
    {
        public static Device Parse(this IInput input)
        {
            var builder = ImmutableArray.CreateBuilder<Command>();
            var lines = input.AsLines().ToArray().AsSpan();
            var instructionPointer = int.Parse(lines[0].Span[3..]);

            foreach (var line in lines[1..])
            {
                builder.Add(ToCommand(line));
            }

            return new Device(instructionPointer, builder.ToImmutable());

            static Command ToCommand(ReadOnlyMemory<char> line)
            {
                var items = line.ToString().Split(' ');

                return new Command(items[0], int.Parse(items[1]), int.Parse(items[2]), int.Parse(items[3]));
            }
        }
    }
}
