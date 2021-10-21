using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Shared
{
    public sealed class Program
    {
        public Program(ImmutableArray<Command> commands, Dictionary<char, int> registers, int pointer)
        {
            Commands = commands;
            Registers = registers;
            Pointer = pointer;
        }

        public ImmutableArray<Command> Commands { get; set; }

        public Dictionary<char, int> Registers { get; set; }

        public int Pointer { get; set; }

        public static Dictionary<char, int> EmptyRegisters => new Dictionary<char, int>
        {
            ['a'] = 0,
            ['b'] = 0,
            ['c'] = 0,
            ['d'] = 0
        };

        public static Program New(ImmutableArray<Command> commands) => new(commands, EmptyRegisters, 0);

        public Program Run()
        {
            var program = this;

            while (program.Pointer < program.Commands.Length)
            {
                var instruction = program.Commands[program.Pointer];

                instruction.Apply(program);
            }

            return program;
        }

        public int GetValueOrRegisterValue(object input)
            => input is Register register
                ? Registers[register.RegisterName]
                : (int)input;

        public bool TryGetCommandAt(int index, [NotNullWhen(true)] out Command? command)
        {
            if (index > 0 && index < Commands.Length)
            {
                command = Commands[index];
                return true;
            }

            command = null;
            return false;
        }
    }
}
