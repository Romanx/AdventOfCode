using System.Diagnostics.CodeAnalysis;
using MoreLinq;

namespace DayTwentyFour2018
{
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

        public static IEnumerable<Group> RunWithBoostUntilWinner(ImmutableDictionary<ArmyType, ImmutableList<Group>> armies, int boost)
        {
            armies = armies.SetItem(ArmyType.ImmuneSystem, Boost(boost, armies[ArmyType.ImmuneSystem]));
            int? totalDeaths = null;
            while (totalDeaths != 0)
            {
                (armies, totalDeaths) = Round(armies);
            }

            return armies.Values.SelectMany(g => g);

            static ImmutableList<Group> Boost(int boost, IEnumerable<Group> groups) => groups.Select(b => b with { UnitDamage = b.UnitDamage + boost }).ToImmutableList();
        }

        private static (ImmutableDictionary<ArmyType, ImmutableList<Group>> Armies, int totalDeaths) Round(ImmutableDictionary<ArmyType, ImmutableList<Group>> armies)
        {
            var totalDeaths = 0;
            var targets = TargetSelection(armies);
            var units = armies.Values
                .SelectMany(g => g)
                .ToDictionary(k => k.Id, v => v);

            var order = units.Values
                .OrderByDescending(g => g.Initiative)
                .Select(g => g.Id)
                .ToArray();

            foreach (var attackerId in order)
            {
                if (InvalidGroup(attackerId, units, out var attacker))
                {
                    continue;
                }

                var defenderId = targets[attackerId];
                if (InvalidGroup(defenderId, units, out var defender))
                {
                    continue;
                }

                var damage = attacker.CalculateDamage(defender);

                var unitDeaths = damage / defender.UnitHP;
                var remainingUnits = Math.Max(0, defender.UnitCount - unitDeaths);
                totalDeaths += unitDeaths;

                units[defender.Id] = defender with { UnitCount = remainingUnits };
            }

            return (units.Values
                .Where(u => u.UnitCount > 0)
                .GroupBy(u => u.ArmyType)
                .ToImmutableDictionary(k => k.Key, v => v.ToImmutableList()), totalDeaths);

            static bool InvalidGroup(Guid? groupId, Dictionary<Guid, Group> units, [NotNullWhen(false)] out Group? group)
            {
                if (groupId is null)
                {
                    group = null;
                    return true;
                }

                group = units[groupId.Value];
                return group.UnitCount <= 0;
            }
        }

        private static ImmutableDictionary<Guid, Guid?> TargetSelection(IReadOnlyDictionary<ArmyType, ImmutableList<Group>> armies)
        {
            var results = ImmutableDictionary.CreateBuilder<Guid, Guid?>();
            var selectedTargets = new HashSet<Guid>();

            var allGroups = armies.Values.SelectMany(a => a)
                .OrderByDescending(x => (x.EffectivePower, x.Initiative));

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
                    .Select(target => (Target: target, Damage: attacker.CalculateDamage(target)))
                    .Where(t => selectedTargets.Contains(t.Target.Id) is false && t.Damage > 0)
                    .OrderByDescending(x => (x.Damage, x.Target.EffectivePower, x.Target.Initiative))
                    .Select(t => t.Target.Id)
                    .FirstOrDefault();

                return target == Guid.Empty
                    ? null
                    : target;
            }
        }
    }
}
