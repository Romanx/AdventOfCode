using System;
using System.Diagnostics.CodeAnalysis;

namespace DayThree2022;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2022, 12, 3), "Rucksack Reorganization");

    public void PartOne(IInput input, IOutput output)
    {
        var rucksacks = input.Lines
            .AsParsable<Rucksack>()
            .ToImmutableArray();

        var totalPriority = rucksacks.Sum(r =>
        {
            var item = FindCommonItem(r.CompartmentA, r.CompartmentB);
            return CalculatePriority(item);
        });

        output.WriteProperty("Total Priority", totalPriority);

        static char FindCommonItem(ReadOnlySpan<char> left, ReadOnlySpan<char> right)
        {
            for (var i = 0; i < left.Length; i++)
            {
                var c = left[i];
                if (right.Contains(c))
                {
                    return c;
                }
            }

            throw new InvalidOperationException();
        }
    }

    public void PartTwo(IInput input, IOutput output)
    {
        var rucksacks = input.Lines
            .AsParsable<Rucksack>()
            .ToImmutableArray();

        var totalPriority = rucksacks
            .Chunk(3)
            .Select(window =>
            {
                var selection = window[0];
                var otherA = window[1];
                var otherB = window[2];

                foreach (var c in selection.Items)
                {
                    if (otherA.Items.Contains(c) && 
                        otherB.Items.Contains(c))
                    {
                        return CalculatePriority(c);
                    }
                }

                throw new InvalidOperationException();
            })
            .Sum();

        output.WriteProperty("Total Priority of Badges", totalPriority);
    }

    private static int CalculatePriority(char @char) => char.IsUpper(@char)
        ? (@char - 38)
        : (@char - 96);
}

public readonly record struct Rucksack : IParsable<Rucksack>
{
    private readonly string items;

    public Rucksack(string items) => this.items = items;

    public ReadOnlySpan<char> Items => items.AsSpan();

    public ReadOnlySpan<char> CompartmentA => items.AsSpan()[..(items.Length / 2)];

    public ReadOnlySpan<char> CompartmentB => items.AsSpan()[(items.Length / 2)..];

    public static Rucksack Parse(string? s, IFormatProvider? provider = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(s);
        return new Rucksack(s);
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Rucksack result)
    {
        try
        {
            result = Parse(s, provider);
            return true;
        }
        catch
        {
            result = default;
            return false;
        }
    }
}
