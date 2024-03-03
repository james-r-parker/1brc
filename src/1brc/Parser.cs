using System.Text;

namespace _1brc;

public class Parser(string fileName)
{
    private const byte NewLine = (byte)'\n';
    private const byte Separator = (byte)';';
    private const byte Dot = (byte)'.';
    private const byte A = (byte)'A';
    private const byte Z = (byte)'Z';
    private const byte a = (byte)'a';
    private const byte z = (byte)'z';

    public IEnumerable<Output> Run()
    {
        List<(Thread Thread, Unit Unit)> units = new();
        foreach ((Thread Thread, Unit Unit) unit in GetChunks())
        {
            units.Add(unit);
            unit.Thread.Join();
        }

        return units.SelectMany(x => x.Unit.Results)
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
    private IEnumerable<(Thread Thread, Unit Unit)> GetChunks()
    {
        using var reader = Helpers.OpenReader(fileName, FileOptions.RandomAccess);
        long chunkSize = reader.Length / Environment.ProcessorCount;
        byte[] buffer = new byte[1];
        long start = 0L;
        reader.Position = chunkSize;
        while (reader.Read(buffer) > 0)
        {
            if (buffer[0] == Dot)
            {
                yield return Start(new FileChunk(start, (reader.Position + 2) - start));
                start = reader.Position + 2;
                if (reader.Position + chunkSize > reader.Length)
                {
                    yield return Start(new FileChunk(start, reader.Length - start));
                    break;
                }

                reader.Position += chunkSize;
            }
            else if (buffer[0] == NewLine)
            {
                yield return Start(new FileChunk(start, reader.Position - start));
                start = reader.Position;
                if (reader.Position + chunkSize > reader.Length)
                {
                    yield return Start(new FileChunk(start, reader.Length - start));
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
    }
    
    private (Thread Thread, Unit Unit) Start(FileChunk chunk)
    {
        var u = new Unit(fileName, chunk);
        var t = new Thread(u.Run)
        {
            Priority = ThreadPriority.AboveNormal
        };
        t.Start();
        return (t, u);
    }
}