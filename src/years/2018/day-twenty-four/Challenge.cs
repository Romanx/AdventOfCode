using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using MoreLinq;
using NodaTime;
using Shared;

namespace DayTwentyFour2018
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2018, 12, 24), "Immune System Simulator 20XX");

        public override void PartOne(IInput input, IOutput output)
        {
            var armies = input.Parse();

            var combatants = Simulation.RunUntilWinner(armies);
            var armyType = combatants.First().ArmyType;
            var remainingUnits = combatants.Sum(g => g.UnitCount);

            output.WriteProperty("Winning Army", armyType);
            output.WriteProperty("Remaining Units", remainingUnits);
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var armies = input.Parse();

            var range = 0..short.MaxValue;

            var boost = range.BinarySearch(midpoint =>
            {
                var combatants = Simulation.RunWithBoostUntilWinner(armies, midpoint);
                if (combatants.All(g => g.ArmyType == ArmyType.ImmuneSystem))
                {
                    return BinarySearchResult.Lower;
                }
                else
                {
                    return BinarySearchResult.Upper;
                }
            });

            var remaining = Simulation.RunWithBoostUntilWinner(armies, boost);
            var armyType = remaining.First().ArmyType;
            var remainingUnits = remaining.Sum(g => g.UnitCount);

            output.WriteProperty("Winning Army", armyType);
            output.WriteProperty("Boost", boost);
            output.WriteProperty("Remaining Units", remainingUnits);
        }
    }

    record Army(string Name, ImmutableList<Group> Groups);

    record Group(Guid Id, ArmyType ArmyType, int UnitCount, int UnitHP, int UnitDamage, DamageType DamageType, int Initiative, ImmutableArray<DamageType> Immunities, ImmutableArray<DamageType> Weaknesses)
    {
        public int EffectivePower => UnitCount * UnitDamage;

        public int CalculateDamage(Group other)
        {
            if (other.ArmyType == ArmyType)
                return 0;

            return EffectivePower * DamageType switch
            {
                var x when other.Immunities.Contains(x) => 0,
                var x when other.Weaknesses.Contains(x) => 2,
                _ => 1
            };
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append($"[{Id} - {ArmyType}]: {UnitCount} units each with {UnitHP} hit points ");
            if (Immunities.Length > 0 || Weaknesses.Length > 0)
            {
                builder.Append('(');
                if (Immunities.Length > 0)
                {
                    builder.Append($"immune to {string.Join(", ", Immunities).ToLowerInvariant()}");
                }
                if (Immunities.Length > 0 && Weaknesses.Length > 0)
                {
                    builder.Append("; ");
                }
                if (Weaknesses.Length > 0)
                {
                    builder.Append($"weak to {string.Join(", ", Weaknesses).ToLowerInvariant()}");
                }
                builder.Append(") ");
            }
            builder.Append($"with an attack that does {UnitDamage} {DamageType.ToString().ToLower()} damage at initiative {Initiative}");
            return builder.ToString();
        }
    }

    enum ArmyType
    {
        ImmuneSystem,
        Infection
    }

    enum DamageType
    {
        Fire,
        Radiation,
        Bludgeoning,
        Slashing,
        Cold
    }
}
