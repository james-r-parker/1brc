using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace _1brc.tests;

public class ParserTests
{
    private readonly DoubleComparer _comparer = new();
    private const string FileName = @"C:\code\1brc\data\measurements-1000000.txt";

    [Theory]
    [InlineData(2)]
    [InlineData(4)]
    [InlineData(8)]
    [InlineData(16)]
    [InlineData(30)]
    [InlineData(32)]
    public void Count(int threads)
    {
        var parser = new Parser(FileName, threads);
        parser.Run();
        var result = parser.GetResults();
        Assert.Equal(1000000, result.Sum(x => x.Count));
    }

    [Theory]
    [InlineData(2)]
    [InlineData(4)]
    [InlineData(8)]
    [InlineData(16)]
    [InlineData(32)]
    public void Compare(int threads)
    {
        Dictionary<string, Location> data = new();
        using var reader = new StreamReader(FileName);
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            var parts = line.Split(';');
            ref var location = ref CollectionsMarshal.GetValueRefOrNullRef(data, parts[0]);
            if (!Unsafe.IsNullRef(ref location))
            {
                location.Update(double.Parse(parts[1]));
            }
            else
            {
                data.Add(parts[0],
                    new Location(Encoding.UTF8.GetBytes(parts[0]), parts[0].GetHashCode(), double.Parse(parts[1])));
            }
        }

        Assert.Equal(1000000, data.Select(x => x).Sum(x => x.Value.Count));

        var parser = new Parser(FileName, threads);
        parser.Run();
        var result = parser.GetResults();

        foreach (var name in data.Keys)
        {
            Assert.Contains(result, x => x.Name == name);
        }

        foreach (var output in result)
        {
            Assert.Equal(data[output.Name].Min, output.Min, _comparer);
            Assert.Equal(data[output.Name].Max, output.Max, _comparer);
            Assert.Equal(Math.Round(data[output.Name].Sum / data[output.Name].Count, 2, MidpointRounding.AwayFromZero), output.Avg, _comparer);
            Assert.Equal(data[output.Name].Count, output.Count, _comparer);
        }
    }
}