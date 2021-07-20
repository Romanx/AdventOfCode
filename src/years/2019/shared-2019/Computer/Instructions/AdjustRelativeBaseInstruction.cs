using Helpers.Instructions;
using System;

namespace Helpers.Computer.Instructions
{
    internal class AdjustRelativeBaseInstruction : Instruction
    {
        public override OpCodes OpCode => OpCodes.AdjustRelativeBase;

        public override ParameterType[] Parameters { get; } = new[]
        {
            ParameterType.Read
        };

        public override void RunInstruction(in ReadOnlySpan<long> parameters, IntcodeComputer runtime)
        {
            runtime.AdjustRelativeBaseBy((int)parameters[0]);
            runtime.AdjustIndexBy(2);
        }
    }
}
