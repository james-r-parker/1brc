namespace _1brc;

public class Unit
{
    private readonly string _fileName;
    private readonly FileChunk _chunk;
    private const byte NewLine = (byte)'\n';
    private const byte Separator = (byte)';';
    private const ushort ChunkSize = 128;

    private readonly Dictionary<byte[], Location>[] _data;
    
    public Unit(string fileName, FileChunk chunk)
    {
        _fileName = fileName;
        _chunk = chunk;
        _data = new Dictionary<byte[], Location>[255];
        for (int i = 0; i < _data.Length; i++)
        {
            _data[i] = new Dictionary<byte[], Location>(128, new BytesComparer());
        }
    }
    public IEnumerable<KeyValuePair<byte[], Location>> Results => _data.SelectMany(x => x);

    /// <summary>
    /// Process a single chunk of the file.
    /// </summary>
    public void Run()
    {
        using var reader = Helpers.OpenReader(_fileName);
        reader.Position = _chunk.Start;
        Span<byte> buffer = new byte[ChunkSize];
        long end = _chunk.Start + _chunk.Count;
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

                if (_data[name[0]].TryGetValue(name, out var location))
                {
                    location.Update(temp);
                }
                else
                {
                    _data[name[0]].Add(name, new Location(temp));
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