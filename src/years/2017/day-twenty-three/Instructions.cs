using System.Threading.Tasks;
using PCRE;
using Shared.Parser;

namespace DayTwentyThree2017
{
    abstract record Instruction(string Name)
    {
        public abstract void Apply(Program program);
    }

    [CommandRegex(@"set (.*) (.*)")]
    record SetRegister(Argument Target, Argument Source) : Instruction("set")
    {
        public static SetRegister Build(in PcreRefMatch.GroupList group)
        {
            var target = Argument.Parse(group[1].Value);
            var source = Argument.Parse(group[2].Value);
            return new SetRegister(target, source);
        }

        public override void Apply(Program program)
        {
            program.Registers[Target] = Source.GetValue(program.Registers);
        }

        public override string ToString() => $"{Name} {Target} {Source}";
    }

    [CommandRegex(@"sub (.*) (.*)")]
    record SubtractRegister(Argument Target, Argument Source) : Instruction("sub")
    {
        public static SubtractRegister Build(in PcreRefMatch.GroupList group)
        {
            var target = Argument.Parse(group[1].Value);
            var source = Argument.Parse(group[2].Value);
            return new SubtractRegister(target, source);
        }

        public override void Apply(Program program)
        {
            var next = Target.GetValue(program.Registers) - Source.GetValue(program.Registers);
            program.Registers[Target] = next;
        }

        public override string ToString() => $"{Name} {Target} {Source}";
    }

    [CommandRegex(@"mul (.*) (.*)")]
    record MultiplyRegister(Argument Target, Argument Source) : Instruction("mul")
    {
        public static MultiplyRegister Build(in PcreRefMatch.GroupList group)
        {
            var target = Argument.Parse(group[1].Value);
            var source = Argument.Parse(group[2].Value);
            return new MultiplyRegister(target, source);
        }

        public override void Apply(Program program)
        {
            var next = Target.GetValue(program.Registers) * Source.GetValue(program.Registers);
            program.Registers[Target] = next;
        }

        public override string ToString() => $"{Name} {Target} {Source}";
    }

    [CommandRegex(@"jnz (.*) (.*)")]
    record JumpNotZero(Argument Target, Argument Source) : Instruction("jnz")
    {
        public static JumpNotZero Build(in PcreRefMatch.GroupList group)
        {
            var target = Argument.Parse(group[1].Value);
            var source = Argument.Parse(group[2].Value);
            return new JumpNotZero(target, source);
        }

        public override void Apply(Program program)
        {
            var value = Target.GetValue(program.Registers);
            var offset = Source.GetValue(program.Registers);

            if (value != 0)
            {
                program.Pointer += (int)offset - 1;
            }
        }

        public override string ToString() => $"{Name} {Target} {Source}";
    }
}
