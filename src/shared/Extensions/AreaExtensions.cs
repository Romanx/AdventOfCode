using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Grid;

namespace Shared
{
    public static class AreaExtensions
    {
        public static T[,] As2dArray<T>(this Area2d area)
        {
            return new T[area.BottomRight.Y + 1, area.BottomRight.X + 1];
        }
    }
}
