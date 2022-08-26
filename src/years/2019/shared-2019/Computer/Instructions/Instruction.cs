using System.Threading.Tasks;

namespace Helpers.Instructions
{
    internal abstract class Instruction
    {
        public abstract OpCodes OpCode { get; }

        public abstract ParameterType[] Parameters { get; }

        public abstract ValueTask RunInstruction(ReadOnlyMemory<long> parameters, IntcodeComputer runtime);
    }
}
