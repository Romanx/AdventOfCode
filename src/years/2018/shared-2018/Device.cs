using System;
using System.Collections.Immutable;
using System.Linq;
using static Shared.InstructionHelper;

namespace Shared
{
    public class Device
    {
        protected readonly int _instructionPointerBinding;
        protected readonly int[] _registers;

        public ImmutableArray<int> Registers => _registers.ToImmutableArray();
        public ImmutableArray<Command> Commands { get; }

        public Device(int instructionPointerBinding, ImmutableArray<Command> commands)
        {
            _instructionPointerBinding = instructionPointerBinding;
            Commands = commands;
            _registers = new int[6];
            _registers.AsSpan().Fill(0);
        }

        public virtual void Run()
        {
            var ip = _registers[_instructionPointerBinding];
            while ((0..Commands.Length).Contains(ip))
            {
                var command = Commands[ip];
                var instruction = InstructionMap[command.Instruction];
                _registers[_instructionPointerBinding] = ip;
                instruction.Action(_registers, command.A, command.B, command.C);
                ip = _registers[_instructionPointerBinding] + 1;
            }
        }
    }

    public record Command(string Instruction, int A, int B, int C)
    {
        public override string ToString() => $"{Instruction} {A} {B} {C}";
    }
}
