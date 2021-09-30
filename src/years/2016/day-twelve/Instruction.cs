namespace DayTwelve2016
{
    abstract record Instruction(string Name)
    {
        public abstract Program Apply(Program program);
    }

    record Copy(char Register, object Value) : Instruction("cpy")
    {
        public override Program Apply(Program program)
        {
            var newValue = Value is int intVal
                ? intVal
                : program.Registers[((string)Value)[0]];

            return program with
            {
                Pointer = program.Pointer + 1,
                Registers = program.Registers.SetItem(Register, newValue)
            };
        }

        public override string ToString() => $"{Name} {Value} {Register}";
    }

    record Increment(char Register) : Instruction("inc")
    {
        public override Program Apply(Program program)
        {
            return program with
            {
                Pointer = program.Pointer + 1,
                Registers = program.Registers.SetItem(Register, program.Registers[Register] + 1)
            };
        }

        public override string ToString() => $"{Name} {Register}";
    }

    record Decrement(char Register) : Instruction("dec")
    {
        public override Program Apply(Program program)
        {
            return program with
            {
                Pointer = program.Pointer + 1,
                Registers = program.Registers.SetItem(Register, program.Registers[Register] - 1)
            };
        }

        public override string ToString() => $"{Name} {Register}";
    }

    record JumpNotZero(object Target, int Offset) : Instruction("jnz")
    {
        public override Program Apply(Program program)
        {
            var value = Target is int intVal
                ? intVal
                : program.Registers[((string)Target)[0]];

            var isZero = value == 0;

            return isZero
                ? program with { Pointer = program.Pointer + 1 }
                : program with
                {
                    Pointer = program.Pointer + Offset
                };
        }

        public override string ToString() => $"{Name} {Target} {Offset}";
    }
}
