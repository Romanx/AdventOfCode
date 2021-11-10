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

        public override void RunInstruction(in ReadOnlySpan<long> parameters, IntcodeComputer runtime)
        {
            if (parameters[0] == 0)
            {
                runtime.SetIndex((int)parameters[1]);
            }
            else
            {
                runtime.AdjustIndexBy(3);
            }
        }
    }
}
