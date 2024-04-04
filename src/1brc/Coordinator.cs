using System.Text;

namespace _1brc;

public class Coordinator(string fileName, int threads)
{
    private const byte NewLine = (byte)'\n';
    private const byte Separator = (byte)';';
    private const byte Dot = (byte)'.';
    private const byte A = (byte)'A';
    private const byte Z = (byte)'Z';
    private const byte a = (byte)'a';
    private const byte z = (byte)'z';

    private readonly List<(Thread Thread, UnitOfWork Unit)> _units = [];

    private string _output = string.Empty;
    private readonly SortedDictionary<string, Output> _results = new();

    public string Output => _output;
    public IReadOnlyDictionary<string, Output> Results => _results;

    public void Run()
    {
        GC.TryStartNoGCRegion(1024 * 1024 * 10, true);

        foreach (var unit in GetChunks())
        {
            _units.Add(unit);
        }

        var temp = new Dictionary<ReadOnlyMemory<byte>, Output>(1000, new BytesComparer());
        foreach (var unit in _units)
        {
            unit.Thread.Join();

            foreach (Location result in unit.Unit.GetResults())
            {
                if (temp.TryGetValue(result.Name, out var o))
                {
                    o.Count += result.Count;
                    o.Min = o.Min < result.Min ? o.Min : result.Min;
                    o.Max = o.Max > result.Max ? o.Max : result.Max;
                    o.Sum += result.Sum;
                }
                else
                {
                    temp.Add(result.Name, new Output()
                    {
                        NameBytes = result.Name,
                        Count = result.Count,
                        Min = result.Min,
                        Max = result.Max,
                        Sum = result.Sum,
                    });
                }
            }
        }

        foreach (var (name, o) in temp)
        {
            _results.Add(o.Name, o);
        }

        var sb = new StringBuilder("{", 10000);
        foreach (var result in _results)
        {
            sb.AppendFormat("{0}={1:0.0}/{2:0.0}/{3:0.0},", result.Key, result.Value.Min, result.Value.Avg,
                result.Value.Max);
        }

        sb.Remove(sb.Length - 1, 1);
        sb.Append('}');
        _output = sb.ToString();
    }

    /// <summary>
    /// Breaks the file down into chunks for parallel processing.
    /// </summary>
    private IEnumerable<(Thread Thread, UnitOfWork Unit)> GetChunks()
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

    /// <summary>
    /// Starts a new unit of work to process a file chunk on a different thread.
    /// </summary>
    /// <param name="chunk">The chunk of the file to process in this thread.</param>
    private (Thread Thread, UnitOfWork Unit) Start(FileChunk chunk)
    {
        var u = new UnitOfWork(fileName, chunk);
        var t = new Thread(u.Run)
        {
            Priority = ThreadPriority.AboveNormal
        };
        t.Start();
        return (t, u);
    }
}