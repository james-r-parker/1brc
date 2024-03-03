namespace _1brc;

public class Unit(string fileName, FileChunk chunk)
{
    private const byte NewLine = (byte)'\n';
    private const byte Separator = (byte)';';
    private const ushort ChunkSize = 128;

    private readonly Dictionary<byte[], Location> _data = new(512, new BytesComparer());
    
    public IReadOnlyDictionary<byte[], Location> Results => _data;

    /// <summary>
    /// Process a single chunk of the file.
    /// </summary>
    public void Run()
    {
        using var reader = Helpers.OpenReader(fileName);
        reader.Position = chunk.Start;
        Span<byte> buffer = new byte[ChunkSize];
        long end = chunk.Start + chunk.Count;
        ushort index = 0;
        ushort separator = 0;
        while (reader.Position < end)
        {
            buffer[index] = (byte)reader.ReadByte();
            if (buffer[index] == Separator)
            {
                unchecked
                {
                    separator = index;
                }
            }
            else if (buffer[index] == NewLine)
            {
                var name = new byte[separator];
                buffer.Slice(0, separator).CopyTo(name);

                var temp = Helpers.ParseDouble(buffer.Slice(separator + 1, index - separator - 1));

                if (_data.TryGetValue(name, out var location))
                {
                    location.Update(temp);
                }
                else
                {
                    _data.Add(name, new Location(temp));
                }

                unchecked
                {
                    index = 0;
                }

                continue;
            }

            unchecked
            {
                index++;
            }
        }
    }
}