namespace DayTwentyTwo2015
{
    class GamePipelineBuilder
    {
        private readonly ImmutableDictionary<Guid, ImmutableArray<StateTransition>> _cache;

        public GamePipelineBuilder(bool hardMode)
        {
            var builder = ImmutableDictionary.CreateBuilder<Guid, ImmutableArray<StateTransition>>();

            foreach (var spell in SpellBook.Spells)
            {
                builder.Add(spell.Id, BuildPipeline(spell, hardMode));
            }

            _cache = builder.ToImmutableDictionary();
        }

        public ImmutableArray<StateTransition> GetPipeline(SpellBase spell)
            => _cache[spell.Id];

        private static ImmutableArray<StateTransition> BuildPipeline(SpellBase spell, bool hardMode)
        {
            var wizardTurn = new StateTransition[]
            {
                state => StartTurn(state, EntityType.Wizard),
                hardMode
                    ? ApplyHardModeDrain
                    : static state => (state, null),
                ApplyActiveEffects,
                state => CastSpell(state, spell)
            };

            var bossTurn = new StateTransition[]
            {
                state => StartTurn(state, EntityType.Boss),
                ApplyActiveEffects,
                BossAttack
            };

            return wizardTurn.Concat(bossTurn).ToImmutableArray();

            static (GameState NextState, Result? Result) StartTurn(GameState state, EntityType type)
            {
                var message = $@"
-- {type} turn --
- Wizard has {state.Wizard.HP} hit points, {state.Wizard.Armor} armor, {state.Wizard.Mana} mana
- Boss has {state.Boss.HP} hit points";

                var next = state with
                {
                    Turn = state.Turn + 1,
                    Log = state.Log.Add(message)
                };

                return (next, null);
            }

            static (GameState NextState, Result? Result) BossAttack(GameState state)
            {
                var damage = Math.Max(1, state.Boss.Attack - state.Wizard.Armor);
                var next = state with
                {
                    Wizard = state.Wizard with
                    {
                        HP = state.Wizard.HP - damage
                    },
                    Log = state.Log.Add($"Boss attacks for {state.Boss.Attack} - {state.Wizard.Armor} = {damage} damage!")
                };
                return (next, CheckEntityAlive(next.Wizard, Result.BossWin));
            }

            static (GameState NextState, Result? Result) ApplyActiveEffects(GameState state)
            {
                if (state.ActiveSpells.Count == 0)
                {
                    return (state, null);
                }

                var builder = ImmutableDictionary.CreateBuilder<Guid, int>();

                foreach (var (effectId, turnsLeft) in state.ActiveSpells)
                {
                    var effect = SpellBook.Effects[effectId];

                    state = effect.TurnEffect(state, turnsLeft - 1);
                    if (turnsLeft == 1)
                    {
                        state = effect.EndEffect(state);
                    }
                    else
                    {
                        builder.Add(effectId, turnsLeft - 1);
                    }
                }

                var next = state with
                {
                    ActiveSpells = builder.ToImmutable()
                };

                return (next, CheckEntityAlive(next.Boss, Result.WizardWin));
            }

            static (GameState NextState, Result? Result) ApplyHardModeDrain(GameState state)
            {
                var next = state with
                {
                    Wizard = state.Wizard with { HP = state.Wizard.HP - 1 },
                    Log = state.Log.Add($"Wizard takes 1 damage from drain"),
                };

                return (next, CheckEntityAlive(next.Wizard, Result.BossWin));
            }

            static (GameState NextState, Result? Result) CastSpell(GameState state, SpellBase spell)
            {
                if (state.ActiveSpells.ContainsKey(spell.Id))
                {
                    return (state, Result.Invalid);
                }
                else if (spell.ManaCost > state.Wizard.Mana)
                {
                    return (state, Result.Invalid);
                }

                var next = spell.Cast(state);

                return (next, CheckEntityAlive(next.Boss, Result.WizardWin));
            }

            static Result? CheckEntityAlive(Entity entity, Result resultIfDead) => entity.HP <= 0 ? resultIfDead : null;
        }
    }
}
