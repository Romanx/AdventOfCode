using Helpers.Instructions;

namespace Helpers.Computer.Instructions
{
    internal class WriteInstruction : Instruction
    {
        public override OpCodes OpCode => OpCodes.Write;

        public override ParameterType[] Parameters => new []
        {
            ParameterType.Read
        };

        public override void RunInstruction(in ReadOnlySpan<long> parameters, IntcodeComputer runtime)
        {
            runtime.Output.Enqueue(parameters[0]);

            runtime.AdjustIndexBy(2);
        }
    }
}
