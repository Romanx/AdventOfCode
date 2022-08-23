namespace DayOne2015
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2015, 12, 1), "Not Quite Lisp");

        public void PartOne(IInput input, IOutput output)
        {
            var floor = 0;
            foreach (var c in input.Content.AsSpan())
            {
                if (c == '(')
                {
                    floor++;
                }
                else if (c == ')')
                {
                    floor--;
                }
            }

            output.WriteProperty("Floor", floor);
        }

        public void PartTwo(IInput input, IOutput output)
        {
            var floor = 0;
            var span = input.Content.AsSpan();
            int i;
            for (i = 0; i < span.Length; i++)
            {
                var c = span[i];
                if (c == '(')
                {
                    floor++;
                }
                else if (c == ')')
                {
                    floor--;
                }

                if (floor == -1)
                    break;
            }

            output.WriteProperty("Position", i + 1);
        }
    }
}
