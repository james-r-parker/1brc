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
            if (_data[i] is not null)
            {
                foreach (var location in _data[i])
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

        if (_data[index] is null)
        {
            var locationName = new byte[name.Length];
            name.CopyTo(locationName);
            _data[index] = [new(locationName, hashCode, temp)];
            return;
        }

        bool found = false;
        for (var i = 0; i < _data[index].Count; i++)
        {
            if (_data[index][i].HashCode == hashCode && SequenceEqual(name, _data[index][i].Name))
            {
                _data[index][i].Update(temp);
                found = true;
                break;
            }
        }

        if (!found)
        {
            var locationName = new byte[name.Length];
            name.CopyTo(locationName);
            _data[index].Add(new(locationName, hashCode, temp));
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GenerateHashCode(Span<byte> bytes)
    {
        unchecked
        {
            int hash = bytes.Length;
            hash += bytes[0] * (1 << 0);
            hash += bytes[^1] * (1 << bytes.Length - 1);
            return hash;
        }
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
}