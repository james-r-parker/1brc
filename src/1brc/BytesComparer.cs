namespace _1brc;

public class BytesComparer : IEqualityComparer<ReadOnlyMemory<byte>>
{
    public bool Equals(ReadOnlyMemory<byte> x, ReadOnlyMemory<byte> y)
    {
        return x.Span.SequenceEqual(y.Span);
    }

    public int GetHashCode(ReadOnlyMemory<byte> bytes)
    {
        unchecked
        {
            uint hash = 2166136261U;
            hash = ((hash * 19) ^ bytes.Span[0]);
            hash = ((hash * 19) ^ bytes.Span[^2]);
            hash = ((hash * 19) ^ bytes.Span[^1]);
            return (int)hash;
        }
    }
}