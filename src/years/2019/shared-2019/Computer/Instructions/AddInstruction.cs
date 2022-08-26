using System.Threading.Tasks;

namespace Helpers.Instructions
{
    internal class AddInstruction : Instruction
    {
        public override OpCodes OpCode => OpCodes.Add;

        public override ParameterType[] Parameters { get; } = new[]
        {
            ParameterType.Read,
            ParameterType.Read,
            ParameterType.Write
        };

        public override ValueTask RunInstruction(ReadOnlyMemory<long> parameters, IntcodeComputer runtime)
        {
            var span = parameters.Span;
            runtime.WriteToMemory((int)span[2], span[0] + span[1]);
            runtime.AdjustIndexBy(4);
            return ValueTask.CompletedTask;
        }
    }
}

