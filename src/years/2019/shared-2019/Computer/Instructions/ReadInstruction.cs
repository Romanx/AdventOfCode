using System.Threading;
using System.Threading.Tasks;
using Helpers.Instructions;

namespace Helpers.Computer.Instructions
{
    internal class ReadInstruction : Instruction
    {
        public override OpCodes OpCode => OpCodes.Read;

        public override ParameterType[] Parameters { get; } = new[]
        {
            ParameterType.Write
        };

        public override async ValueTask RunInstruction(ReadOnlyMemory<long> parameters, IntcodeComputer runtime, CancellationToken token)
        {
            var outputAddress = (int)parameters.Span[0];

            var input = await runtime.Input.Reader.ReadAsync(token);

            runtime.WriteToMemory(outputAddress, input);
            runtime.AdjustIndexBy(2);
        }
    }
}
