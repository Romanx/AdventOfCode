using System.Threading.Tasks;
using Helpers;

namespace DayThirteen2019;

public class ArcadeCabinet
{
    private readonly IntcodeComputer computer;

    const long Block = 2;
    const long Paddle = 3;
    const long Ball = 4;

    public ArcadeCabinet(ImmutableArray<long> memory)
    {
        computer = new IntcodeComputer(memory);
    }

    public async Task<long> WatchLoadScreen()
    {
        await computer.Run();

        var blocks = 0;

        while (await computer.Output.Reader.WaitToReadAsync())
        {
            computer.Output.Reader.TryRead(out _);
            computer.Output.Reader.TryRead(out _);
            computer.Output.Reader.TryRead(out var tileType);
            if (tileType == Block)
            {
                blocks++;
            }
        }

        return blocks;
    }

    public async Task<long> Play()
    {
        long score = 0;

        await Task.WhenAll(
            Task.Run(RunStateStep),
            computer.Run());

        return score;

        async Task RunStateStep()
        {
            long paddleX = 0;
            while (await computer.Output.Reader.WaitToReadAsync())
            {
                var x = await computer.Output.Reader.ReadAsync();
                var y = await computer.Output.Reader.ReadAsync();
                var val = await computer.Output.Reader.ReadAsync();

                if (x is -1)
                {
                    score = val;
                }
                else if (val is Paddle)
                {
                    paddleX = x;
                    var tile = val;
                }
                else if (val is Ball)
                {
                    await computer.Input.Writer.WriteAsync(Math.Sign(x - paddleX));
                }
            }
        }
    }
}
