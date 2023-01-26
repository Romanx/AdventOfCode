using Generator.Equals;

namespace DaySeventeen2022;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2022, 12, 17), "Pyroclastic Flow");

    public void PartOne(IInput input, IOutput output)
    {
        const int rounds = 2022;
        var chamber = Chamber.Create(input);
        var rocks = InfiniteQueue.Create(Rocks.All);

        for (var i = 0; i < rounds; i++)
        {
            var rock = rocks.Dequeue();
            chamber.DropRock(rock);
        }

        output.WriteProperty("Final Height", chamber.Height);
    }

    public void PartTwo(IInput input, IOutput output)
    {
        const long rounds = 1_000_000_000_000 - 1;

        var chamber = Chamber.Create(input);
        var rocks = InfiniteQueue.Create(Rocks.All);
        var seen = new Dictionary<SearchState, (int BlockCount, int Height)>();

        while (true)
        {
            var rock = rocks.Dequeue();
            chamber.DropRock(rock);

            var state = new SearchState(
                chamber.BuildNormalisedCeiling(),
                chamber.Pattern.Index,
                rocks.Index);

            if (seen.TryGetValue(state, out var existing))
            {
                var (blockCountAtLoopStart, heightAtLoopStart) = existing;
                var blocksPerLoop = chamber.BlockCount - 1L - blockCountAtLoopStart;
                var totalLoops = (rounds - blockCountAtLoopStart) / blocksPerLoop;

                var remainingBlocksFromClosestLoopToGoal = (rounds - blockCountAtLoopStart) - (totalLoops * blocksPerLoop);
                var heightGainedSinceLoop = chamber.Height - heightAtLoopStart;

                // Drop the remaining rocks
                for (var i = 0; i < remainingBlocksFromClosestLoopToGoal; i++)
                {
                    rock = rocks.Dequeue();
                    chamber.DropRock(rock);
                }

                var result = chamber.Height + (heightGainedSinceLoop * (totalLoops - 1));
                output.WriteProperty("Final Height", result);
                break;
            }

            seen[state] = (chamber.BlockCount - 1, chamber.Height);
        }
    }
}

internal static class Rocks
{
    public static ImmutableArray<Rock> All { get; } = ImmutableArray.Create(
        Rock.Parse("####"),
        Rock.Parse(
            """
            .#.
            ###
            .#.
            """),
        Rock.Parse(
            """
            ..#
            ..#
            ###
            """),
        Rock.Parse(
            """
            #
            #
            #
            #
            """),
        Rock.Parse(
            """
            ##
            ##
            """)
        );
}

[Equatable]
readonly partial record struct SearchState(
    [property: OrderedEquality] ImmutableArray<int> Ceiling,
    int JetIndex,
    int BlockIndex) : IEquatable<SearchState>;
