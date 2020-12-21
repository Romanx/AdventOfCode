using System;
using System.Collections.Generic;

namespace Shared
{
    public interface IInput
    {
        char[,] As2DArray();

        ReadOnlyMemory<char> AsReadOnlyMemory();

        string AsString();

        IEnumerable<ReadOnlyMemory<char>> AsLines();

        IEnumerable<string> AsStringLines();
    }
}
