using System.Buffers;

namespace _1brc;

public class UnitOfWork(string fileName, FileChunk chunk)
{
    private const int ChunkSize = 1024 * 32;
    private static readonly SearchValues<byte> NewLines = SearchValues.Create([(byte)'\n']);
    private static readonly SearchValues<byte> Separators = SearchValues.Create([(byte)';']);

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
        int lastNewLine = 0;
        while (reader.Position < end)
        {
            var read = reader.Read(buffer);

            while (true)
            {
                Span<byte> window = buffer.Slice(lastNewLine);
                int currentNewLine = window.IndexOfAny(NewLines);
                if (currentNewLine == -1)
                {
                    break;
                }
                
                int separator = window.IndexOfAny(Separators);
                
                _data.Add(
                    window.Slice(0, separator),
                    window.Slice(separator + 1, currentNewLine - separator - 1));
                
                lastNewLine += currentNewLine + 1;
            }
            
            reader.Position -= read - lastNewLine;
            lastNewLine = 0;
            if (reader.Position + ChunkSize > end)
            {
                buffer = new byte[end - reader.Position];
            }
        }
    }
}