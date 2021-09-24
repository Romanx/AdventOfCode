using System;
using System.Linq;
using NodaTime;
using Shared;

namespace DayFour2016
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2016, 12, 4), "Security Through Obscurity");

        public override void PartOne(IInput input, IOutput output)
        {
            var rooms = input.Parse();

            var sum = rooms
                .Where(r => r.IsValid)
                .Sum(r => r.SectorId);

            output.WriteProperty("Sum of Valid Sector Ids", sum);
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var rooms = input.Parse();

            var targetRoom = rooms
                .Where(r => r.IsValid)
                .First(r => Decrypter.DecryptRoomName(r).Contains("Pole", StringComparison.OrdinalIgnoreCase));

            output.WriteProperty("Target Room Name", Decrypter.DecryptRoomName(targetRoom));
            output.WriteProperty("Target Room Sector Id", targetRoom.SectorId);
        }
    }

    record Room
    {
        public Room(string name, int sectorId, string checksum)
        {
            Name = name;
            SectorId = sectorId;
            Checksum = checksum;
            IsValid = ValidateChecksum(Name, checksum);
        }

        public string Name { get; }

        public int SectorId { get; }

        public string Checksum { get; }

        public bool IsValid { get; }

        private static bool ValidateChecksum(string name, string checksum)
        {
            var caculated = name.Where(c => char.IsLetter(c))
                .GroupBy(c => c)
                .OrderByDescending(c => c.Count())
                .ThenBy(c => c.Key)
                .Select(c => c.Key)
                .Take(5);

            return caculated.SequenceEqual(checksum);
        }
    }
}
