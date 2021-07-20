using System;

namespace Helpers.Instructions
{
    internal class MultiplyInstruction : Instruction
    {
        public override OpCodes OpCode => OpCodes.Multiply;

        public override ParameterType[] Parameters { get; } = new[]
        {
            ParameterType.Read,
            ParameterType.Read,
            ParameterType.Write
        };

        public override void RunInstruction(in ReadOnlySpan<long> parameters, IntcodeComputer runtime)
        {
            runtime.WriteToMemory((int)parameters[2], parameters[0] * parameters[1]);
            runtime.AdjustIndexBy(4);
        }
    }
}