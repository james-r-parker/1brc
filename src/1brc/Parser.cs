using System.Text;

namespace _1brc;

public class Parser(string fileName)
{
    private const int BufferSize = 8192;
    private const int ChunkSize = 128;

    private const int NewLine = 10;
    private const int Separator = 59;

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

    private FileChunk[] GetChunks()
    {
        var chunks = new FileChunk[Environment.ProcessorCount];
        using var reader = OpenReader(FileOptions.RandomAccess);
        var chunkSize = reader.Length / chunks.Length;
        var buffer = new byte[1];
        var start = 0L;
        var chunk = 0;
        reader.Position = chunkSize;
        while (reader.Read(buffer) > 0)
        {
            if (buffer[0] == 10)
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
        }

        return chunks;
    }

    private Dictionary<byte[], Location> ProcessChunk(FileChunk chunk)
    {
        Dictionary<byte[], Location> data = new(10000, new BytesComparer());

        using var reader = OpenReader();
        reader.Position = chunk.Start;
        Span<byte> buffer = new byte[ChunkSize];
        long end = chunk.Start + chunk.Count;
        int index = 0;
        int separator = 0;
        while (reader.Position < end)
        {
            buffer[index] = (byte)reader.ReadByte();
            if (buffer[index] == Separator) separator = index;
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

                index = 0;
                continue;
            }

            index++;
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