using System.Text;

namespace _1brc;

public class Parser(string fileName, int threads)
{
    private const byte NewLine = (byte)'\n';
    private const byte Separator = (byte)';';
    private const byte Dot = (byte)'.';
    private const byte A = (byte)'A';
    private const byte Z = (byte)'Z';
    private const byte a = (byte)'a';
    private const byte z = (byte)'z';

    private readonly List<(Thread Thread, Unit Unit)> _units = [];

    public void Run()
    {
        foreach (var unit in GetChunks())
        {
            _units.Add(unit);
        }

        foreach (var unit in _units)
        {
            unit.Thread.Join();
        }
    }

    public IEnumerable<Output> GetResults()
    {
        var output = new SortedDictionary<string, Output>();

        foreach (var unit in _units)
        {
            foreach (Location result in unit.Unit.GetResults())
            {
                var name = Encoding.UTF8.GetString(result.Name);
                if (output.TryGetValue(name, out var o))
                {
                    o.Count += result.Count;
                    o.Min = Math.Min(o.Min, result.Min);
                    o.Max = Math.Max(o.Max, result.Max);
                    o.Sum += result.Sum;
                }
                else
                {
                    output.Add(name, new Output()
                    {
                        Name = name,
                        Count = result.Count,
                        Min = result.Min,
                        Max = result.Max,
                        Sum = result.Sum,
                    });
                }
            }
        }
        
        return output.Values;
    }

    /// <summary>
    /// Breaks the file down into chunks for parallel processing.
    /// </summary>
    private IEnumerable<(Thread Thread, Unit Unit)> GetChunks()
    {
        using var reader = Helpers.OpenReader(fileName, FileOptions.RandomAccess);
        long chunkSize = reader.Length / threads;
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