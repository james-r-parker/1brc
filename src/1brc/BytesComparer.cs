namespace _1brc;

public class BytesComparer : IEqualityComparer<byte[]>
{
    public bool Equals(byte[]? x, byte[]? y)
    {
        for (int i = 0; i < x.Length; i++)
        {
            if (x[i] != y[i]) return false;
        }

        return true;
    }

    public int GetHashCode(byte[] obj)
    {
        unchecked
        {
            int hash = obj.Length;
            hash += obj[2] * (1 << 2);
            hash += obj[obj.Length - 1] * (1 << obj.Length - 1);
            return hash;
        }
    }
}