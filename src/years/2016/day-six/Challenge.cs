using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.Collections.Extensions;
using NodaTime;
using Shared;

namespace DaySix2016
{
    public class Challenge : ChallengeSync
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2016, 12, 6), "Signals and Noise");

        public override void PartOne(IInput input, IOutput output)
        {
            var recordedMessages = input.Lines.AsString().ToImmutableArray();
            var message = DecodeMessage(recordedMessages, collection =>
            {
                return collection.OrderByDescending(kvp => kvp.Value).First().Key;
            });

            output.WriteProperty("Decoded message", message);
        }

        public override void PartTwo(IInput input, IOutput output)
        {
            var recordedMessages = input.Lines.AsString().ToImmutableArray();
            var message = DecodeMessage(recordedMessages, collection =>
            {
                return collection.OrderBy(kvp => kvp.Value).First().Key;
            });

            output.WriteProperty("Decoded message", message);
        }

        private static string DecodeMessage(
            ImmutableArray<string> recordedMessages,
            Func<IEnumerable<KeyValuePair<char, int>>, char> selectorFunc)
        {
            var length = recordedMessages[0].Length;

            var state = (recordedMessages, selectorFunc);

            return string.Create(length, state, static (span, state) =>
            {
                var (recordedMessages, selectorFunc) = state;

                for (var i = 0; i < span.Length; i++)
                {
                    var dictionary = new DictionarySlim<char, int>();

                    foreach (var line in recordedMessages)
                    {
                        ref var count = ref dictionary.GetOrAddValueRef(line[i]);
                        count++;
                    }

                    span[i] = selectorFunc(dictionary);
                }
            });
        }
    }
}
