using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MoreLinq;
using Shared2019;

namespace DayTwentyFive2019
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2019, 12, 25), "Cryostasis");

        public async Task PartOne(IInput input, IOutput output)
        {
            var program = input.AsIntcodeProgram();
            var droid = new ScanningDroid(program);
            await droid.SeekToAirlock();
            var result = await BruteForceDoor(droid, output);
            var passcode = Regex.Match(result, "typing ([0-9]*) on the keypad at the main airlock.").Groups[1].Value;
            output.WriteProperty("Passcode", passcode);
        }

        private static async Task<string> BruteForceDoor(ScanningDroid droid, IOutput output)
        {
            var allItems = await droid.GetInventory();
            var allSubsets = allItems.Subsets();
            await droid.Drop(allItems);

            foreach (var subset in allSubsets)
            {
                if (subset.Count == 0)
                    continue;

                await droid.Take(subset);
                var result = await droid.Go(Direction.South);

                if (result is not null && result.Contains("you are ejected back to the checkpoint.") is false)
                {
                    output.WriteProperty("Items Used", string.Join(", ", subset));
                    return result;
                }
                await droid.Drop(subset);
            }

            throw new InvalidOperationException("We can't get to the right weight!");
        }
    }

    record SearchState(string Room, ImmutableHashSet<string> Items, ImmutableArray<Direction> Doors);

    enum TileState
    {
        Explored,
        Unknown
    }
}
