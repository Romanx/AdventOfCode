using System.Diagnostics;

namespace DayFifteen2018
{
    public class Challenge : ChallengeSync
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2018, 12, 15), "Beverage Bandits");

        public override void PartOne(IInput input, IOutput output)
        {
            var game = input.Parse();
            var initalState = game.Print();
            game.RunGameToEnd();
            output.WriteOutcome(game, initalState);
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var initalState = input.Parse().Print();
            var range = 4..256;

            var attackPower = range.BinarySearch(midpoint =>
            {
                (var elvesLive, _) = RunSimulation(input, (uint)midpoint);
                if (elvesLive)
                {
                    return BinarySearchResult.Lower;
                }

                return BinarySearchResult.Upper;
            });

            var (_, game) = RunSimulation(input, (uint)attackPower);

            output.WriteProperty("Attack Power Found", attackPower);
            output.WriteOutcome(game!, initalState);

            static (bool elvesLive, Game? game) RunSimulation(IInput input, uint elfAttack)
            {
                var game = input.Parse(elfAttack);
                while (true)
                {
                    if (game.Step() is false)
                        break;

                    if (game.Entities.Any(e => e.EntityType == EntityType.Elf && e.IsDead))
                        return (false, null);
                }

                if (game.Entities.Any(e => e.EntityType == EntityType.Elf && e.IsDead))
                {
                    return (false, null);
                }

                return (true, game);
            }
        }
    }

    public class ReadingOrderComparer : IComparer<Point2d>
    {
        public static IComparer<Point2d> Instance { get; } = new ReadingOrderComparer();

        public int Compare(Point2d? x, Point2d? y)
        {
            Debug.Assert(x is not null);
            Debug.Assert(y is not null);
            return (x.Column, x.Row).CompareTo((y.Column, y.Row));
        }
    }

    enum CellType
    {
        Wall,
        OpenSpace
    }

    enum EntityType
    {
        Goblin,
        Elf
    }
}
