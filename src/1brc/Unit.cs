namespace _1brc;

public class Unit(string fileName, FileChunk chunk)
{
    private const byte NewLine = (byte)'\n';
    private const byte Separator = (byte)';';
    private const int ChunkSize = 1024 * 128;

    private readonly DataStructure _data = new();

    public IReadOnlyCollection<Location> GetResults() => _data.GetResults();

    /// <summary>
    /// Process a single chunk of the file.
    /// </summary>
    public void Run()
    {
        using var reader = Helpers.OpenReader(fileName);
        reader.Position = chunk.Start;
        Span<byte> buffer = new byte[ChunkSize];
        long end = chunk.Start + chunk.Count;
        int separator = 0;
        int read;
        int lastNewLine = 0;
        double temp;
        while (reader.Position < end)
        {
            read = reader.Read(buffer);
            for (var i = 0; i < read; i++)
            {
                switch (buffer[i])
                {
                    case Separator:
                        separator = i;
                        break;
                    case NewLine:
                        Span<byte> name = buffer.Slice(lastNewLine, separator - lastNewLine);
                        temp = Helpers.ParseDouble(buffer.Slice(separator + 1, i - separator - 1));
                        _data.Add(name, temp);
                        lastNewLine = i + 1;
                        break;
                }
            }

            reader.Position -= read - lastNewLine;
            separator = 0;
            lastNewLine = 0;

            if (reader.Position + ChunkSize > end)
            {
                buffer = new byte[end - reader.Position];
            }
        }
    }
}