namespace _1brc;

public class Unit(string fileName, FileChunk chunk)
{
    private const byte NewLine = (byte)'\n';
    private const byte Separator = (byte)';';
    private const ushort ChunkSize = 107;

    private readonly DataStructure _data = new();

    public IEnumerable<Location> GetResults() => _data.GetResults();

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
                Span<byte> name = buffer.Slice(0, separator);
                double temp = Helpers.ParseDouble(buffer.Slice(separator + 1, index - separator - 1));
                _data.Add(name, temp);
                
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