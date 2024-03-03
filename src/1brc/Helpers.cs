using System.Runtime.CompilerServices;

namespace _1brc;

public static class Helpers
{
    private const byte DecimalPoint = (byte)'.';
    private const int Minus = (byte)'-';
    private const int Zero = (byte)'0';
    
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double ParseDouble(ReadOnlySpan<byte> bytes)
    {
        var decimalIndex = bytes.IndexOf(DecimalPoint);
        var isNegative = bytes[0] == Minus;

        if (isNegative)
        {
            return decimalIndex switch
            {
                3
                    => -1
                       * (
                           (bytes[decimalIndex - 2] - Zero) * 10
                           + (bytes[decimalIndex - 1] - Zero)
                           + (bytes[decimalIndex + 1] - Zero) * 0.1
                       ),
                2
                    => -1
                       * (
                           (bytes[decimalIndex - 1] - Zero)
                           + (bytes[decimalIndex + 1] - Zero) * 0.1
                       ),
                _ => double.Parse(bytes)
            };
        }

        return decimalIndex switch
        {
            2
                => (bytes[decimalIndex - 2] - Zero) * 10
                   + (bytes[decimalIndex - 1] - Zero)
                   + (bytes[decimalIndex + 1] - Zero) * 0.1,
            1 => (bytes[decimalIndex - 1] - Zero) + (bytes[decimalIndex + 1] - Zero) * 0.1,
            _ => double.Parse(bytes)
        };
    }
}