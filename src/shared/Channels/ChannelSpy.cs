using System.Threading.Channels;
using System.Threading.Tasks;

namespace Shared;

public readonly record struct ChannelSpy<T>
{
    private readonly ChannelReader<T> input;
    private readonly ChannelWriter<T> output;
    private readonly Channel<T> spy;

    public ChannelReader<T> Items => spy.Reader;

    public ChannelSpy(ChannelReader<T> input, ChannelWriter<T> output, BoundedChannelOptions options)
    {
        this.input = input;
        this.output = output;
        spy = Channel.CreateBounded<T>(options);
    }

    public async Task Listen()
    {
        await foreach (var item in input.ReadAllAsync())
        {
            await spy.Writer.WriteAsync(item);
            output.TryWrite(item);
        }

        spy.Writer.Complete();
    }
}
