using System.Text;

namespace _1brc;

public class Output
{
    public required ReadOnlyMemory<byte> NameBytes { get; init; }
    public string Name => Encoding.UTF8.GetString(NameBytes.Span);
    public required int Count { get; set; }
    public required double Min { get; set; }
    public required double Max { get; set; }
    public required double Sum { get; set; }
    public double Avg => Math.Round(Sum / Count, 2, MidpointRounding.AwayFromZero);
}

public record FileChunk(long Start, long Count);

public class Location
{
    public Location(byte[] name, int hashCode, double temp)
    {
        Name = name;
        HashCode = hashCode;
        Min = temp;
        Max = temp;
        Sum = temp;
    }
    
    public readonly byte[] Name;
    public readonly int HashCode;
    public double Min;
    public double Max;
    public double Sum;
    public int Count = 1;

    public void Update(double temp)
    {
        unchecked
        {
            Count++;
            Min = Min < temp ? Min : temp;
            Max = Max > temp ? Max : temp;
            Sum += temp;
        }
    }
}