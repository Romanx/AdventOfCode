namespace DayFour2017
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2017, 12, 4), "High-Entropy Passphrases");

        public void PartOne(IInput input, IOutput output)
        {
            var validPhrases = input.Lines.ToPassphrase()
                .Count(passphrase => ValidatePassphrase(passphrase));

            output.WriteProperty("Valid passphrases", validPhrases);

            static bool ValidatePassphrase(string[] passphrase)
            {
                return passphrase.Distinct().Count() == passphrase.Length;
            }
        }

        public void PartTwo(IInput input, IOutput output)
        {
            var validPhrases = input.Lines.ToPassphrase()
                .Count(passphrase => ValidatePassphrase(passphrase));

            output.WriteProperty("Valid passphrases", validPhrases);

            static bool ValidatePassphrase(string[] passphrase)
            {
                for (var i = 0; i < passphrase.Length; i++)
                {
                    for (var y = i + 1; y < passphrase.Length; y++)
                    {
                        if (Anagram(passphrase[i], passphrase[y]))
                        {
                            return false;
                        }
                    }
                }

                return true;
            }

            static bool Anagram(string a, string b)
            {
                var aa = a.ToCharArray();
                var bb = b.ToCharArray();
                Array.Sort(aa);
                Array.Sort(bb);

                return aa.SequenceEqual(bb);
            }
        }
    }

    internal static class ParseExtensions
    {
        public static IEnumerable<string[]> ToPassphrase(this IInputLines lines) =>
            lines.Transform(str => str.Split(' ', StringSplitOptions.RemoveEmptyEntries & StringSplitOptions.TrimEntries));
    }
}
