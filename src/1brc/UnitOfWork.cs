using System.Buffers;
using System.Diagnostics;

namespace _1brc;

public class UnitOfWork(string fileName, FileChunk chunk)
{
    private const int ChunkSize = 1024 * 512;
    private const byte NewLine = (byte)'\n';
    private const byte Separator = (byte)';';

    private readonly DataStructure _data = new();

    public IReadOnlyCollection<Location> GetResults() => _data.GetResults();

    /// <summary>
    /// Process a single chunk of the file.
    /// </summary>
    public void Run()
    {
        using var reader = Helpers.OpenReader(fileName);
        reader.Seek(chunk.Start, SeekOrigin.Begin);
        byte[] buffer = new byte[ChunkSize];
        long end = chunk.Start + chunk.Count;
        int lastNewLine = 0;
        while (reader.Position < end)
        {
            var read = reader.Read(buffer, 0, Math.Min(ChunkSize, (int)(end - reader.Position)));
            Span<byte> range = buffer.AsSpan(0, read);
            Span<byte> window = range;
            
            while (true)
            {
                int currentNewLine = window.IndexOf(NewLine);
                if (currentNewLine == -1)
                {
                    break;
                }
                
                int separator = window.IndexOf(Separator);
                
                _data.Add(
                    window[..separator],
                    window.Slice(separator + 1, currentNewLine - separator - 1));
                
                lastNewLine += currentNewLine + 1;
                window = range[lastNewLine..];
            }
            
            reader.Seek(lastNewLine - read, SeekOrigin.Current);
            lastNewLine = 0;
        }
    }
}