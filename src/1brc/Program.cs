using System.Diagnostics;
using _1brc;

const string fileName = @"C:\code\1brc\data\measurements-1000000000.txt";
const int runs = 5;
List<TimeSpan> times = new();

for(var i = 0; i < runs; i++) {
    var stopWatch = new Stopwatch();
    stopWatch.Start();
    var parser = new Parser(fileName);
    var result = parser.Run();
    stopWatch.Stop();
    times.Add(stopWatch.Elapsed);
}

Console.WriteLine($"Avg time: {new TimeSpan((long)times.Average(x => x.Ticks))}");
Console.WriteLine($"Min time: {new TimeSpan((long)times.Min(x => x.Ticks))}");
Console.WriteLine($"Max time: {new TimeSpan((long)times.Max(x => x.Ticks))}");
