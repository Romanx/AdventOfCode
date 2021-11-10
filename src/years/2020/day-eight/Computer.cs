namespace DayEight2020
{
    public record Instruction(string Type, int Argument);

    internal static class Computer
    {
        public static (int Accumulator, bool FinishedSuccessfully) RunInstructions(ImmutableArray<Instruction> instructions)
        {
            var hashset = new HashSet<Instruction>(ReferenceEqualityComparer.Instance);
            var accumulator = 0;
            var pointer = 0;
            while (true)
            {
                if (pointer == instructions.Length)
                {
                    break;
                }

                var instruction = instructions[pointer];
                if (hashset.Contains(instruction))
                {
                    return (accumulator, false);
                }

                hashset.Add(instruction);
                (accumulator, pointer) = instruction switch
                {
                    { Type: "nop" } => (accumulator, pointer + 1),
                    { Type: "acc" } => (accumulator + instruction.Argument, pointer + 1),
                    { Type: "jmp" } => (accumulator, pointer + instruction.Argument),
                    _ => throw new InvalidOperationException($"Invalid Instruction {instruction.Type}")
                };
            };

            return (accumulator, true);
        }
    }
}
