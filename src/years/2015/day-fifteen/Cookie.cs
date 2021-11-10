using System.Text;

namespace DayFifteen2015
{
    record Cookie
    {
        public Cookie(ImmutableArray<Ingredient> ingredients, ImmutableArray<int> ratios)
        {
            Ingredients = ingredients;
            Ratios = ratios;

            var capacity = 0;
            var durability = 0;
            var flavor = 0;
            var texture = 0;
            var calories = 0;

            for (var i = 0; i < ingredients.Length; i++)
            {
                var ingredient = ingredients[i];
                if (ratios.Length <= i)
                {
                    break;
                }
                var ratio = ratios[i];

                capacity += ratio * ingredient.Capacity;
                durability += ratio * ingredient.Durability;
                flavor += ratio * ingredient.Flavor;
                texture += ratio * ingredient.Texture;
                calories += ratio * ingredient.Calories;
            }

            Capacity = capacity;
            Durability = durability;
            Flavor = flavor;
            Texture = texture;
            Score = Math.Max(Capacity, 0) *
                    Math.Max(Durability, 0) *
                    Math.Max(Flavor, 0) *
                    Math.Max(Texture, 0);

            Calories = calories;
        }

        public int Capacity { get; }
        public int Durability { get; }
        public int Flavor { get; }
        public int Texture { get; }
        public int Score { get; }
        public int Calories { get; }

        public ImmutableArray<Ingredient> Ingredients { get; }

        public ImmutableArray<int> Ratios { get; }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine($"Capacity: {Capacity}");
            builder.AppendLine($"Durability: {Durability}");
            builder.AppendLine($"Flavor: {Flavor}");
            builder.AppendLine($"Texture: {Texture}");
            builder.AppendLine($"Score: {Score}");
            builder.AppendLine("Ingredients:");
            for (var i = 0; i < Ingredients.Length; i++)
            {
                builder.AppendLine($"  - {Ratios[i]} teaspoons of {Ingredients[i].Name}");
            }

            return builder.ToString().Trim();
        }
    }
}
