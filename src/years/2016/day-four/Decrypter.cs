using static Shared.AlphabetHelper;

namespace DayFour2016
{
    static class Decrypter
    {
        public static string DecryptRoomName(Room room)
        {
            return string.Create(room.Name.Length, room, static (span, state) =>
            {
                for (var i = 0; i < state.Name.Length; i++)
                {
                    var c = state.Name[i];
                    if (c == '-')
                    {
                        span[i] = ' ';
                    }
                    else
                    {
                        var startIndex = Lowercase.IndexOf(c);
                        var targetIndex = (startIndex + state.SectorId) % Lowercase.Length;

                        span[i] = Lowercase[targetIndex];
                    }
                }
            });
        }
    }
}
