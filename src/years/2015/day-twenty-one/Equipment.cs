namespace DayTwentyOne2015
{
    record Equipment(string Name, int Cost, int Damage, int Armor)
    {
        public override string ToString()
            => $"{Name}, Cost = {Cost}, Damage = {Damage}, Armor = {Armor}";
    }
}
