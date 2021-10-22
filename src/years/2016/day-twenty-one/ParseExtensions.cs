using System;
using System.Collections.Immutable;
using MoreLinq;
using PCRE;
using Shared;

namespace DayTwentyOne2016
{
    internal static class ParseExtensions
    {
        private static readonly PcreRegex swapPositionRegex = new("swap position (?<X>[0-9]+) with position (?<Y>[0-9]+)");
        private static readonly PcreRegex swapLetterRegex = new("swap letter (?<X>[a-z]) with letter (?<Y>[a-z])");
        private static readonly PcreRegex rotateDirectionRegex = new("rotate (?<Direction>left|right) (?<X>[0-9])+ step(s)?");
        private static readonly PcreRegex rotatePositionRegex = new("rotate based on position of letter (?<X>[a-z])+");
        private static readonly PcreRegex reversePositionsRegex = new("reverse positions (?<X>[0-9]) through (?<Y>[0-9])");
        private static readonly PcreRegex movePositionsRegex = new("move position (?<X>[0-9]) to position (?<Y>[0-9])");

        public static Scrambler Parse(this IInput input)
        {
            var builder = ImmutableArray.CreateBuilder<ScrambleFunction>();

            foreach (var line in input.Lines.AsMemory())
            {
                builder.Add(BuildFunction(line.Span));
            }

            return new Scrambler(builder.ToImmutable());
        }

        private static ScrambleFunction BuildFunction(ReadOnlySpan<char> line)
        {
            var rotateDirectionMatch = rotateDirectionRegex.Match(line);
            var rotatePositionMatch = rotatePositionRegex.Match(line);

            if (line.StartsWith("swap position"))
            {
                var match = swapPositionRegex.Match(line);

                return new SwapPosition(
                    int.Parse(match.Groups["X"].Value),
                    int.Parse(match.Groups["Y"].Value));
            }
            else if (line.StartsWith("swap letter"))
            {
                var match = swapLetterRegex.Match(line);

                return new SwapLetter(
                    match.Groups["X"].Value[0],
                    match.Groups["Y"].Value[0]);
            }
            else if (rotateDirectionMatch.Success)
            {
                var direction = Enum.Parse<Direction>(rotateDirectionMatch.Groups["Direction"].Value.ToString(), true);

                return new RotateDirection(
                    direction,
                    int.Parse(rotateDirectionMatch.Groups["X"].Value));
            }
            else if (rotatePositionMatch.Success)
            {
                return new RotatePosition(
                    rotatePositionMatch["X"].Value[0]);
            }
            else if (line.StartsWith("reverse"))
            {
                var match = reversePositionsRegex.Match(line);

                return new ReversePosition(
                    int.Parse(match["X"].Value),
                    int.Parse(match["Y"].Value));
            }
            else if (line.StartsWith("move"))
            {
                var match = movePositionsRegex.Match(line);

                return new MovePosition(
                    int.Parse(match["X"].Value),
                    int.Parse(match["Y"].Value));
            }

            throw new InvalidOperationException("Function not Found");
        }
    }
}
