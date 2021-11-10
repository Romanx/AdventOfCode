using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace DayEighteen2017
{
    public class Program
    {
        public Program(int id, ChannelWriter<long> output, ChannelReader<long> input)
        {
            Id = id;
            Output = output;
            Input = input;
        }

        public int Id { get; }

        public ChannelWriter<long> Output { get; }

        public ChannelReader<long> Input { get; }

        public int Sends { get; private set; }

        public int Pointer { get; set; } = 0;

        public Dictionary<char, long> Registers { get; set; } = new Dictionary<char, long>();

        public async ValueTask SendValue(long value)
        {
            await Output.WriteAsync(value);
            Sends++;
        }

        public async Task<long> RecieveOrDeadlock()
        {
            var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));

            var readValue = await Input.ReadAsync(cts.Token);
            return readValue;
        }
    }
}
