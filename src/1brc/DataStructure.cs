using System.Runtime.CompilerServices;

namespace _1brc;

public class DataStructure
{
    private const int Capacity = 10007;
    private readonly List<Location>?[] _data = new List<Location>?[Capacity];

    public IEnumerable<Location> GetResults()
    {
        for (var i = 0; i < Capacity; i++)
        {
            var item = _data[i];
            if (item is not null)
            {
                foreach (var location in item)
                {
                    yield return location;
                }
            }
        }
    }

    public void Add(Span<byte> name, double temp)
    {
        int hashCode = GenerateHashCode(name);
        int index = Math.Abs(hashCode % Capacity);
        var item = _data[index];

        if (item is null)
        {
            var locationName = new byte[name.Length];
            name.CopyTo(locationName);
            _data[index] = [new(locationName, hashCode, temp)];
            return;
        }

        bool found = false;
        for (var i = 0; i < item.Count; i++)
        {
            var location = item[i];
            if (Equals(location, hashCode, name))
            {
                location.Update(temp);
                found = true;
                break;
            }
        }

        if (!found)
        {
            var locationName = new byte[name.Length];
            name.CopyTo(locationName);
            item.Add(new(locationName, hashCode, temp));
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(Location location, int hashCode, Span<byte> bytes)
    {
        return location.HashCode == hashCode && SequenceEqual(bytes, location.Name);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool SequenceEqual(Span<byte> span, byte[] other)
    {
        if (span.Length != other.Length)
        {
            return false;
        }

        for (var i = 0; i < span.Length; i++)
        {
            if (span[i] != other[i])
            {
                return false;
            }
        }

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int GenerateHashCode(Span<byte> bytes)
    {
        unchecked
        {
            int hash = bytes.Length;
            hash += bytes[0] * (1 << 0);
            hash += bytes[^1] * (1 << bytes.Length - 1);
            return hash;
        }
    }
}