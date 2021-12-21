namespace DayNineteen2021;

record Transform(int Scanner, Point3d Origin, int Configuration)
{
    public IEnumerable<Point3d> Apply(IEnumerable<Point3d> points)
        => points
            .Select(p => p.ApplyFacingAndRotationChange(Configuration) + Origin);
}
