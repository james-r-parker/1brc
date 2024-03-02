using System.Data;

namespace _1brc;

public class Output
{
    public required string Name { get; init; }
    public required int Count { get; init; }
    public required double Min { get; init; }
    public required double Max { get; init; }
    public required double Sum { get; init; }

    public double Avg => Math.Round(Sum / Count, 2, MidpointRounding.AwayFromZero);
}

public record FileChunk(long Start, long Count);

public class Location(double temp)
{
    public double Min { get; private set; } = temp;
    public double Max { get; private set; } = temp;
    public double Sum { get; private set; } = temp;
    public int Count { get; private set; } = 1;

    public double Avg => Math.Round(Sum / Count, 2, MidpointRounding.AwayFromZero);

    public void Update(double temp)
    {
        Count++;
        Min = Math.Min(Min, temp);
        Max = Math.Max(Max, temp);
        Sum += temp;
    }
}