using System;
using System.Collections.Immutable;
using System.Linq;

namespace DayTwentyTwo2015
{
    static class SpellBook
    {
        public static SpellBase MagicMissle { get; } = new Spell(Guid.NewGuid(), "Magic Missle", 53, state => state with
        {
            Boss = state.Boss with { HP = state.Boss.HP - 4 },
            Log = state.Log.Add("Player casts Magic Missile, dealing 4 damage.")
        });

        public static SpellBase Drain { get; } = new Spell(Guid.NewGuid(), "Drain", 73, state => state with
        {
            Boss = state.Boss with { HP = state.Boss.HP - 2 },
            Wizard = state.Wizard with { HP = state.Wizard.HP + 2 },
            Log = state.Log.Add("Player casts Drain, dealing 2 damage, and healing 2 hit points.")
        });

        public static SpellBase Shield { get; } = new Effect(Guid.NewGuid(), "Shield", 113, 6,
            (state, turnsLeft) => state with
            {
                Wizard = state.Wizard with { Armor = 7 },
                Log = state.Log.Add($"Shield's timer is now {turnsLeft}.")
            },
            state => state with
            {
                Wizard = state.Wizard with { Armor = 0 },
                Log = state.Log.Add("Shield wears off, decreasing armor by 7.")
            });

        public static SpellBase Poison { get; } = new Effect(Guid.NewGuid(), "Poison", 173, 6,
            (state, turnsLeft) => state with
            {
                Boss = state.Boss with { HP = state.Boss.HP - 3 },
                Log = state.Log.Add($"Poison deals 3 damage; its timer is now {turnsLeft}.")
            },
            state => state with
            {
                Log = state.Log.Add("Poison wears off.")
            });

        public static SpellBase Recharge { get; } = new Effect(Guid.NewGuid(), "Recharge", 229, 5,
            (state, turnsLeft) => state with
            {
                Wizard = state.Wizard with { Mana = state.Wizard.Mana + 101 },
                Log = state.Log.Add($"Recharge provides 101 mana; its timer is now {turnsLeft}.")
            },
            state => state with
            {
                Log = state.Log.Add("Recharge wears off.")
            });

        public static ImmutableArray<SpellBase> Spells { get; } = new SpellBase[]
        {
            MagicMissle,
            Drain,
            Shield,
            Poison,
            Recharge,
        }.ToImmutableArray();

        public static ImmutableDictionary<Guid, Effect> Effects { get; } = Spells
            .Where(s => s is Effect)
            .Cast<Effect>()
            .ToImmutableDictionary(k => k.Id, v => v);
    }
}
