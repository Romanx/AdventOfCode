using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;
using NodaTime;
using Shared;

namespace DayFour2020
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2020, 12, 04), "Passport Processing");

        public override void PartOne(IInput input, IOutput output)
        {
            var passports = input.ParsePassports();

            var validPassports = passports.Count(p => ValidatePassport(p));

            output.WriteProperty("Number of Passports", passports.Length);
            output.WriteProperty("Number of Valid Passports", validPassports);

            static bool ValidatePassport(Passport passport)
            {
                var fields = passport.Fields.Keys;
                var missingFields = Passport.ValidFields.Except(fields);

                if (missingFields.Count == 0)
                {
                    return true;
                }

                return false;
            }
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var passports = input.ParsePassports();

            var validPassports = passports.Count(p => ValidatePassport(p));

            output.WriteProperty("Number of Passports", passports.Length);
            output.WriteProperty("Number of Valid Passports", validPassports);

            static bool ValidatePassport(Passport passport)
            {
                var fields = passport.Fields.Keys;
                var missingFields = Passport.ValidFields.Except(fields);

                if (missingFields.Count > 0)
                {
                    return false;
                }

                foreach (var (key, value) in passport.Fields)
                {
                    var result = key switch
                    {
                        "byr" => IsIntInRange(value, 1920, 2002),
                        "iyr" => IsIntInRange(value, 2010, 2020),
                        "eyr" => IsIntInRange(value, 2020, 2030),
                        "hgt" => HeightValidate(value),
                        "hcl" => Regex.IsMatch(value, "^#[0-9a-f]{6}$"),
                        "ecl" => ValidateEyeColor(value),
                        "pid" => Regex.IsMatch(value, "^\\d{9}$"),
                        "cid" => true,
                        _ => throw new InvalidOperationException("Field not known")
                    };

                    if (result is false)
                    {
                        return false;
                    }
                }

                return true;
            }

            static bool IsIntInRange(string value, int lowerBound, int upperBound)
            {
                return int.TryParse(value, out var res) && (res >= lowerBound && res <= upperBound);
            }

            static bool HeightValidate(string value)
            {
                var match = Regex.Match(value, "(\\d+)(cm|in)");

                if (match.Success)
                {
                    return match.Groups[2].Value switch
                    {
                        "cm" => IsIntInRange(match.Groups[1].Value, 150, 193),
                        "in" => IsIntInRange(match.Groups[1].Value, 59, 76),
                        _ => throw new InvalidOperationException("Height not known")
                    };
                }

                return false;
            }

            static bool ValidateEyeColor(string value) => eyeColours.Contains(value);
        }

        private static readonly ImmutableHashSet<string> eyeColours = ImmutableHashSet.Create("amb", "blu", "brn", "gry", "grn", "hzl", "oth");
    }

    internal static class ParsingExtensions
    {
        private static readonly Regex keyValuePairRegex = new Regex("([a-z]{3}):(\\S+)");

        public static ImmutableArray<Passport> ParsePassports(this IInput input)
        {
            var builder = ImmutableArray.CreateBuilder<Passport>();
            var fieldBuilder = ImmutableDictionary.CreateBuilder<string, string>();

            foreach (var line in input.AsLines())
            {
                if (line.IsEmpty)
                {
                    builder.Add(new Passport(fieldBuilder.ToImmutable()));
                    fieldBuilder.Clear();
                }
                else
                {
                    var matches = keyValuePairRegex.Matches(line.ToString());
                    foreach (Match match in matches)
                    {
                        fieldBuilder.Add(match.Groups[1].Value, match.Groups[2].Value);
                    }
                }
            }

            if (fieldBuilder.Count > 0)
            {
                builder.Add(new Passport(fieldBuilder.ToImmutable()));
            }

            return builder.ToImmutable();
        }
    }

    record Passport(ImmutableDictionary<string, string> Fields)
    {
        public static ImmutableHashSet<string> AllFields { get; } = ImmutableHashSet.Create("byr", "iyr", "eyr", "hgt", "hcl", "ecl", "pid", "cid");
        public static ImmutableHashSet<string> ValidFields { get; } = AllFields.Except(new[] { "cid" });
    }
}
