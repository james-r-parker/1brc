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
    public double Avg => Math.Round(Sum / Count, 1, MidpointRounding.AwayFromZero);
}

public record FileChunk(long Start, long Count);

public struct Location(ReadOnlyMemory<byte> name, double temp)
{
    public readonly ReadOnlyMemory<byte> Name = name;
    public double Min = temp;
    public double Max = temp;
    public double Sum = temp;
    public int Count = 1;

    public void Update(double temp)
    {
        Count++;
        Sum += temp;
        if(temp > Max)
        {
            Max = temp;
        }
        else if(temp < Min)
        {
            Min = temp;
        }
    }
}