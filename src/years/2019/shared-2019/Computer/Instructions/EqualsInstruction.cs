using System.Threading.Tasks;
using Helpers.Instructions;

namespace Helpers.Computer.Instructions
{
    internal class EqualsInstruction : Instruction
    {
        public override OpCodes OpCode => OpCodes.Equals;

        public override ParameterType[] Parameters => new ParameterType[]
        {
            ParameterType.Read,
            ParameterType.Read,
            ParameterType.Write
        };

        public override ValueTask RunInstruction(ReadOnlyMemory<long> parameters, IntcodeComputer runtime)
        {
            var span = parameters.Span;

            var first = span[0];
            var second = span[1];
            var outAddress = (int)span[2];

            var result = first == second
                ? 1
                : 0;

            runtime.WriteToMemory(outAddress, result);
            runtime.AdjustIndexBy(4);

            return ValueTask.CompletedTask;
        }
    }
}
