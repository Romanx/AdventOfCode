using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
        private string[]? _lines;

        public InputLines(string content)
        {
            _content = content;
        }

        public int Length
        {
            get
            {
                EnsureReadLines();
                return _lines!.Length;
            }
        }

        public IEnumerable<ReadOnlyMemory<char>> AsMemory()
            => Transform(static str => str.AsMemory());

        public IEnumerable<string> AsString()
        {
            EnsureReadLines();
            return _lines!;
        }

        public IEnumerable<TOut> Transform<TOut>(Func<string, TOut> transformer)
        {
            foreach (var line in AsString())
            {
                yield return transformer(line);
            }
        }

        private void EnsureReadLines()
        {
            if (_lines is not null)
            {
                return;
            }

            using var reader = new StringReader(_content);

            _lines = reader.ReadAllLines().ToArray();
        }
    }
}
