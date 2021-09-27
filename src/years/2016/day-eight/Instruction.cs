using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using Shared;

namespace DayEight2016
{
    abstract record Instruction(CommandType CommandType)
    {
        public abstract Display Apply(Display display);
    }

    enum CommandType
    {
        Rect,
        RotateColumn,
        RotateRow
    }

    enum LightState
    {
        [Display(Name = ".")]
        Off,
        [Display(Name = "#")]
        On
    }
}
