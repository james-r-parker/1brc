using System.Data;

namespace _1brc;

public class Output
{
    public required string Name { get; set; }
    public required int Count { get; set; }
    public required double Min { get; set; }
    public required double Max { get; set; }
    public required double Sum { get; set; }

    public double Avg => Math.Round(Sum / Count, 2, MidpointRounding.AwayFromZero);
}

public record FileChunk(long Start, long Count);

public class Location(byte[] name, int hashCode, double temp)
{
    public byte[] Name { get; private set; } = name;
    
    public int HashCode { get; private set; } = hashCode;
    public double Min { get; private set; } = temp;
    public double Max { get; private set; } = temp;
    public double Sum { get; private set; } = temp;
    public int Count { get; private set; } = 1;

    public double Avg => Math.Round(Sum / Count, 2, MidpointRounding.AwayFromZero);

    public void Update(double temp)
    {
        unchecked
        {
            Count++;
            Min = Min < temp ? Min : temp;
            Max = Max > temp ? Max : temp;
        }

        Sum += temp;
    }
}