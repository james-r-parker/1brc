using System.Diagnostics;
using _1brc;


var stopWatch = new Stopwatch();
stopWatch.Start();
var parser = new Parser(@"C:\code\1brc\data\measurements-1000000000.txt");
var result = await parser.Run();
stopWatch.Stop();
Console.WriteLine($"Time: {stopWatch.Elapsed}");

foreach (var output in result.OrderByDescending(x => x.Count).Take(5))
{
    Console.WriteLine($"\"{output.Name}\" - Count: {output.Count} Min: {output.Min}, Max: {output.Max}, Avg: {output.Avg}");
}
