using System;
using System.Collections.Generic;

namespace Shared
{
    public interface IInput
    {
        IInputLines Lines { get; }

        IInputContent Content { get; }
    }

    public interface IInputLines
    {
        int Length { get; }

        IEnumerable<ReadOnlyMemory<char>> AsMemory();

        IEnumerable<string> AsString();

        IEnumerable<TOut> Transform<TOut>(Func<string, TOut> transformer);
    }

    public interface IInputContent
    {
        string AsString();

        ReadOnlyMemory<char> AsMemory();

        TOut Transform<TOut>(Func<string, TOut> transformer);
    }
}
