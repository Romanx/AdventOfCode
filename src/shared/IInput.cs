using System;
using System.Collections.Generic;

namespace Shared
{
    public interface IInput
    {
        char[,] As2DArray();

        ReadOnlyMemory<char> AsReadOnlyMemory();

        IEnumerable<ReadOnlyMemory<char>> AsLines();
    }
}
