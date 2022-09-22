using System.Threading.Tasks;
using Shared2019;

namespace DayThirteen2019;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2019, 12, 13), "Care Package");

    public async Task PartOne(IInput input, IOutput output)
    {
        var cabinet = new ArcadeCabinet(input.AsIntcodeProgram());

        var blocks = await cabinet.WatchLoadScreen();

        output.WriteProperty("Number of blocks", blocks);
    }

    public async Task PartTwo(IInput input, IOutput output)
    {
        var program = input.AsIntcodeProgram();
        var freePlay = program.SetItem(0, 2);

        var cabinet = new ArcadeCabinet(freePlay);
        var score = await cabinet.Play();

        output.WriteProperty("Score", score);
    }
}
