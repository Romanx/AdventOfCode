using System.Diagnostics;

namespace DayFifteen2018
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2018, 12, 15), "Beverage Bandits");

        public void PartOne(IInput input, IOutput output)
        {
            var game = input.Parse();
            var initalState = game.Print();
            game.RunGameToEnd();
            output.WriteOutcome(game, initalState);
        }

        public void PartTwo(IInput input, IOutput output)
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
