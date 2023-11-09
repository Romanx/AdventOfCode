namespace DayThirteen2018;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2018, 12, 13), "Mine Cart Madness");

    public void PartOne(IInput input, IOutput output)
    {
        var track = input.Parse();
        var collision = track.Collisions().First();

        output.WriteProperty("First collision at", collision);
    }

    public void PartTwo(IInput input, IOutput output)
    {
        var track = input.Parse();
        var collision = track.Collisions().ToArray();
        var cart = track.Minecarts.First(it => it.Alive);

        output.WriteProperty("Last cart at", cart.Position);
    }
}
