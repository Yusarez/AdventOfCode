using System.Diagnostics;

var lines = File.ReadAllLines("Input.txt").Select(s => s.ToCharArray()).ToArray();

const char ROLLING_ROCK = 'O';
const char EMTPY = '.';
const char STATIONARY_ROCK = '#';

bool IsVertical(Tilt tilt)
    => tilt == Tilt.North || tilt == Tilt.South;

int CalculateNorthLoad()
{
    var load = 0;
    for (var col = 0; col < lines.First().Count(); col++)
        for (var row = 0; row < lines.Count(); row++)
            if (lines[row][col] == ROLLING_ROCK)
                load += lines.Count() - row;
    return load;
}

void TiltMap(Tilt tilt)
{
    var firstAxisMax = tilt switch
    {
        Tilt.North => lines.First().Count(),
        Tilt.West => lines.Count(),
        Tilt.South => lines.First().Count(),
        Tilt.East => lines.Count(),
    };
    var secondAxisMinMax = tilt switch
    {
        Tilt.North => (0, lines.Count(), 1),
        Tilt.West => (0, lines.First().Count(), 1),
        Tilt.South => (lines.Count() - 1, -1, -1),
        Tilt.East => (lines.First().Count() - 1, -1, -1),
    };

    for (var firstAxis = 0; firstAxis != firstAxisMax; firstAxis++)
    {
        var nextRollingRockPosition = secondAxisMinMax.Item1;
        for (var secondAxis = secondAxisMinMax.Item1; secondAxis != secondAxisMinMax.Item2; secondAxis += secondAxisMinMax.Item3)
        {
            (var row, var col) = tilt switch
            {
                Tilt.North => (secondAxis, firstAxis),
                Tilt.West => (firstAxis, secondAxis),
                Tilt.South => (secondAxis, firstAxis),
                Tilt.East => (firstAxis, secondAxis),
            };

            switch (lines[row][col])
            {
                case STATIONARY_ROCK:
                    {
                        if(IsVertical(tilt))
                            nextRollingRockPosition = row + secondAxisMinMax.Item3;
                        else
                            nextRollingRockPosition = col + secondAxisMinMax.Item3;
                        break;
                    }
                case ROLLING_ROCK:
                    {
                        // move boulder
                        if (nextRollingRockPosition == (IsVertical(tilt) ? row : col))
                        {
                            nextRollingRockPosition += secondAxisMinMax.Item3;
                            break;
                        }
                        if(IsVertical(tilt))
                        {
                            lines[nextRollingRockPosition][col] = ROLLING_ROCK;
                            lines[row][col] = EMTPY;
                            nextRollingRockPosition += secondAxisMinMax.Item3;
                            while (lines[nextRollingRockPosition][col] == STATIONARY_ROCK) nextRollingRockPosition += secondAxisMinMax.Item3;
                        }
                        else
                        {
                            lines[row][nextRollingRockPosition] = ROLLING_ROCK;
                            lines[row][col] = EMTPY;
                            nextRollingRockPosition += secondAxisMinMax.Item3;
                            while (lines[row][nextRollingRockPosition] == STATIONARY_ROCK) nextRollingRockPosition += secondAxisMinMax.Item3;
                        }

                        break;
                    }
                default:
                    break;
            }
        }
    }
}

void CycleTilts()
{
    TiltMap(Tilt.North);
    TiltMap(Tilt.West);
    TiltMap(Tilt.South);
    TiltMap(Tilt.East);
}

char[][] GetDeepCopy(char[][] map)
{
    var copy = new char[map.Length][];
    for(var i = 0; i < map.Length; i++)
    {
        copy[i] = new char[map[i].Length];
        Array.Copy(map[i], copy[i], map[i].Length);
    }
    return copy;
}

bool AreEqual(char[][] map1, char[][] map2)
{
    Debug.Assert(map1.Length == map2.Length);
    for (var row = 0; row < map1.Length; row++)
        for (var col = 0; col < map1[0].Length; col++)
            if (map1[row][col] != map2[row][col])
                return false;
    return true;
}

/*
TiltMap(Tilt.North);
Console.WriteLine("Part1: " + CalculateNorthLoad());
*/

var totalCyclesToDo = 1_000_000_000;
var cyclesDone = 0;
var mapHistory = new List<char[][]>();
while (cyclesDone < totalCyclesToDo)
{
    CycleTilts();
    cyclesDone++;
    var historyIndex = -1;
    for(var histIdx = 0; histIdx < mapHistory.Count; histIdx++)
    {
        if (AreEqual(lines, mapHistory[histIdx]))
        {
            historyIndex = histIdx;
            break;
        }
    }

    if(historyIndex == -1)
    {
        mapHistory.Add(GetDeepCopy(lines));
        continue;
    }

    // found a cycle, fast forward a bunch of cycles
    var cycleLength = cyclesDone - historyIndex - 1;
    if (totalCyclesToDo - cyclesDone < cycleLength)
        continue; //already forwarded

    cyclesDone = totalCyclesToDo  - (totalCyclesToDo - cyclesDone) % cycleLength;
}

Console.WriteLine("Part2: " + CalculateNorthLoad());
Console.ReadKey();


enum Tilt
{
    North,
    West,
    South,
    East,
}