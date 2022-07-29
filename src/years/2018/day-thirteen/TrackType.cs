using System.ComponentModel.DataAnnotations;

namespace DayThirteen2018;

enum TrackType
{
    [Display(Name = ".")]
    None,
    [Display(Name = "|")]
    UpDown,
    [Display(Name = "-")]
    LeftRight,
    [Display(Name = "/")]
    CurveRight,
    [Display(Name = "\\")]
    CurveLeft,
    [Display(Name = "+")]
    Intersection,
}
