using System;
using System.Collections.Generic;
using System.IO;

namespace Shared
{
    public sealed class Input : IInput
    {
        public Input(string content)
        {
            Content = new InputContent(content);
            Lines = new InputLines(content);
        }

        public IInputContent Content { get; }

        public IInputLines Lines { get; }
    }

    public sealed class InputContent : IInputContent
    {
        private readonly string _content;

        public InputContent(string content)
        {
            _content = content.Trim();
        }

        public ReadOnlyMemory<char> AsMemory() => _content.AsMemory();

        public string AsString() => _content;

        public TOut Transform<TOut>(Func<string, TOut> transformer)
            => transformer(_content);
    }

    public sealed class InputLines : IInputLines
    {
        private readonly string _content;

        public InputLines(string content)
        {
            _content = content;
        }

        public IEnumerable<ReadOnlyMemory<char>> AsMemory()
            => Transform(static str => str.AsMemory());

        public IEnumerable<string> AsString()
        {
            using var reader = new StringReader(_content);

            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                yield return line;
            }
        }

        public IEnumerable<TOut> Transform<TOut>(Func<string, TOut> transformer)
        {
            foreach (var line in AsString())
            {
                yield return transformer(line);
            }
        }
    }
}
