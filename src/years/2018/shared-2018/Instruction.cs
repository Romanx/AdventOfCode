namespace Shared
{
    public delegate void InstructionAction(Span<int> registers, int a, int b, int output);

    public record Instruction(string Name, InstructionAction Action);
}
