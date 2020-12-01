using NodaTime;

namespace Shared
{
    public abstract class Challenge
    {
        public abstract ChallengeInfo Info { get; }

        public abstract void PartOne(IInput input, IOutput output);

        public abstract void PartTwo(IInput input, IOutput output);

        public virtual void TestPartOne(IOutput input)
        {
        }

        public virtual void TestPartTwo(IOutput input)
        {
        }
    }

    public record ChallengeInfo(LocalDate Date, string Name);
}
