var lines = File.ReadAllLines("Input.txt");

const int INVALID_HEAT = -1;

var rowMax = lines.Length;
var colMax = lines.First().Length;

var heats = lines
    .Select(line => line.Select(ch => int.Parse(ch.ToString())).ToArray())
    .ToArray();

bool IsInBounds(int row, int col)
    => 0 <= row && row < rowMax && 0 <= col && col < colMax;

int Solve(int maxStraightDistance, int minStraightDistance = 0)
{
    var history = new Dictionary<(int row, int col, int drow, int dcol, int dist), int>();
    var queue = new PriorityQueue<(int row, int col, int drow, int dcol, int dist), int>();
    queue.Enqueue((0, 1, 0, 1, 1), heats[0][1]);
    queue.Enqueue((1, 0, 1, 0, 1), heats[1][0]);

    //declare func in func to access outer variables, if done properly, move to class
    int Explore((int row, int col, int drow, int dcol, int dist) pos, int heat)
    {
        (int row, int col, int drow, int dcol, int dist) = pos;
        var nextRow = row + drow;
        var nextCol = col + dcol;

        if (!IsInBounds(nextRow, nextCol))
            return INVALID_HEAT;

        var nextHeat = heat + heats[nextRow][nextCol];
        if (nextRow == rowMax - 1 
            && nextCol == colMax - 1
            && dist >= minStraightDistance)
            return nextHeat;

        var nextKey = (nextRow, nextCol, drow, dcol, dist);
        if (!history.ContainsKey(nextKey))
        {
            history[nextKey] = nextHeat;
            queue.Enqueue(nextKey, nextHeat);
        }
        return INVALID_HEAT;
    }

    var minHeat = INVALID_HEAT;
    while (minHeat == INVALID_HEAT && queue.TryDequeue(out var currentPos, out var heat))
    {
        (int row, int col, int drow, int dcol, int dist) = currentPos;

        var toExplore = new List<(int row, int col, int drow, int dcol, int dist)>(3);

        // do left & right turns
        // do rotation matrix on dx, dy so newDy = oldDx and newDx = -oldDy etc
        if(dist >= minStraightDistance)
        {
            toExplore.Add((row, col, dcol, -drow, 1));
            toExplore.Add((row, col, -dcol, drow, 1));
        }

        //go straight
        if (dist < maxStraightDistance)
            toExplore.Add((row, col, drow, dcol, dist + 1));

        foreach (var pos in toExplore)
        {
            var finalHeat = Explore(pos, heat);
            if (finalHeat != INVALID_HEAT)
            {
                minHeat = finalHeat;
                break;
            }
        }
    }
    return minHeat;
}

Console.WriteLine("Part1: " + Solve(maxStraightDistance: 3));
Console.WriteLine("Part2: " + Solve(maxStraightDistance: 10, minStraightDistance: 4));
Console.ReadKey();