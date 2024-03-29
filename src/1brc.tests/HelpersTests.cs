using System.Text;

namespace _1brc.tests;

public class HelpersTests
{
    private readonly DoubleComparer _comparer = new();
    private const string FileName = @"C:\code\1brc\data\measurements-1000000.txt";
    
    [Fact]
    public void Numbers()
    {
        var number = Enumerable.Range(0, 100).SelectMany(x =>
        {
            return Enumerable.Range(0, 10).Select(y => $"{x}.{y}");
        }).ToList();
        
        foreach (var n in number)
        {
            var bytes = Encoding.UTF8.GetBytes(n).AsSpan();
            var result = Helpers.ParseDouble(bytes);
            var expected = double.Parse(Encoding.UTF8.GetString(bytes));
            Assert.Equal(expected, result,_comparer);
        }
        
        foreach (var n in number)
        {
            var bytes = Encoding.UTF8.GetBytes($"-{n}").AsSpan();
            var result = Helpers.ParseDouble(bytes);
            var expected = double.Parse(Encoding.UTF8.GetString(bytes));
            Assert.Equal(expected, result,_comparer);
        }
    }
}