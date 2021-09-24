namespace DayThree2016
{
    record Triangle
    {
        public Triangle(int a, int b, int c)
        {
            A = a;
            B = b;
            C = c;
            Valid = a + b > c &&
                a + c > b &&
                c + b > a;
        }

        public bool Valid { get; }
        public int A { get; }
        public int B { get; }
        public int C { get; }

        public override string ToString() => $"{A} x {B} x {C}";
    }
}
