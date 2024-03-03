using System.Text;
using System.Text.Json.Nodes;

namespace _1brc;

public class Parser(string fileName)
{
    private const int BufferSize = 8192;
    private const ushort ChunkSize = 128;

    private const byte NewLine = (byte)'\n';
    private const byte Separator = (byte)';';
    private const byte Dot = (byte)'.';
    private const byte A = (byte)'A';
    private const byte Z = (byte)'Z';
    private const byte a = (byte)'a';
    private const byte z = (byte)'z';

    public async Task<IEnumerable<Output>> Run()
    {
        var chunks = GetChunks();
        var tasks = new Task<Dictionary<byte[], Location>>[chunks.Length];
        for (var i = 0; i < chunks.Length; i++)
        {
            var chunk = chunks[i];
            tasks[i] = Task.Run(() => ProcessChunk(chunk));
        }

        await Task.WhenAll(tasks);

        return tasks.SelectMany(x => x.Result)
            .GroupBy(x => Encoding.UTF8.GetString(x.Key))
            .Select(x =>
            {
                return new Output()
                {
                    Name = x.Key,
                    Count = x.Sum(y => y.Value.Count),
                    Min = x.Min(y => y.Value.Min),
                    Max = x.Max(y => y.Value.Max),
                    Sum = x.Sum(y => y.Value.Sum),
                };
            })
            .OrderBy(x => x.Name)
            .ToList();
    }

    /// <summary>
    /// Breaks the file down into chunks for parallel processing.
    /// </summary>
    private FileChunk[] GetChunks()
    {
        var chunks = new FileChunk[Environment.ProcessorCount];
        using var reader = OpenReader(FileOptions.RandomAccess);
        long chunkSize = reader.Length / chunks.Length;
        byte[] buffer = new byte[1];
        long start = 0L;
        ushort chunk = 0;
        reader.Position = chunkSize;
        while (reader.Read(buffer) > 0)
        {
            if (buffer[0] == Dot)
            {
                chunks[chunk] = new FileChunk(start, (reader.Position + 2) - start);
                start = reader.Position + 2;
                chunk++;
                if (reader.Position + chunkSize > reader.Length)
                {
                    chunks[chunk] = new FileChunk(start, reader.Length - start);
                    break;
                }

                reader.Position += chunkSize;
            }
            else if (buffer[0] == NewLine)
            {
                chunks[chunk] = new FileChunk(start, reader.Position - start);
                start = reader.Position;
                chunk++;
                if (reader.Position + chunkSize > reader.Length)
                {
                    chunks[chunk] = new FileChunk(start, reader.Length - start);
                    break;
                }

                reader.Position += chunkSize;
            }
            else
                switch (buffer[0])
                {
                    case Separator:
                        reader.Position += 1;
                        break;
                    case >= A and <= Z:
                        reader.Position += 3;
                        break;
                    case >= a and <= z:
                        reader.Position += 2;
                        break;
                }
        }

        return chunks;
    }

    /// <summary>
    /// Process a single chunk of the file.
    /// </summary>
    private Dictionary<byte[], Location> ProcessChunk(FileChunk chunk)
    {
        Dictionary<byte[], Location> data = new(512, new BytesComparer());

        using var reader = OpenReader();
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
                
                if (data.TryGetValue(name, out var location))
                {
                    location.Update(temp);
                }
                else
                {
                    data.Add(name, new Location(temp));
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

        return data;
    }

    private FileStream OpenReader(FileOptions options = FileOptions.SequentialScan)
    {
        return File.Open(fileName, new FileStreamOptions
        {
            Options = options,
            Access = FileAccess.Read,
            Mode = FileMode.Open,
            Share = FileShare.Read,
            BufferSize = BufferSize,
        });
    }
}