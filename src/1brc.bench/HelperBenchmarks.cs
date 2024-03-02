using System.Text;
using BenchmarkDotNet.Attributes;

namespace _1brc.bench;

public class HelperBenchmarks
{
    private readonly byte[] _bytes = "99.99"u8.ToArray();
    
    [Benchmark]
    public double ParseDouble() => Helpers.ParseDouble(_bytes);
    
    [Benchmark]
    public double ParseDoubleDefault() => double.Parse(Encoding.UTF8.GetString(_bytes));
}