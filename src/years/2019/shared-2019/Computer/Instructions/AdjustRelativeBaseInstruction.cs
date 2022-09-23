using System.Threading;
using System.Threading.Tasks;
using Helpers.Instructions;

namespace Helpers.Computer.Instructions
{
    internal class AdjustRelativeBaseInstruction : Instruction
    {
        public override OpCodes OpCode => OpCodes.AdjustRelativeBase;

        public override ParameterType[] Parameters { get; } = new[]
        {
            ParameterType.Read
        };

        public override ValueTask RunInstruction(ReadOnlyMemory<long> parameters, IntcodeComputer runtime, CancellationToken token)
        {
            var span = parameters.Span;
            runtime.AdjustRelativeBaseBy((int)span[0]);
            runtime.AdjustIndexBy(2);
            return ValueTask.CompletedTask;
        }
    }
}
