using System.Text;

namespace DayTwentyOne2015
{
    class Unit
    {
        private readonly int _baseDamage;
        private readonly int _baseArmor;

        public Unit(UnitType unitType, int hitPoints, int damage, int armor, ImmutableArray<Equipment> equipment)
        {
            UnitType = unitType;
            HitPoints = hitPoints;
            Equipment = equipment;

            _baseDamage = damage;
            _baseArmor = armor;
            var cost = 0;
            foreach (var e in equipment)
            {
                damage += e.Damage;
                armor += e.Armor;
                cost += e.Cost;
            }
            Damage = damage;
            Armor = armor;
            TotalCost = cost;
        }

        public UnitType UnitType { get; init; }
        public int HitPoints { get; init; }
        public int Damage { get; init; }
        public int Armor { get; init; }
        public ImmutableArray<Equipment> Equipment { get; init; }

        public int TotalCost { get; }

        public int CalculateDeathTurn(Unit other)
        {
            return (int)MathF.Ceiling(HitPoints / (float)Math.Max(other.Damage - Armor, 1));
        }

        public Unit CalculateStateOnTurn(int turn, Unit other)
        {
            var damagePerTurn = Math.Max(other.Damage - Armor, 1);

            return new Unit(
                UnitType,
                HitPoints - (damagePerTurn * turn),
                _baseDamage,
                _baseArmor,
                Equipment);
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine($"[{UnitType}]: HP = {HitPoints}, Damage = {Damage}, Armor = {Armor}, Cost = {TotalCost}");
            foreach (var equipment in Equipment)
            {
                builder.AppendLine($"  - {equipment}");
            }

            return builder.ToString();
        }

        internal Unit WithEquipment(ImmutableArray<Equipment> equipment)
        {
            return new Unit(
                UnitType,
                HitPoints,
                _baseDamage,
                _baseArmor,
                equipment
            );
        }
    }
}
