using NodaTime;

namespace Shared
{
    public abstract class Challenge
    {
        public abstract ChallengeInfo Info { get; }
    }

    public record ChallengeInfo(LocalDate Date, string Name);
}
