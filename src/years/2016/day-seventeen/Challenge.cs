namespace DaySeventeen2016
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2016, 12, 17), "Two Steps Forward");

        public void PartOne(IInput input, IOutput output)
        {
            var passcode = input.Content.AsString();
            var vault = new Vault(passcode);
            var path = vault.Search(string.Empty).FirstOrDefault();

            output.WriteProperty("Passcode", passcode);

            var shortest = path is not null
                ? $"{path} : ({path.Length})"
                : "[None Found]";

            output.WriteProperty("Shortest Path", shortest);
        }

        public void PartTwo(IInput input, IOutput output)
        {
            var passcode = input.Content.AsString();
            var vault = new Vault(passcode);
            var path = vault
                .Search(string.Empty)
                .MaxBy(path => path.Length);

            output.WriteProperty("Passcode", passcode);

            var longest = path is not null
                ? $"{path} : ({path.Length})"
                : "[None Found]";

            output.WriteProperty("Longest Path", longest);
        }
    }
}
