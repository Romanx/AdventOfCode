namespace DayEleven2015
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2015, 12, 11), "Corporate Policy");

        public void PartOne(IInput input, IOutput output)
        {
            var nextPassword = PasswordGenerator.GeneratePassword(input.Content.AsString());
            output.WriteProperty("Next password", nextPassword);
        }

        public void PartTwo(IInput input, IOutput output)
        {
            var nextPassword = PasswordGenerator.GeneratePassword(input.Content.AsString());
            nextPassword = PasswordGenerator.GeneratePassword(nextPassword);
            output.WriteProperty("Next password", nextPassword);
        }
    }
}
