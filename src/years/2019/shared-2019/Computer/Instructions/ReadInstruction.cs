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

        public override void RunInstruction(in ReadOnlySpan<long> parameters, IntcodeComputer runtime)
        {
            var outputAddress = (int)parameters[0];

            var input = runtime.Input.Dequeue();

            runtime.WriteToMemory(outputAddress, input);
            runtime.AdjustIndexBy(2);
        }
    }
}
