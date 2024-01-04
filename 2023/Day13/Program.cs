var lines = File.ReadAllLines("Input.txt").ToList();

lines.Add(string.Empty); // to make parsing easier

var inputs = new List<List<string>>();
var input = new List<string>();
foreach(var line in lines)
{
    if(string.IsNullOrWhiteSpace(line))
    {
        inputs.Add(input);
        input = new List<string>();
        continue;
    }
    input.Add(line);
}

int Solve(int exactRequiredMismatches)
{
    var sum = 0;
    foreach (var map in inputs)
    {
        // look for horizontal lines
        for (var split = 1; split < map.Count(); split++)
        {
            var mismatches = 0;
            var currentRowOffset = 1;
            while (mismatches <= exactRequiredMismatches
                && split - currentRowOffset >= 0
                && split + currentRowOffset - 1 < map.Count())
            {
                for (var col = 0; col < map.First().Count(); col++)
                {
                    if (map[split - currentRowOffset][col] != map[split + currentRowOffset - 1][col])
                    {
                        mismatches++;
                        if(mismatches > exactRequiredMismatches)
                            break;
                    }
                }

                currentRowOffset++;
            }
            if (mismatches == exactRequiredMismatches)
            {
                sum += 100 * split;
                break;
            }
        }

        // look for vertical lines
        for (var split = 1; split < map.First().Count(); split++)
        {
            var mismatches = 0;
            var currentColOffset = 1;
            while (mismatches <= exactRequiredMismatches
                && split - currentColOffset >= 0
                && split + currentColOffset - 1 < map.First().Count())
            {
                for (var row = 0; row < map.Count(); row++)
                {
                    if (map[row][split - currentColOffset] != map[row][split + currentColOffset - 1])
                    {
                        mismatches++;
                        if (mismatches > exactRequiredMismatches)
                            break;
                    }
                }

                currentColOffset++;
            }
            if (mismatches == exactRequiredMismatches)
            {
                sum += split;
                break;
            }
        }
    }
    return sum;
}

Console.WriteLine("Part1: " + Solve(exactRequiredMismatches: 0));
Console.WriteLine("Part2: " + Solve(exactRequiredMismatches: 1));
Console.ReadKey();
