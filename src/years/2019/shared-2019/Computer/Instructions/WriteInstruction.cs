﻿using System.Threading;
using System.Threading.Tasks;
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

        public override async ValueTask RunInstruction(ReadOnlyMemory<long> parameters, IntcodeComputer runtime, CancellationToken token)
        {
            await runtime.Output.Writer.WriteAsync(parameters.Span[0], token);

            runtime.AdjustIndexBy(2);
        }
    }
}
