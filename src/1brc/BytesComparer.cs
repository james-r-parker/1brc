namespace _1brc;

public class BytesComparer : IEqualityComparer<byte[]>
{
    public bool Equals(byte[]? x, byte[]? y)
    {
        if (x.Length != y.Length) return false;
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
            int hash = 0;
            for (int i = 0; i < obj.Length; i++)
            {
                hash += obj[i] * (1 << i);
            }
            return hash;
        }
    }
}