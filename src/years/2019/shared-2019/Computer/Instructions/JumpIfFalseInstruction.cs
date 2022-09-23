using System.Threading;
using System.Threading.Tasks;
using Helpers.Instructions;

namespace Helpers.Computer.Instructions
{
    internal class JumpIfFalseInstruction : Instruction
    {
        public override OpCodes OpCode => OpCodes.JumpIfFalse;

        public override ParameterType[] Parameters => new ParameterType[]
        {
            ParameterType.Read,
            ParameterType.Read
        };

        public override ValueTask RunInstruction(ReadOnlyMemory<long> parameters, IntcodeComputer runtime, CancellationToken token)
        {
            var span = parameters.Span;

            if (span[0] == 0)
            {
                runtime.SetIndex((int)span[1]);
            }
            else
            {
                runtime.AdjustIndexBy(3);
            }

            return ValueTask.CompletedTask;
        }
    }
}
