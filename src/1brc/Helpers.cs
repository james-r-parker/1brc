using System.Runtime.CompilerServices;

namespace _1brc;

public static class Helpers
{
    private const byte DecimalPoint = (byte)'.';
    private const int Minus = (byte)'-';
    private const int Zero = (byte)'0';

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static FileStream OpenReader(string fileName, FileOptions options = FileOptions.SequentialScan)
    {
        return File.Open(fileName, new FileStreamOptions
        {
            Options = options,
            Access = FileAccess.Read,
            Mode = FileMode.Open,
            Share = FileShare.Read,
        });
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double ParseDouble(ReadOnlySpan<byte> bytes)
    {
        var decimalIndex = bytes.IndexOf(DecimalPoint);
        if (bytes[0] == Minus)
        {
            if (decimalIndex == 3)
                return -1 * (
                    (bytes[decimalIndex - 2] - Zero) * 10
                    + (bytes[decimalIndex - 1] - Zero)
                    + (bytes[decimalIndex + 1] - Zero) * 0.1
                );
            return -1 * (
                (bytes[decimalIndex - 1] - Zero)
                + (bytes[decimalIndex + 1] - Zero) * 0.1
            );
        }

        if (decimalIndex == 2)
            return (bytes[decimalIndex - 2] - Zero) * 10
                   + (bytes[decimalIndex - 1] - Zero)
                   + (bytes[decimalIndex + 1] - Zero) * 0.1;
        return (bytes[decimalIndex - 1] - Zero) + (bytes[decimalIndex + 1] - Zero) * 0.1;
    }
}