using System.Runtime.CompilerServices;

namespace _1brc;

public static class Helpers
{
    private const int BufferSize = 1024 * 1024 * 5;
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
            BufferSize = BufferSize,
        });
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double ParseDouble(ReadOnlySpan<byte> bytes)
    {
        var decimalIndex = bytes.IndexOf(DecimalPoint);
        if (bytes[0] == Minus)
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
                _ => throw new ApplicationException("Invalid negative number format")
            };
        }

        return decimalIndex switch
        {
            2
                => (bytes[decimalIndex - 2] - Zero) * 10
                   + (bytes[decimalIndex - 1] - Zero)
                   + (bytes[decimalIndex + 1] - Zero) * 0.1,
            1 => (bytes[decimalIndex - 1] - Zero) + (bytes[decimalIndex + 1] - Zero) * 0.1,
            _ => throw new ApplicationException("Invalid negative number format")
        };
    }
}