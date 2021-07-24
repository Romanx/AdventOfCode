using System.Collections.Immutable;
using System.Linq;
using System.Security.Cryptography;
using MoreLinq;
using NodaTime;
using Shared;

namespace DaySeventeen2018
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2018, 12, 17), "Reservoir Research");

        public override void PartOne(IInput input, IOutput output)
        {
            ImmutableHashSet<Point2d> clayPoints = input.ParseClayPoints();
            var (_, yRange) = Point2d.FindSpaceOfPoints(clayPoints);

            var before = output.File("before.txt");
            var after = output.File("after.txt");

            var scan = new Scan(clayPoints);
            scan.Write(before);

            scan.Flow(Scan.WaterSpring);
            scan.Write(after);

            var waterBlocks = scan.Map
                .Where(i => i.Key.Y >= yRange.Min)
                .Count(i => i.Value is CellType.FlowingWater or CellType.StillWater);

            output.WriteProperty("Number of flowing and still blocks", waterBlocks);
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            ImmutableHashSet<Point2d> clayPoints = input.ParseClayPoints();
            var (_, yRange) = Point2d.FindSpaceOfPoints(clayPoints);
            var scan = new Scan(clayPoints);
            scan.Flow(Scan.WaterSpring);

            var waterBlocks = scan.Map
                .Where(i => i.Key.Y >= yRange.Min)
                .Count(i => i.Value is CellType.StillWater);

            output.WriteProperty("Number of flowing and still blocks", waterBlocks);
        }
    }

    public enum CellType
    {
        Sand,
        Spring,
        FlowingWater,
        StillWater,
        Clay
    }
}
