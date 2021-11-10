namespace DaySixteen2017
{
    public class Challenge : ChallengeSync
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2017, 12, 16), "Permutation Promenade");

        public override void PartOne(IInput input, IOutput output)
        {
            var initial = "abcdefghijklmnop";
            var instructions = input.Content.ParseInstructions();

            var result = Dance(initial, instructions);

            output.WriteProperty("Before", initial);
            output.WriteProperty("After", result);
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            const int Target = 1_000_000_000;
            var seen = new HashSet<string>();

            var initial = "abcdefghijklmnop";
            seen.Add(initial);
            var instructions = input.Content.ParseInstructions();

            int i;

            for (i = 1; ; i++)
            {
                var result = Dance(initial, instructions);
                if (seen.Add(result) is false)
                {
                    break;
                }
                initial = result;
            }

            var list = seen.ToList();
            var end = i;

            var remainder = Target % end;

            var position = list[remainder];

            output.WriteProperty($"After {Target} loops", position);
        }

        private static string Dance(string initial, ImmutableArray<Instruction> instructions)
        {
            var state = (initial, instructions);

            return string.Create(initial.Length, state, static (span, state) =>
            {
                var (initial, instructions) = state;
                initial.AsSpan().CopyTo(span);

                foreach (var instruction in instructions)
                {
                    instruction.Apply(span);
                }
            });
        }
    }
}
