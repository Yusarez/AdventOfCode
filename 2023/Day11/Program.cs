var lines = File.ReadAllLines("Input.txt").ToList();

long SolveForExpansionRate(int expansionFactor)
{
    // find horizontal expansion indices
    var horizontalBoundaries = new List<int>();
    var i = 0;
    while (i < lines.Count)
    {
        if (lines[i].All(ch => ch == '.'))
            horizontalBoundaries.Add(i);
        i++;
    }

    // find vertical expansion indices
    var verticalBoundaries = new List<int>();
    i = 0;
    while (i < lines.First().Count())
    {
        if (lines.Select(line => line[i]).All(ch => ch == '.'))
            verticalBoundaries.Add(i);
        i++;
    }

    // find galaxies
    var galaxies = new List<(int row, int col)>();
    for (var row = 0; row < lines.Count(); row++)
        for (var col = 0; col < lines.First().Count(); col++)
            if (lines[row][col] == '#')
                galaxies.Add((row, col));

    // find distances
    var sumOfShortestPaths = 0L;
    for (var j = 0; j < galaxies.Count(); j++)
    {
        for (var k = j + 1; k < galaxies.Count(); k++)
        {
            // add initial distance
            sumOfShortestPaths += Math.Abs(galaxies[k].row - galaxies[j].row) 
                + Math.Abs(galaxies[k].col - galaxies[j].col);

            // add extra space due to expanded boundaries
            foreach (var horizontalBoundary in horizontalBoundaries)
                if ((galaxies[k].row < horizontalBoundary && horizontalBoundary < galaxies[j].row)
                    || (galaxies[j].row < horizontalBoundary && horizontalBoundary < galaxies[k].row))
                    sumOfShortestPaths += expansionFactor - 1; // - 1 because there already was an empty row
            foreach (var verticalBoundary in verticalBoundaries)
                if ((galaxies[k].col < verticalBoundary && verticalBoundary < galaxies[j].col)
                    || (galaxies[j].col < verticalBoundary && verticalBoundary < galaxies[k].col))
                    sumOfShortestPaths += expansionFactor - 1; // - 1 because there already was an empty row
        }
    }
    return sumOfShortestPaths;
}

Console.WriteLine("Part1: " + SolveForExpansionRate(2));
Console.WriteLine("Part2: " + SolveForExpansionRate(1_000_000));
Console.ReadKey();