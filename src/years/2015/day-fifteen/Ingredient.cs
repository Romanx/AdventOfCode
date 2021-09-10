namespace DayFifteen2015
{
    record Ingredient(string Name, int Capacity, int Durability, int Flavor, int Texture, int Calories)
    {
        public override string ToString()
            => $"Name: {Name}, Capacity: {Capacity}, Durability: {Durability}, Flavor: {Flavor}, Texture: {Texture}, Calories: {Calories}";
    }
}
