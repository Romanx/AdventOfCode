using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using NodaTime;
using Shared;

namespace DayTwo2020
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2020, 12, 02), "Password Philosophy");

        public override void PartOne(IInput input, IOutput output)
        {
            var records = input.AsRecords();
            var validPasswords = records.Where(r => r.IsValidBasedOnCount()).Count();

            output.WriteProperty("Total Passwords", records.Length);
            output.WriteProperty("Number of Valid Passwords", validPasswords);
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var records = input.AsRecords();
            var validPasswords = records.Where(r => r.IsValidBasedOnPosition()).Count();

            output.WriteProperty("Total Passwords", records.Length);
            output.WriteProperty("Number of Valid Passwords", validPasswords);
        }
    }

    internal record Policy(char Letter, int LowerBound, int UpperBound);

    internal record PasswordRecord(Policy Policy, string Password)
    {
        public bool IsValidBasedOnCount()
        {
            var chars = Password
                .GroupBy(c => c)
                .ToDictionary(k => k.Key, v => v.Count());

            if (chars.TryGetValue(Policy.Letter, out var count) is false)
            {
                return false;
            }

            if (count < Policy.LowerBound || count > Policy.UpperBound)
            {
                return false;
            }

            return true;
        }

        public bool IsValidBasedOnPosition()
        {
            var lowerBoundValid = Password[Policy.LowerBound - 1] == Policy.Letter;
            var upperBoundValid = Password[Policy.UpperBound - 1] == Policy.Letter;

            return (lowerBoundValid && !upperBoundValid) ||
                   (!lowerBoundValid && upperBoundValid);
        }
    }

    internal static class ParsingExtensions
    {
        internal static readonly Regex PasswordRegex = new Regex("(?<LowerBound>[0-9]+)-(?<UpperBound>[0-9]+) (?<Letter>[a-z]): (?<Password>[a-z]*)");

        public static ImmutableArray<PasswordRecord> AsRecords(this IInput input)
        {
            var builder = ImmutableArray.CreateBuilder<PasswordRecord>();

            foreach (var line in input.Lines.AsMemory())
            {
                var matches = PasswordRegex.Match(line.ToString());

                if (!matches.Success) Debugger.Break();

                var policy = new Policy(
                    Letter: matches.Groups["Letter"].Value[0],
                    LowerBound: int.Parse(matches.Groups["LowerBound"].Value),
                    UpperBound: int.Parse(matches.Groups["UpperBound"].Value));

                var password = matches.Groups["Password"].Value;

                builder.Add(new PasswordRecord(policy, password));
            }

            return builder.ToImmutable();
        }
    }
}
