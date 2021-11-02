using System;
using System.Text;
using NodaTime;
using Shared;

namespace DayTwo2016
{
    public class Challenge : ChallengeSync
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2016, 12, 2), "Bathroom Security");

        public override void PartOne(IInput input, IOutput output)
        {
            var builder = new StringBuilder();
            var keypad = new Keypad(new char[,]
            {
                { '1', '2', '3' },
                { '4', '5', '6' },
                { '7', '8', '9' },
            });
            var position = keypad.FiveKey;

            foreach (var line in input.Lines.AsMemory())
            {
                (var key, position) = FindKey(keypad, line.Span, position);

                builder.Append(key);
            }

            output.WriteProperty("Code", builder.ToString());
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var builder = new StringBuilder();
            var keypad = new Keypad(new char[,]
            {
                { ' ', ' ', '1', ' ', ' ' },
                { ' ', '2', '3', '4', ' ' },
                { '5', '6', '7', '8', '9' },
                { ' ', 'A', 'B', 'C', ' ' },
                { ' ', ' ', 'D', ' ', ' ' },
            });
            var position = keypad.FiveKey;

            foreach (var line in input.Lines.AsMemory())
            {
                (var key, position) = FindKey(keypad, line.Span, position);

                builder.Append(key);
            }

            output.WriteProperty("Code", builder.ToString());
        }

        private static (char Key, Point2d position) FindKey(
            Keypad keypad,
            ReadOnlySpan<char> span,
            Point2d position)
        {
            foreach (var c in span)
            {
                var next = c switch
                {
                    'U' => position + GridDirection.Up,
                    'R' => position + GridDirection.Right,
                    'D' => position + GridDirection.Down,
                    'L' => position + GridDirection.Left,
                    _ => throw new InvalidOperationException("Not a valid movement")
                };

                if (keypad.Valid(next))
                {
                    position = next;
                }
            }

            return (keypad.Key(position), position);
        }
    }
}
