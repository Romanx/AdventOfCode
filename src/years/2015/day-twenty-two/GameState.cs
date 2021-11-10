namespace DayTwentyTwo2015
{
    record GameState(
        Wizard Wizard,
        Boss Boss,
        int Turn,
        ImmutableDictionary<Guid, int> ActiveSpells,
        ImmutableArray<SpellBase> CastSpells,
        ImmutableArray<string> Log)
    {
        public GameState(Wizard wizard, Boss boss)
            : this(wizard, boss, 1, ImmutableDictionary<Guid, int>.Empty, ImmutableArray<SpellBase>.Empty, ImmutableArray<string>.Empty)
        {
        }

        public int TotalManaSpent => CastSpells.Sum(m => m.ManaCost);
    }

    record Entity(int HP, EntityType EntityType);

    record Wizard(int HP, int Armor, int Mana) : Entity(HP, EntityType.Wizard);

    record Boss(int HP, int Attack) : Entity(HP, EntityType.Boss);

    enum EntityType
    {
        Boss,
        Wizard
    }

    enum Result
    {
        BossWin,
        WizardWin,
        Continue,
        Invalid
    }
}
