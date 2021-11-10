namespace Helpers.Instructions
{
    internal abstract class Instruction
    {
        public abstract OpCodes OpCode { get; }

        public abstract ParameterType[] Parameters { get; }

        public abstract void RunInstruction(in ReadOnlySpan<long> parameters, IntcodeComputer runtime);
    }
}
