using Shared.Parser;

namespace Shared
{
    [CommandRegex("cpy (?<First>.*) (?<Second>.*)")]
    record Copy(object[] Arguments) : Command("cpy", Arguments)
    {
        public override void Apply(Program program)
        {
            var newValue = program.GetValueOrRegisterValue(Arguments[0]);
            var register = (Register)Arguments[1];

            program.Pointer++;
            program.Registers[register] = newValue;
        }

        public static Copy Build(in PcreRefMatch.GroupList groups)
            => new(ParseArguments(groups));

        public override string ToString() => $"{Name} {string.Join(' ', Arguments)}";
    }

    [CommandRegex("inc (?<First>.*)")]
    record Increment(object[] Arguments) : Command("inc", Arguments)
    {
        public override void Apply(Program program)
        {
            var register = (Register)Arguments[0];

            program.Pointer++;
            program.Registers[register] = program.Registers[register] + 1;
        }

        public static Increment Build(in PcreRefMatch.GroupList groups)
            => new(ParseArguments(groups));

        public override string ToString() => $"{Name} {string.Join(' ', Arguments)}";
    }

    [CommandRegex("dec (?<First>.*)")]
    record Decrement(object[] Arguments) : Command("dec", Arguments)
    {
        public override void Apply(Program program)
        {
            var register = (Register)Arguments[0];

            program.Pointer++;
            program.Registers[register] = program.Registers[register] - 1;
        }

        public static Decrement Build(in PcreRefMatch.GroupList groups)
            => new(ParseArguments(groups));

        public override string ToString() => $"{Name} {string.Join(' ', Arguments)}";
    }

    [CommandRegex("jnz (?<First>.*) (?<Second>.*)")]
    record JumpNotZero(object[] Arguments) : Command("jnz", Arguments)
    {
        public override void Apply(Program program)
        {
            var value = program.GetValueOrRegisterValue(Arguments[0]);
            var offset = program.GetValueOrRegisterValue(Arguments[1]);

            var isZero = value == 0;

            program.Pointer = isZero
                ? program.Pointer + 1
                : program.Pointer + offset;
        }

        public static JumpNotZero Build(in PcreRefMatch.GroupList groups)
            => new(ParseArguments(groups));

        public override string ToString() => $"{Name} {string.Join(' ', Arguments)}";
    }

    [CommandRegex("tgl (?<First>.*)")]
    record Toggle(object[] Arguments) : Command("tgl", Arguments)
    {
        public override void Apply(Program program)
        {
            var offset = program.GetValueOrRegisterValue(Arguments[0]);
            var targetIndex = program.Pointer + offset;

            if (program.TryGetCommandAt(targetIndex, out var command))
            {
                Command replacement = command switch
                {
                    Increment inc => new Decrement(inc.Arguments),
                    Decrement dec => new Increment(dec.Arguments),
                    Toggle tog => new Increment(tog.Arguments),
                    JumpNotZero jnz => new Copy(jnz.Arguments),
                    Copy cpy => new JumpNotZero(cpy.Arguments),
                    _ => throw new InvalidOperationException("Not a valid command to replace"),
                };

                program.Commands = program.Commands.SetItem(targetIndex, replacement);
            }

            program.Pointer++;
        }

        public static Toggle Build(in PcreRefMatch.GroupList groups)
            => new(ParseArguments(groups));

        public override string ToString() => $"{Name} {string.Join(' ', Arguments)}";
    }

    [CommandRegex("add (.*) (.*) (.*)")]
    record Add(object[] Arguments) : Command("add", Arguments)
    {
        public override void Apply(Program program)
        {
            var first = program.GetValueOrRegisterValue(Arguments[0]);
            var second = program.GetValueOrRegisterValue(Arguments[1]);

            var target = (Register)Arguments[2];

            program.Registers[target] = first + second;
            program.Pointer++;
        }

        public static Add Build(in PcreRefMatch.GroupList groups)
            => new(ParseArguments(groups));

        public override string ToString() => $"{Name} {string.Join(' ', Arguments)}";
    }

    [CommandRegex("mul (.*) (.*) (.*)")]
    record Multiply(object[] Arguments) : Command("mul", Arguments)
    {
        public override void Apply(Program program)
        {
            var first = program.GetValueOrRegisterValue(Arguments[0]);
            var second = program.GetValueOrRegisterValue(Arguments[1]);

            var target = (Register)Arguments[2];

            program.Registers[target] = first * second;
            program.Pointer++;
        }

        public static Multiply Build(in PcreRefMatch.GroupList groups)
            => new(ParseArguments(groups));

        public override string ToString() => $"{Name} {string.Join(' ', Arguments)}";
    }

    [CommandRegex("nop")]
    record Nop() : Command("nop", Array.Empty<object>())
    {
        public static Nop Build(in PcreRefMatch.GroupList _)
            => new();

        public override void Apply(Program program)
        {
            program.Pointer++;
        }

        public override string ToString() => "nop";
    }
}
