namespace DayTwentyTwo2015
{
    abstract record SpellBase(Guid Id, string Name, int ManaCost)
    {
        public abstract GameState Cast(GameState state);
    }

    record Spell(Guid Id, string Name, int ManaCost, Func<GameState, GameState> ApplyEffect)
        : SpellBase(Id, Name, ManaCost)
    {
        public override GameState Cast(GameState state)
        {
            var next = ApplyEffect(state);

            return next with
            {
                Wizard = next.Wizard with { Mana = next.Wizard.Mana - ManaCost },
                CastSpells = next.CastSpells.Add(this)
            };
        }
    }


    record Effect(
        Guid Id,
        string Name,
        int ManaCost,
        int Duration,
        Func<GameState, int, GameState> TurnEffect,
        Func<GameState, GameState> EndEffect)
        : SpellBase(Id, Name, ManaCost)
    {
        public override GameState Cast(GameState state)
        {
            return state with
            {
                Wizard = state.Wizard with { Mana = state.Wizard.Mana - ManaCost },
                ActiveSpells = state.ActiveSpells.Add(Id, Duration),
                CastSpells = state.CastSpells.Add(this),
                Log = state.Log.Add($"Player casts {Name}.")
            };
        }
    }
}
