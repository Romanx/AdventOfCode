using System;
using System.Collections.Specialized;

namespace Shared
{
    public static class BitVector32Extensions
    {
        public static string ToBinaryString(BitVector32 vector)
        {
            return vector.Data.ToString("b32");
        }
    }
}
