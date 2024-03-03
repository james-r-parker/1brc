namespace _1brc.tests;

public class DoubleComparer : IEqualityComparer<double>
{
    public bool Equals(double x, double y)
    {
        return Math.Abs(x - y) < 0.1;
    }

    public int GetHashCode(double obj)
    {
        return obj.GetHashCode();
    }
}