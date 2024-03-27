using System.Diagnostics;
using _1brc;

const string fileName = @"C:\code\1brc\data\measurements-1000000000.txt";
const int runs = 5;
List<TimeSpan> times = new();

for(var i = 0; i < runs; i++)
{
    var stopWatch = new Stopwatch();
    stopWatch.Start();
    var parser = new Parser(fileName, Environment.ProcessorCount);
    parser.Run();
    var result = parser.GetResults();
    stopWatch.Stop();
    times.Add(stopWatch.Elapsed);
    
    GC.Collect();
    GC.WaitForPendingFinalizers();
    await Task.Delay(1000);
}

var sorted = times.OrderBy(x => x.Ticks).ToList();
var avg = sorted.Skip(1).Take(runs - 2).Average(x => x.TotalSeconds);
Console.WriteLine($"Score: {avg:F} Seconds");
