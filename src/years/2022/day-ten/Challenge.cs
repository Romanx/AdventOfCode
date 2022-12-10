using Shared.Parser;
using Spectre.Console;

namespace DayTen2022;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2022, 12, 10), "Cathode-Ray Tube");

    public void PartOne(IInput input, IOutput output)
    {
        var instructions = new CommandParser<Instruction>()
            .AddDerivedTypes<Instruction>()
            .ParseCommands(input.Lines);

        var cycle = 0;
        var totalSignalStrength = 0;

        var registers = new Dictionary<char, int>()
        {
            ['X'] = 1
        };

        foreach (var instruction in instructions)
        {
            for (var i = 0; i < instruction.Cycles; i++)
            {
                cycle++;
                if (cycle == 20 || (cycle % 40) == 20)
                {
                    var signalStrength = cycle * registers['X'];
                    totalSignalStrength += signalStrength;
                }
            }
            instruction.Apply(registers);
        }

        output.WriteProperty("Total Signal Strength", totalSignalStrength);
    }

    public void PartTwo(IInput input, IOutput output)
    {
        var screen = new Color[6, 40];

        var instructions = new CommandParser<Instruction>()
            .AddDerivedTypes<Instruction>()
            .ParseCommands(input.Lines);

        var cycle = 0;

        var registers = new Dictionary<char, int>()
        {
            ['X'] = 1
        };

        foreach (var instruction in instructions)
        {
            var sprite = new NumberRange<int>(registers['X'] - 1, registers['X'] + 1);

            for (var i = 0; i < instruction.Cycles; i++)
            {
                var row = cycle / 40;
                var col = cycle % 40;

                screen[row, col] = sprite.Contains(col)
                    ? Color.White
                    : Color.Black;

                cycle++;
            }
            instruction.Apply(registers);
            if (instruction is AddX)
            {
                // When X changes update the sprite range.
                sprite = new NumberRange<int>(registers['X'] - 1, registers['X'] + 1);
            }
        }

        output.WriteBlock(() =>
        {
            return new Panel(ConsoleImagePrinter.Print(screen));
        });
    }
}

abstract record Instruction(int Cycles)
{
    public abstract void Apply(Dictionary<char, int> registers);
}

[CommandRegex("noop")]
record Noop() : Instruction(Cycles: 1)
{
    public static Instruction Build(in PcreRefMatch.GroupList groups)
    {
        return new Noop();
    }

    public override void Apply(Dictionary<char, int> registers)
    {
    }
}

[CommandRegex(@"addx (?<Value>-?\d+)")]
record AddX(int Value) : Instruction(Cycles: 2)
{
    public static Instruction Build(in PcreRefMatch.GroupList groups)
    {
        var value = int.Parse(groups["Value"].Value);
        return new AddX(value);
    }

    public override void Apply(Dictionary<char, int> registers)
    {
        registers['X'] = registers['X'] + Value;
    }
}
