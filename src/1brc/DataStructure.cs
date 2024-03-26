using System.Runtime.CompilerServices;

namespace _1brc;

public class DataStructure
{
    private const int Capacity = 20011;
    private readonly List<Location>?[] _data = new List<Location>?[Capacity];

    public IReadOnlyCollection<Location> GetResults()
    {
        var list = new List<Location>(capacity: Capacity / 2);
        for (var i = 0; i < Capacity; i++)
        {
            var item = _data[i];
            if (item is not null)
            {
                list.AddRange(item);
            }
        }
        return list;
    }

    public void Add(ReadOnlySpan<byte> name, ReadOnlySpan<byte> temp)
    {
        int hashCode = GenerateHashCode(name);
        int index = Math.Abs(hashCode % Capacity);
        var item = _data[index];
        var temperature = Helpers.ParseDouble(temp);

        if (item is null)
        {
            var newLocationName = new byte[name.Length];
            name.CopyTo(newLocationName);
            _data[index] = new List<Location>(5)
            {
                new(newLocationName, hashCode, temperature)
            };
            return;
        }

        for (var i = 0; i < item.Count; i++)
        {
            var location = item[i];
            if (Equals(hashCode, name, location.HashCode, location.Name.Span))
            {
                location.Update(temperature);
                return;
            }
        }

        var locationName = new byte[name.Length];
        name.CopyTo(locationName);
        item.Add(new(locationName, hashCode, temperature));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool Equals(
        int newHashCode,
        ReadOnlySpan<byte> newName,
        int oldHashCode,
        ReadOnlySpan<byte> oldName)
    {
        return newHashCode == oldHashCode && newName.SequenceEqual(oldName);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int GenerateHashCode(ReadOnlySpan<byte> bytes)
    {
        unchecked
        {
            uint hash = 2166136261U;
            hash = ((hash * 19) ^ bytes[0]);
            hash = ((hash * 19) ^ bytes[^2]);
            hash = ((hash * 19) ^ bytes[^1]);
            return (int)hash;
        }
    }
}