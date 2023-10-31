using MoreLinq;

namespace DayTwenty2022;

public class Challenge : Shared.Challenge
{
    public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2022, 12, 20), "Grove Positioning System");

    public void PartOne(IInput input, IOutput output)
    {
        var numbers = input
            .Lines
            .As<int>()
            .Index()
            .Select(kvp => new MappedNumber(kvp.Key, kvp.Value))
            .ToList();

        Decrypt(numbers);

        var coordinates = GroveCoordinates(numbers);
        output.WriteProperty("Grove Coordinates", coordinates);
    }

    public void PartTwo(IInput input, IOutput output)
    {
        const int DecryptionKey = 811589153;

        var numbers = input
            .Lines
            .As<long>()
            .Index()
            .Select(kvp => new MappedNumber(kvp.Key, kvp.Value * DecryptionKey))
            .ToList();

        for (var i = 0; i < 10; i++)
        {
            Decrypt(numbers);
        }

        var coordinates = GroveCoordinates(numbers);
        output.WriteProperty("Grove Coordinates", coordinates);
    }

    void Decrypt(List<MappedNumber> numbers)
    {
        for (var i = 0; i < numbers.Count; i++)
        {
            var currentIndex = numbers.FindIndex(mn => mn.Index == i);
            var item = numbers[currentIndex];
            numbers.RemoveAt(currentIndex);

            var mod = (currentIndex + item.Number) % numbers.Count;
            var target = mod >= 0
                ? mod
                : mod + numbers.Count;

            numbers.Insert((int)target, item);
        }
    }

    static long GroveCoordinates(List<MappedNumber> numbers)
    {
        var zero = numbers.FindIndex(item => item.Number is 0);

        return numbers[(zero + 1000) % numbers.Count].Number +
            numbers[(zero + 2000) % numbers.Count].Number +
            numbers[(zero + 3000) % numbers.Count].Number;
    }
}

readonly record struct MappedNumber(int Index, long Number);
