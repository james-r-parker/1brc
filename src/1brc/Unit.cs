namespace _1brc;

public class Unit(string fileName, FileChunk chunk)
{
    private const byte NewLine = (byte)'\n';
    private const byte Separator = (byte)';';

    private readonly DataStructure _data = new();

    public IEnumerable<Location> GetResults() => _data.GetResults();

    /// <summary>
    /// Process a single chunk of the file.
    /// </summary>
    public void Run()
    {
        using var reader = Helpers.OpenReader(fileName);
        reader.Position = chunk.Start;
        Span<byte> buffer = new byte[Helpers.FileBufferSize];
        long end = (chunk.Start + chunk.Count) - 1;
        int separator = 0;
        int read = 0;
        int lastNewLine = 0;
        while (reader.Position < end)
        {
            read = reader.Read(buffer);
            for (int i = 0; i < read; i++)
            {
                if (buffer[i] == Separator)
                {
                    unchecked
                    {
                        separator = i;
                    }
                }
                else if (buffer[i] == NewLine)
                {
                    Span<byte> name = buffer.Slice(lastNewLine, separator - lastNewLine);
                    double temp = Helpers.ParseDouble(buffer.Slice(separator + 1, i - separator - 1));
                    _data.Add(name, temp);
                    unchecked
                    {
                        lastNewLine = i + 1;
                    }

                }
            }
            
            if (lastNewLine > 0)
            {
                unchecked
                {
                    reader.Position -= read - lastNewLine;
                    lastNewLine = 0;
                }
            }
        }
    }
}