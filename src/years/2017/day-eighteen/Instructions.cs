using System.Threading.Tasks;
using Shared.Parser;

namespace DayEighteen2017
{
    abstract record Instruction(string Name)
    {
        public abstract ValueTask Apply(Program program);
    }

    [CommandRegex(@"snd (.*)")]
    record PlaySound(Argument Frequency) : Instruction("snd")
    {
        public static PlaySound Build(in PcreRefMatch.GroupList group)
        {
            var frequency = Argument.Parse(group[1].Value);
            return new PlaySound(frequency);
        }

        public override async ValueTask Apply(Program program)
        {
            await program.SendValue(Frequency.GetValue(program.Registers));
        }

        public override string ToString() => $"{Name} {Frequency}";
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

        public override ValueTask Apply(Program program)
        {
            program.Registers[Target] = Source.GetValue(program.Registers);
            return ValueTask.CompletedTask;
        }

        public override string ToString() => $"{Name} {Target} {Source}";
    }

    [CommandRegex(@"add (.*) (.*)")]
    record AddRegister(Argument Target, Argument Source) : Instruction("add")
    {
        public static AddRegister Build(in PcreRefMatch.GroupList group)
        {
            var target = Argument.Parse(group[1].Value);
            var source = Argument.Parse(group[2].Value);
            return new AddRegister(target, source);
        }

        public override ValueTask Apply(Program program)
        {
            var next = Target.GetValue(program.Registers) + Source.GetValue(program.Registers);
            program.Registers[Target] = next;
            return ValueTask.CompletedTask;
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

        public override ValueTask Apply(Program program)
        {
            var next = Target.GetValue(program.Registers) * Source.GetValue(program.Registers);
            program.Registers[Target] = next;
            return ValueTask.CompletedTask;
        }

        public override string ToString() => $"{Name} {Target} {Source}";
    }

    [CommandRegex(@"mod (.*) (.*)")]
    record ModuloRegister(Argument Target, Argument Source) : Instruction("mod")
    {
        public static ModuloRegister Build(in PcreRefMatch.GroupList group)
        {
            var target = Argument.Parse(group[1].Value);
            var source = Argument.Parse(group[2].Value);
            return new ModuloRegister(target, source);
        }

        public override ValueTask Apply(Program program)
        {
            var next = Target.GetValue(program.Registers) % Source.GetValue(program.Registers);
            program.Registers[Target] = next;
            return ValueTask.CompletedTask;
        }

        public override string ToString() => $"{Name} {Target} {Source}";
    }

    [CommandRegex(@"rcv (.*)")]
    record RecoverSound(Argument Target) : Instruction("rcv")
    {
        public static RecoverSound Build(in PcreRefMatch.GroupList group)
        {
            var target = Argument.Parse(group[1].Value);
            return new RecoverSound(target);
        }

        public override async ValueTask Apply(Program program)
        {
            var value = await program.RecieveOrDeadlock();
            program.Registers[Target] = value;
        }

        public override string ToString() => $"{Name} {Target}";

        public async ValueTask<long?> ApplyPart1(Program program)
        {
            var val = Target.GetValue(program.Registers);
            if (val > 0)
            {
                var list = new List<long>();
                program.Output.Complete();
                await foreach (var value in program.Input.ReadAllAsync())
                {
                    list.Add(value);
                }

                return list[^1];
            }

            return null;
        }
    }

    [CommandRegex(@"jgz (.*) (.*)")]
    record JumpToOffset(Argument Target, Argument Source) : Instruction("jgz")
    {
        public static JumpToOffset Build(in PcreRefMatch.GroupList group)
        {
            var target = Argument.Parse(group[1].Value);
            var source = Argument.Parse(group[2].Value);
            return new JumpToOffset(target, source);
        }

        public override ValueTask Apply(Program program)
        {
            var value = Target.GetValue(program.Registers);
            var offset = Source.GetValue(program.Registers);

            if (value > 0)
            {
                program.Pointer += (int)offset - 1;
            }

            return ValueTask.CompletedTask;
        }

        public override string ToString() => $"{Name} {Target} {Source}";
    }
}
