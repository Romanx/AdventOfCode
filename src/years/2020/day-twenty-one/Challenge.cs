using System.Text.RegularExpressions;

namespace DayTwentyOne2020
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2020, 12, 21), "Allergen Assessment");

        public void PartOne(IInput input, IOutput output)
        {
            var foodList = input.ParseFoodList();
            var (safeIngredients, _) = FindDangerousAndSafeIngredients(foodList);

            var recipes = foodList
                .SelectMany(r => r.Ingredients)
                .Where(i => safeIngredients.Contains(i))
                .Count();

            output.WriteProperty("Allergen Free Ingedients", string.Join(", ", safeIngredients));
            output.WriteProperty("Times in recipe", recipes);
        }

        public void PartTwo(IInput input, IOutput output)
        {
            var foodList = input.ParseFoodList();
            var (_, unsafeIngredients) = FindDangerousAndSafeIngredients(foodList);

            var ordered = unsafeIngredients
                .OrderBy(a => a.Key)
                .Select(a => a.Value);

            output.WriteProperty("Dangerous Ingredients", string.Join(",", ordered));
        }

        private static (ImmutableHashSet<string> SafeIngredients, ImmutableDictionary<string, string> DangerousIngredients) FindDangerousAndSafeIngredients(ImmutableArray<Food> foodList)
        {
            var allergenMap = foodList
                .SelectMany(f => f.Allergens.Select(allergen => (Allergen: allergen, f.Ingredients)))
                .GroupBy(f => f.Allergen)
                .ToImmutableDictionary(f => f.Key, v => v.Select(i => i.Ingredients).ToImmutableArray());

            var dangerousIngredients = ImmutableDictionary.CreateBuilder<string, string>();
            var unseenIngredients = ImmutableHashSet.CreateBuilder<string>();
            unseenIngredients.UnionWith(foodList.SelectMany(f => f.Ingredients));

            while (allergenMap.Count > 0)
            {
                var matches = allergenMap
                    .OrderByDescending(kvp => kvp.Value.Length)
                    .Select(kvp => FindMatch(kvp.Key, kvp.Value))
                    .Where(ic => ic.HasValue)
                    .Select(ic => ic!.Value);

                foreach (var (allergen, ingredient) in matches)
                {
                    dangerousIngredients.Add(allergen, ingredient);
                    unseenIngredients.Remove(ingredient);

                    allergenMap = UpdateAllergenMap(allergenMap, allergen, ingredient);
                }
            }

            return (unseenIngredients.ToImmutable(), dangerousIngredients.ToImmutable());

            static ImmutableDictionary<string, ImmutableArray<ImmutableHashSet<string>>> UpdateAllergenMap(
                ImmutableDictionary<string, ImmutableArray<ImmutableHashSet<string>>> allergenMap,
                string allergen,
                string ingredient)
            {
                var builder = allergenMap.ToBuilder();
                builder.Remove(allergen);

                foreach (var (key, values) in allergenMap)
                {
                    if (key == allergen)
                        continue;

                    builder[key] = values.Select(r => r.Remove(ingredient)).ToImmutableArray();
                }

                return builder.ToImmutable();
            }

            static (string Allergen, string Ingredient)? FindMatch(string allergen, ImmutableArray<ImmutableHashSet<string>> recipes)
            {
                var ingredientCounts = recipes
                    .SelectMany(r => r)
                    .GroupBy(v => v)
                    .ToDictionary(r => r.Key, v => v.Count());

                var ingredients = ingredientCounts.Where(i => i.Value == recipes.Length).ToArray();
                if (ingredients.Length == 1)
                    return (allergen, ingredients[0].Key);

                return null;
            }
        }
    }

    internal static class ParseExtensions
    {
        private static readonly Regex regex = new("(.*) \\(contains (.*)\\)");

        public static ImmutableArray<Food> ParseFoodList(this IInput input)
        {
            var builder = ImmutableArray.CreateBuilder<Food>();
            foreach (var line in input.Lines.AsString())
            {
                var match = regex.Match(line);
                if (match.Success is false)
                {
                    throw new InvalidOperationException($"Line did not match regex '{line}'");
                }

                var ingredients = match.Groups[1].Value.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToImmutableHashSet();
                var allergens = match.Groups[2].Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToImmutableHashSet();

                builder.Add(new Food(ingredients, allergens));
            }

            return builder.ToImmutable();
        }
    }

    record Food(ImmutableHashSet<string> Ingredients, ImmutableHashSet<string> Allergens);
}
