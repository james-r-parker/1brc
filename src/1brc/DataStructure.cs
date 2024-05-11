using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace _1brc;

public class DataStructure
{
    private const int Capacity = 9973;
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
        int index = Math.Abs(GenerateHashCode(name) % Capacity);
        var item = _data[index];
        var temperature = Helpers.ParseDouble(temp);

        if (item is null)
        {
            var newLocationName = new byte[name.Length];
            name.CopyTo(newLocationName);
            _data[index] = new List<Location>(3)
            {
                new(newLocationName, temperature)
            };
            return;
        }

        var listAsSpan = CollectionsMarshal.AsSpan(item);
        for (var i = 0; i < listAsSpan.Length; i++)
        {
            ref var location = ref listAsSpan[i];
            if (name.SequenceEqual(location.Name.Span))
            {
                location.Update(temperature);
                return;
            }
        }

        var locationName = new byte[name.Length];
        name.CopyTo(locationName);
        item.Add(new(locationName, temperature));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int GenerateHashCode(ReadOnlySpan<byte> bytes)
    {
        unchecked
        {
            int hash = bytes.Length;
            hash = ((hash * 19) ^ bytes[0]);
            hash = ((hash * 19) ^ bytes[^2]);
            return hash;
        }
    }
}