
var lines = File.ReadAllLines("Input.txt");
var maxCounts = new Dictionary<string, int>
{
    ["red"] = 12,
    ["green"] = 13,
    ["blue"] = 14,
};

Part2();

void Part1()
{
    var sumOfIds = 0;
    foreach (var line in lines)
    {
        (var gameId, var gameSets) = ParseLine(line);
        var impossibleGame = false;
        foreach(var gameSet in gameSets)
        {
            if (impossibleGame) break;
            foreach ((var color, var count) in gameSet)
                if (count > maxCounts[color])
                    impossibleGame = true;
        }
        if (!impossibleGame)
            sumOfIds += gameId;
    }

    Console.WriteLine("Sum: " + sumOfIds);
    Console.ReadKey();
}

void Part2()
{
    var sumOfPowers = 0;
    foreach (var line in lines)
    {
        (var gameId, var gameSets) = ParseLine(line);
        var minCounts = new Dictionary<string, int>
        {
            ["red"] = 0,
            ["green"] = 0,
            ["blue"] = 0,
        };
        foreach (var gameSet in gameSets)
        {
            foreach ((var color, var count) in gameSet)
                if (count > minCounts[color])
                    minCounts[color] = count;
        }
        sumOfPowers += minCounts.Aggregate(1, (x, y) => x * y.Value);
    }
    Console.WriteLine("Sum: " + sumOfPowers);
    Console.ReadKey();
}

(int gameId, List<Dictionary<string, int>> gameSets) ParseLine(string line)
{
    var colonParts = line.Split(":");
    var gameId = int.Parse(colonParts[0].Split(" ")[1]);
    var gameSets = new List<Dictionary<string, int>>();
    foreach (var gameSet in colonParts[1].Split(";"))
    {
        Dictionary<string, int> counts = new();
        foreach (var colors in gameSet.Split(","))
        {
            var parts = colors.Split(" ");
            counts[parts[2]] = int.Parse(parts[1]);
        }
        gameSets.Add(counts);
    }
    return (gameId, gameSets);
}