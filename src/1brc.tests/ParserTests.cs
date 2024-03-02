﻿using System.Diagnostics;

namespace _1brc.tests;

public class ParserTests
{
    private const string FileName = @"C:\code\1brc\data\measurements-1000000.txt";
    
    [Fact]
    public async Task Count()
    {
        var parser = new Parser(FileName);
        var result = await parser.Run();
        Assert.Equal(1000000, result.Sum(x => x.Count));
    }

    [Fact]
    public async Task Compare()
    {
        Dictionary<string, Location> data = new();
        using var reader = new StreamReader(FileName);
        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            var parts = line.Split(';');
            if (data.TryGetValue(parts[0], out var location))
            {
                location.Update(double.Parse(parts[1]));
            }
            else
            {
                data.Add(parts[0], new Location(double.Parse(parts[1])));
            }
        }
        
        Assert.Equal(1000000, data.Select(x => x).Sum(x => x.Value.Count));
        
        var parser = new Parser(FileName);
        var result = await parser.Run();

        foreach (var name in data.Keys)
        {
            Assert.Contains(result, x => x.Name == name);
        }
        
        foreach (var output in result)
        {
            Assert.Equal(data[output.Name].Min, output.Min);
            Assert.Equal(data[output.Name].Max, output.Max);
            Assert.Equal(data[output.Name].Avg, output.Avg);
            Assert.Equal(data[output.Name].Count, output.Count);
        }
    }
}