using System.Threading.Channels;
using System.Threading.Tasks;
using Helpers;
using Helpers.Computer;
using Shared2019;

namespace DayTwentyOne2019;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2019, 12, 21), "Springdroid Adventure");

    public async Task PartOne(IInput input, IOutput output)
    {
        var droid = new SpringDroid(input.AsIntcodeProgram());

        var instructions = ImmutableArray.Create(new[]
        {
            new Instruction(Operation.NOT, ReadRegister.T, WriteRegister.T), // Reset T to TRUE
            new Instruction(Operation.AND, ReadRegister.A, WriteRegister.T), // If there is a hole 1 step away Set T to True
            new Instruction(Operation.AND, ReadRegister.B, WriteRegister.T), // If there is a hole 1 and 2 steps away Set T to True
            new Instruction(Operation.AND, ReadRegister.C, WriteRegister.T), // If there is a hole 1, 2 and 3 steps away Set T to True
            new Instruction(Operation.NOT, ReadRegister.T, WriteRegister.J), // Don't jump if there isn't a 3 size hole in front of me

            // If there is a not hole four spots away, and the jump register is true, set the jump register to true
            new Instruction(Operation.AND, ReadRegister.D, WriteRegister.J),

            // Go!
            new Instruction("WALK")
        });

        var hullDamage = await droid.Run(instructions);

        output.WriteProperty("Hull damage reported", hullDamage);
    }

    public async Task PartTwo(IInput input, IOutput output)
    {
        var droid = new SpringDroid(input.AsIntcodeProgram());

        var instructions = ImmutableArray.Create(new[]
        {
            new Instruction(Operation.NOT, ReadRegister.T, WriteRegister.T), // Reset T to TRUE
            new Instruction(Operation.AND, ReadRegister.A, WriteRegister.T), // A
            new Instruction(Operation.AND, ReadRegister.B, WriteRegister.T), // A AND B
            new Instruction(Operation.AND, ReadRegister.C, WriteRegister.T), // A AND B AND C
            new Instruction(Operation.NOT, ReadRegister.T, WriteRegister.T), // !A OR !B OR !C
            new Instruction(Operation.NOT, ReadRegister.J, WriteRegister.J), // TRUE
            new Instruction(Operation.AND, ReadRegister.E, WriteRegister.J), // E
            new Instruction(Operation.OR , ReadRegister.H, WriteRegister.J), // E OR H
            new Instruction(Operation.AND, ReadRegister.T, WriteRegister.J), // (!A OR !B OR !C) AND (E OR H)
            new Instruction(Operation.AND, ReadRegister.D, WriteRegister.J), // (!A OR !B OR !C) AND (E OR H) AND D
            new Instruction("RUN")
        });

        var hullDamage = await droid.Run(instructions);

        output.WriteProperty("Hull damage reported", hullDamage);
    }
}

class SpringDroid
{
    private readonly ImmutableArray<long> program;

    public SpringDroid(ImmutableArray<long> program)
    {
        this.program = program;
    }

    public async Task<long> Run(ImmutableArray<Instruction> instructions)
    {
        var computer = new IntcodeComputer(program);
        foreach (var instruction in instructions)
        {
            instruction.WriteCommand(computer.Input.Writer);
        }

        var results = await computer.RunAndGetOutput();

        return results[^1];
    }
}

readonly record struct Instruction
{
    private readonly string command;

    public Instruction(Operation operation, ReadRegister a, WriteRegister b)
    {
        command = $"{operation} {a} {b}";
    }

    public Instruction(string command)
    {
        this.command = command;
    }

    internal void WriteCommand(ChannelWriter<long> writer)
    {
        foreach (var c in command)
        {
            writer.TryWrite(c);
        }
        writer.TryWrite('\n');
    }
}

enum Operation
{
    NOT,
    AND,
    OR
}

enum ReadRegister
{
    A,
    B,
    C,
    D,
    E,
    F,
    G,
    H,
    I,
    T,
    J
}

enum WriteRegister
{
    T,
    J
}
