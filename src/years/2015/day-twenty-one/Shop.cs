namespace DayTwentyOne2015
{
    static class Shop
    {
        public static ImmutableArray<Equipment> Weapons { get; } = ImmutableArray.Create(
            new Equipment("Dagger", 8, 4, 0),
            new Equipment("Shortsword", 10, 5, 0),
            new Equipment("Warhammer", 25, 6, 0),
            new Equipment("Longsword", 40, 7, 0),
            new Equipment("Greataxe", 74, 8, 0)
        );

        public static ImmutableArray<Equipment> Armor { get; } = ImmutableArray.Create(
            new Equipment("None", 0, 0, 0),
            new Equipment("Leather", 13, 0, 1),
            new Equipment("Chainmail", 31, 0, 2),
            new Equipment("Splintmail", 53, 0, 3),
            new Equipment("Bandedmail", 75, 0, 4),
            new Equipment("Platemail", 102, 0, 5)
        );

        public static ImmutableArray<Equipment> Rings { get; } = ImmutableArray.Create(
            new Equipment("None", 0, 0, 0),
            new Equipment("Damage +1", 25, 1, 0),
            new Equipment("Damage +2", 50, 2, 0),
            new Equipment("Damage +3", 100, 3, 0),
            new Equipment("Defense +1", 20, 0, 1),
            new Equipment("Defense +2", 40, 0, 2),
            new Equipment("Defense +3", 80, 0, 3)
        );
    }
}
