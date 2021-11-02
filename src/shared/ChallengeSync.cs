using System.Threading.Tasks;
using NodaTime;

namespace Shared
{
    public abstract class ChallengeSync : Challenge
    {
        public abstract void PartOne(IInput input, IOutput output);

        public abstract void PartTwo(IInput input, IOutput output);
    }

    public abstract class ChallengeAsync : Challenge
    {
        public abstract Task PartOne(IInput input, IOutput output);

        public abstract Task PartTwo(IInput input, IOutput output);
    }

    public abstract class Challenge
    {
        public abstract ChallengeInfo Info { get; }
    }

    public record ChallengeInfo(LocalDate Date, string Name);
}
