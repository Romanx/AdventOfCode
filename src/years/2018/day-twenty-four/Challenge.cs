using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
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
            for (var i = 0; ; i++)
            {
                var boostedArmy = armies.SetItem(ArmyType.ImmuneSystem, Boost(i, armies[ArmyType.ImmuneSystem]));
                var combatants = Simulation.RunUntilWinner(boostedArmy);
                if (combatants.All(g => g.ArmyType == ArmyType.ImmuneSystem))
                {
                    var armyType = combatants.First().ArmyType;
                    var remainingUnits = combatants.Sum(g => g.UnitCount);

                    output.WriteProperty("Winning Army", armyType);
                    output.WriteProperty("Boost", i);
                    output.WriteProperty("Remaining Units", remainingUnits);
                    break;
                }
            }

            static ImmutableList<Group> Boost(int boost, IEnumerable<Group> groups) => groups.Select(b => b with { UnitDamage = b.UnitDamage + boost }).ToImmutableList();
        }
    }

    static class Simulation
    {
        public static IEnumerable<Group> RunUntilWinner(ImmutableDictionary<ArmyType, ImmutableList<Group>> armies)
        {
            int? totalDeaths = null;
            while (totalDeaths != 0)
            {
                (armies, totalDeaths) = Round(armies);
            }

            return armies.Values.SelectMany(g => g);
        }

        private static (ImmutableDictionary<ArmyType, ImmutableList<Group>> Armies, int TotalDeaths) Round(ImmutableDictionary<ArmyType, ImmutableList<Group>> armies)
        {
            var totalDeaths = 0;
            var targets = TargetSelection(armies);
            var units = armies.Values.SelectMany(g => g)
                .ToDictionary(k => k.Id, v => v);

            var order = units.Values
                .OrderByDescending(g => g.Initiative)
                .Select(g => g.Id)
                .ToArray();

            foreach (var groupId in order)
            {
                if (InvalidGroup(groupId, units, out var attacker))
                {
                    continue;
                }

                var defenderId = targets[groupId];
                if (InvalidGroup(defenderId, units, out var defender))
                {
                    continue;
                }
                var damage = CalculateDamage(attacker, defender);

                var unitDeaths = damage / defender.UnitHP;
                var remainingUnits = defender.UnitCount - unitDeaths;
                totalDeaths += unitDeaths;

                units[defenderId!.Value] = defender with { UnitCount = remainingUnits };
            }

            return (units.Values
                .Where(u => u.GroupHP > 0)
                .GroupBy(u => u.ArmyType)
                .ToImmutableDictionary(k => k.Key, v => v.ToImmutableList()), totalDeaths);

            static bool InvalidGroup(Guid? groupId, Dictionary<Guid, Group> units, out Group group)
            {
                if (groupId is null)
                {
                    group = null;
                    return true;
                }

                group = units[groupId.Value];
                return group.GroupHP <= 0;
            }
        }

        private static ImmutableDictionary<Guid, Guid?> TargetSelection(IReadOnlyDictionary<ArmyType, ImmutableList<Group>> armies)
        {
            var results = ImmutableDictionary.CreateBuilder<Guid, Guid?>();
            var selectedTargets = new HashSet<Guid>();

            var allGroups = armies.Values.SelectMany(a => a)
                .OrderByDescending(x => x.EffectivePower)
                .ThenByDescending(x => x.Initiative);

            foreach (var group in allGroups)
            {
                var enemyType = group.ArmyType == ArmyType.ImmuneSystem
                    ? ArmyType.Infection
                    : ArmyType.ImmuneSystem;

                if (armies.TryGetValue(enemyType, out var enemyGroups) is false)
                {
                    enemyGroups = ImmutableList<Group>.Empty;
                }

                var target = BestTarget(
                    group,
                    enemyGroups,
                    selectedTargets);

                results[group.Id] = target;
                if (target is not null)
                {
                    selectedTargets.Add(target.Value);
                }
            }

            return results.ToImmutable();

            Guid? BestTarget(Group attacker, IEnumerable<Group> targets, HashSet<Guid> selectedTargets)
            {
                var target = targets
                    .Select(t => (Target: t, Damage: CalculateDamage(attacker, t)))
                    .Where(t => selectedTargets.Contains(t.Target.Id) is false && t.Damage > 0)
                    .OrderByDescending(x => x.Damage)
                    .ThenByDescending(x => x.Target.EffectivePower)
                    .ThenByDescending(x => x.Target.Initiative)
                    .Select(t => t.Target.Id)
                    .FirstOrDefault();

                return target == Guid.Empty
                    ? null
                    : target;
            }
        }

        private static int CalculateDamage(Group attacker, Group defender)
        {
            return attacker.EffectivePower * attacker.DamageType switch
            {
                var x when defender.Immunities.Contains(x) => 0,
                var x when defender.Weaknesses.Contains(x) => 2,
                _ => 1
            };
        }
    }

    record Army(string Name, ImmutableList<Group> Groups);

    record Group(Guid Id, ArmyType ArmyType, int UnitCount, int UnitHP, int UnitDamage, DamageType DamageType, int Initiative, ImmutableArray<DamageType> Immunities, ImmutableArray<DamageType> Weaknesses)
    {
        public int EffectivePower => UnitCount * UnitDamage;

        public int GroupHP => UnitCount * UnitHP;
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
