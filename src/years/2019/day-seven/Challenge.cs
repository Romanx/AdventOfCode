using System.Threading.Channels;
using System.Threading.Tasks;
using Helpers;
using MoreAsyncLINQ;
using Shared2019;
using static MoreLinq.Extensions.PermutationsExtension;

namespace DaySeven2019;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2019, 12, 7), "Amplification Circuit");

    public async Task PartOne(IInput input, IOutput output)
    {
        var program = input.AsIntcodeProgram();
        var permutations = new[] { 0, 1, 2, 3, 4 }.Permutations();

        var (thrusterOutput, phaseCombination) = await permutations
            .ToAsyncEnumerable()
            .SelectAwait(async permutation =>
            {
                var thrusterOutput = await RunThrusterProgram(program, permutation);
                return (ThrusterOutput: thrusterOutput, Permutation: permutation);
            })
            .MaxBy(x => x.ThrusterOutput)
            .FirstAsync();

        output.WriteProperty("Max Thruster output", thrusterOutput);
        output.WriteProperty("Creating sequence", string.Join(", ", phaseCombination));

        static async Task<int> RunThrusterProgram(
            ImmutableArray<long> program,
            IList<int> phases)
        {
            return await Enumerable.Range(0, 5)
                .ToAsyncEnumerable()
                .AggregateAwaitAsync(0, async (signal, phaseIdx) =>
                {
                    var computer = new IntcodeComputer(program);
                    await computer.Input.Writer.WriteAsync(phases[phaseIdx]);
                    await computer.Input.Writer.WriteAsync(signal);

                    var output = await computer.RunAndGetOutput();

                    return (int)output[0];
                });
        }
    }

    public async Task PartTwo(IInput input, IOutput output)
    {
        var program = input.AsIntcodeProgram();
        var permutations = new[] { 5, 6, 7, 8, 9 }.Permutations();

        var (thrusterOutput, phaseCombination) = await permutations
            .ToAsyncEnumerable()
            .SelectAwait(async permutation =>
            {
                var thrusterOutput = await RunThrusterProgram(program, permutation);
                return (ThrusterOutput: thrusterOutput, Permutation: permutation);
            })
            .MaxBy(x => x.ThrusterOutput)
            .FirstAsync();

        output.WriteProperty("Max Thruster output", thrusterOutput);
        output.WriteProperty("Creating sequence", string.Join(", ", phaseCombination));

        static async Task<long> RunThrusterProgram(
            ImmutableArray<long> program,
            IList<int> phases)
        {
            var a = new IntcodeComputer(program);
            var b = new IntcodeComputer(program);
            var c = new IntcodeComputer(program);
            var d = new IntcodeComputer(program);
            var e = new IntcodeComputer(program);
            var spy = new ChannelSpy<long>(
                e.Output.Reader,
                a.Input.Writer,
                new BoundedChannelOptions(1) { FullMode = BoundedChannelFullMode.DropOldest });

            await a.Input.Writer.WriteAsync(phases[0]);
            await a.Input.Writer.WriteAsync(0);

            await b.Input.Writer.WriteAsync(phases[1]);
            b.AddInputReader(a.Output.Reader);

            await c.Input.Writer.WriteAsync(phases[2]);
            c.AddInputReader(b.Output.Reader);

            await d.Input.Writer.WriteAsync(phases[3]);
            d.AddInputReader(c.Output.Reader);

            await e.Input.Writer.WriteAsync(phases[4]);
            e.AddInputReader(d.Output.Reader);

            await Task.WhenAll(
                spy.Listen(),
                a.Run(),
                b.Run(),
                c.Run(),
                d.Run(),
                e.Run());

            var result = await spy.Items.ReadAsync();

            return result;
        }
    }
}
