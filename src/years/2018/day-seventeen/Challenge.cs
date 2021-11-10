using System.ComponentModel.DataAnnotations;
using SixLabors.ImageSharp;

namespace DaySeventeen2018
{
    public class Challenge : ChallengeSync
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
            output.AddImage("before.png", ImageWriter.Generate(scan.Map));

            scan.Flow(Scan.WaterSpring);
            scan.Write(after);
            output.AddImage("after.png", ImageWriter.Generate(scan.Map));

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

        public class ImageWriter : GridImageWriter<CellType>
        {
            private ImageWriter(IReadOnlyDictionary<Point2d, CellType> map) : base(map)
            {
            }

            public static Image Generate(ImmutableDictionary<Point2d, CellType> map) => new ImageWriter(map).Generate();

            protected override Color GetColorForPoint(Point2d point)
            {
                var cellType = _map.TryGetValue(point, out var ct)
                    ? ct
                    : CellType.Sand;

                return cellType switch
                {
                    CellType.Sand => Color.FromRgb(194, 178, 128),
                    CellType.Spring => Color.FromRgb(2, 75, 134),
                    CellType.FlowingWater => Color.FromRgb(223, 251, 251),
                    CellType.StillWater => Color.FromRgb(20, 114, 201),
                    CellType.Clay => Color.FromRgb(107, 104, 103),
                    _ => throw new NotImplementedException(),
                };
            }
        }
    }

    public enum CellType
    {
        [Display(Name = ".")]
        Sand,
        [Display(Name = "+")]
        Spring,
        [Display(Name = "|")]
        FlowingWater,
        [Display(Name = "~")]
        StillWater,
        [Display(Name = "#")]
        Clay
    }
}
