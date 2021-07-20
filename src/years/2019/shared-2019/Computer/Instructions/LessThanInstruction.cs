using Helpers.Instructions;
using System;

namespace Helpers.Computer.Instructions
{
    internal class LessThanInstruction : Instruction
    {
        public override OpCodes OpCode => OpCodes.LessThan;

        public override ParameterType[] Parameters => new ParameterType[]
        {
            ParameterType.Read,
            ParameterType.Read,
            ParameterType.Write
        };

        public override void RunInstruction(in ReadOnlySpan<long> parameters, IntcodeComputer runtime)
        {
            var first = parameters[0];
            var second = parameters[1];
            var outAddress = (int)parameters[2];

            var result = first < second
                ? 1
                : 0;

            runtime.WriteToMemory(outAddress, result);
            runtime.AdjustIndexBy(4);
        }
    }
}
