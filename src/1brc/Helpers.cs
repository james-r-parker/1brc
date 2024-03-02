using System.Runtime.CompilerServices;

namespace _1brc;

public static class Helpers
{
    private const int Minus = 45;
    private const int Dot = 46;
    private const int Zero = 48;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double ParseDouble(ReadOnlySpan<byte> span)
    {
        var temp = 0.0;
        int dec = 0;
        bool isNegative = span[0] == Minus;
        for (int i = (isNegative ? 1 : 0); i < span.Length; i++)
        {
            if (span[i] == Dot) dec = span.Length - i - 1;
            else temp = temp * 10 + (span[i] - Zero);
        }

        return isNegative ? -temp / Math.Pow(10, dec) : temp / Math.Pow(10, dec);
    }
}