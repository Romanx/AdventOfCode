using System.Diagnostics;
using System.Threading.Tasks;
using Helpers;
using Spectre.Console;

namespace DaySeventeen2019;

public static class ScaffoldingInterface
{
    public static async Task<ImmutableDictionary<Point2d, char>> ScanScaffolding(ImmutableArray<long> program)
    {
        var computer = new IntcodeComputer(program);
        await computer.Run();

        var result = ImmutableDictionary.CreateBuilder<Point2d, char>();
        var position = Point2d.Origin;

        await foreach (var value in computer.Output.Reader.ReadAllAsync())
        {
            var @char = (char)value;

            switch (@char)
            {
                case '#':
                case '.':
                case '^':
                case '>':
                case '<':
                case 'V':
                    result.Add(position, @char);
                    position += (1, 0);
                    break;
                case '\n':
                    position = (0, position.Y + 1);
                    break;
                default:
                    throw new InvalidOperationException("What case!?");
            }
        }

        return result.ToImmutable();
    }

    internal static async Task<long> MoveRobot(
        ImmutableArray<long> scaffoldProgram,
        string robotProgram,
        ImmutableArray<string[]> functions)
    {
        var computer = new IntcodeComputer(scaffoldProgram);
        EnqueueCommand(computer, robotProgram.Select(c => c.ToString()));

        foreach (var function in functions)
        {
            EnqueueCommand(computer, function);
        }

        computer.Input.Writer.TryWrite('n'); // Do not ouput live view
        computer.Input.Writer.TryWrite('\n');

        var results = await computer.RunAndGetOutput();
        return results[^1];

        static void EnqueueCommand(
            IntcodeComputer computer,
            IEnumerable<string> command)
        {
            var ascii = ToAscii(string.Join(",", command));

            if (ascii.Count() > 20)
            {
                Console.WriteLine("Too Many Instructions!");
                Debugger.Break();
            }

            foreach (var c in ascii)
            {
                computer.Input.Writer.TryWrite(c);
            }
            computer.Input.Writer.TryWrite('\n');

            static IEnumerable<int> ToAscii(string @in) => @in.ToCharArray().Select(c => (int)c);
        }
    }
}
