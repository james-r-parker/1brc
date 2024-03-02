using System.Diagnostics;
using _1brc;

var stopWatch = new Stopwatch();
stopWatch.Start();
var parser = new Parser(@"C:\code\1brc\data\measurements-1000000000.txt");
var result = await parser.Run();
stopWatch.Stop();
Console.WriteLine($"Time: {stopWatch.Elapsed}");

foreach (var output in result.Take(10))
{
    Console.WriteLine($"{output.Name}={output.Min}/{output.Avg}/{output.Max}");
}
