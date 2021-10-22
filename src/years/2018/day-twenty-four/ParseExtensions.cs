using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using Shared;

namespace DayTwentyFour2018
{
    internal static class ParseExtensions
    {
        public static ImmutableDictionary<ArmyType, ImmutableList<Group>> Parse(this IInput input)
        {
            var paras = input.Lines.AsParagraphs();

            var dictionary = ImmutableDictionary.CreateBuilder<ArmyType, ImmutableList<Group>>();

            dictionary.Add(ParseArmy(paras[0]));
            dictionary.Add(ParseArmy(paras[1]));

            return dictionary.ToImmutable();
        }

        private static KeyValuePair<ArmyType, ImmutableList<Group>> ParseArmy(ReadOnlyMemory<ReadOnlyMemory<char>> lines)
        {
            var armyName = lines.Span[0].Span[..^1];
            var builder = ImmutableList.CreateBuilder<Group>();

            var type = armyName.SequenceEqual("Infection")
                ? ArmyType.Infection
                : ArmyType.ImmuneSystem;

            var groups = lines.Span[1..];
            foreach (var group in lines.Span[1..])
            {
                builder.Add(ParseGroup(group.ToString(), type));
            }

            return KeyValuePair.Create(type, builder.ToImmutable());
        }

        private static Group ParseGroup(string group, ArmyType armyType)
        {
            var match = groupRegex.Match(group);
            var unitCount = int.Parse(match.Groups["UnitCount"].Value);
            var hp = int.Parse(match.Groups["HP"].Value);
            var statusEffects = match.Groups["StatusEffects"].Value;
            var damage = int.Parse(match.Groups["Damage"].Value);
            var damageType = Enum.Parse<DamageType>(match.Groups["DamageType"].Value, true);
            var initiative = int.Parse(match.Groups["Initiative"].Value);
            var (immunities, weaknesses) = StatusEffects(statusEffects);

            return new(Guid.NewGuid(), armyType, unitCount, hp, damage, damageType, initiative, immunities, weaknesses);

            static (ImmutableArray<DamageType>, ImmutableArray<DamageType>) StatusEffects(string statusEffects)
            {
                if (statusEffects.Length == 0)
                    return (ImmutableArray<DamageType>.Empty, ImmutableArray<DamageType>.Empty);

                var immunitiesMatch = immunitiesRegex.Match(statusEffects);
                var immunities = immunitiesMatch.Groups.TryGetValue("Immunities", out var igroup) && igroup.Length > 0
                    ? igroup.Value.Split(',').Select(i => Enum.Parse<DamageType>(i, true)).ToImmutableArray()
                    : ImmutableArray<DamageType>.Empty;

                var weaknessesMatch = weaknessesRegex.Match(statusEffects);
                var weaknesses = weaknessesMatch.Groups.TryGetValue("Weaknesses", out var wgroup) && wgroup.Length > 0
                    ? wgroup.Value.Split(',').Select(i => Enum.Parse<DamageType>(i, true)).ToImmutableArray()
                    : ImmutableArray<DamageType>.Empty;

                return (immunities, weaknesses);
            }
        }

        private static readonly Regex immunitiesRegex = new("(?:immune to (?<Immunities>[a-zA-Z, ]*))");
        private static readonly Regex weaknessesRegex = new("(?:weak to (?<Weaknesses>[a-zA-Z, ]*))");
        private static readonly Regex groupRegex = new("(?<UnitCount>[0-9]*) .* (?<HP>[0-9]*) hit points(?: \\((?<StatusEffects>.*)\\))? with an attack that does (?<Damage>[0-9]*) (?<DamageType>.*) damage at initiative (?<Initiative>[0-9]*)");
    }
}
