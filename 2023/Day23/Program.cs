using System.Data;
using System.Diagnostics;

var lines = File.ReadAllLines("Input.txt");
var sw = Stopwatch.StartNew();

var rowMax = lines.Length;
var colMax = lines[0].Length;

var charToTile = new Dictionary<char, Tile>
{
    { '#', Tile.Forest },
    { '.', Tile.Path },
    { '>', Tile.SlopeRight },
    { 'v', Tile.SlopeDown },
    { '<', Tile.SlopeLeft },
    { '^', Tile.SlopeUp },
    { 'S', Tile.Start },
    { 'X', Tile.End },
};

var slopeAllowedFastForwards = new Dictionary<Tile, (int drow, int dcol)>
{
    { Tile.SlopeUp, (-1, 0) },
    { Tile.SlopeDown, (1, 0) },
    { Tile.SlopeLeft, (0, -1) },
    { Tile.SlopeRight, (0, 1) },
};

var map = new Tile[rowMax][];
for(var i = 0; i < rowMax; i++)
{
    map[i] = new Tile[colMax];
    for (var j = 0; j < colMax; j++)
        map[i][j] = charToTile[lines[i][j]];
}
//mark start and end manually, seems to be always topleft & botright
map[0][1] = Tile.Start;
map[^1][^2] = Tile.End;

bool IsSlope((int row, int col) pos)
{
    return new Tile[4] { Tile.SlopeUp, Tile.SlopeLeft, Tile.SlopeRight, Tile.SlopeDown }
        .Contains(map[pos.row][pos.col]);
}

int Solve(bool slopesArePaths)
{
    var queue = new Queue<State>();
    queue.Enqueue(new State
    {
        // start pos
        CurrentPos = (0, 1),
        History = new() { (0, 1) },
    });

    var longestPath = int.MinValue;
    while (queue.TryDequeue(out var state))
    {
        var possibleNextPositions = new (int row, int col)[4]
        {
            (1, 0),
            (-1, 0),
            (0, 1),
            (0, -1),
        };

        foreach ((var drow, var dcol) in possibleNextPositions)
        {
            (int row, int col) pos = (state.CurrentPos.row + drow, state.CurrentPos.col + dcol);

            // bound check
            if (!(0 <= pos.row && pos.row < rowMax && 0 <= pos.col && pos.col < colMax))
                continue;

            // history check, cant walk same tile twice
            if (state.History.Contains(pos))
                continue;

            var newHistory = new HashSet<(int row, int col)>(state.History) { pos };

            // forward slope tiles (input shows they are never chained, so no while loop is required)
            if (!slopesArePaths && IsSlope(pos))
            {
                if (slopeAllowedFastForwards[map[pos.row][pos.col]] != (drow, dcol))
                    continue;
                pos = (pos.row + drow, pos.col + dcol);
                if (newHistory.Contains(pos))
                    continue;
                newHistory.Add(pos);
            }

            switch (map[pos.row][pos.col])
            {
                case Tile.Forest:
                    break;
                case Tile.SlopeUp when slopesArePaths:
                case Tile.SlopeDown when slopesArePaths:
                case Tile.SlopeRight when slopesArePaths:
                case Tile.SlopeLeft when slopesArePaths:
                case Tile.Path:
                    queue.Enqueue(new State
                    {
                        CurrentPos = pos,
                        History = newHistory,
                    });
                    break;
                case Tile.End:
                    if (false)
                    {
                        Console.WriteLine("Found path of length: " + state.History.Count());

                        //print map for debugging purposes
                        var debugMap = map
                            .Select(
                                row => row.Select(tile => charToTile.ToList().Single(kvp => kvp.Value == tile).Key).ToArray()
                                ).ToArray();
                        foreach ((var row, var col) in state.History)
                            debugMap[row][col] = 'O';
                        for (var i = 0; i < debugMap.Length; i++)
                            Console.WriteLine(new string(debugMap[i]));
                    }
                    longestPath = Math.Max(longestPath, state.History.Count());
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }
        }

    }
    return longestPath;
}


Console.WriteLine("Part1: " + Solve(slopesArePaths: false));

// too slow for part2, have to optimize
//Console.WriteLine("Part2: " + Solve(slopesArePaths: true));

// notice there are many single paths going only in 1 direction, we can short circuit these
// modeling the map as a graph

// first get rid of the slopes
for (var i = 0; i < rowMax; i++)
    for (var j = 0; j < colMax; j++)
        if(IsSlope((i, j)))
            map[i][j] = Tile.Path;
map[0][1] = Tile.Path;
map[^1][^2] = Tile.Path;

(int row, int col)[] GetSurroundingPaths((int row, int col) pos)
{
    var possibleNextPositions = new (int row, int col)[4]
    {
        (pos.row + 1, pos.col),
        (pos.row - 1, pos.col),
        (pos.row, pos.col + 1),
        (pos.row, pos.col - 1),
    };
    return possibleNextPositions
        .Where(pos => 0 <= pos.row && pos.row < rowMax)
        .Where(pos => 0 <= pos.col && pos.col < colMax)
        .Where(pos => map[pos.row][pos.col] == Tile.Path)
        .ToArray();
}

// reconstruct the graph
var uniDirectionalGraph = new Dictionary<(int row, int col), Dictionary<(int row, int col), int>>();
var alreadyExplored = new HashSet<(int row, int col)> { (0, 1) };
var graphQueue = new Queue<((int row, int col) prev, (int row, int col) current)>();
graphQueue.Enqueue(((0, 1), (1, 1)));
while(graphQueue.TryDequeue(out var next))
{
    var edgeWeight = 1;
    (var prev, var current) = next;
    if (alreadyExplored.Contains(current))
        continue;
    alreadyExplored.Add(current);
    var surroundingPaths = GetSurroundingPaths(current);
    while(surroundingPaths.Length == 2)
    {
        var prevCopy = prev;
        prev = current;
        current = surroundingPaths.Single(pos => pos != prevCopy);
        edgeWeight++;
        surroundingPaths = GetSurroundingPaths(current);
        if(alreadyExplored.Contains(current))
            break; // found graph node
        alreadyExplored.Add(current);
    }
    // found graph node
    if (!uniDirectionalGraph.TryGetValue(next.prev, out var nodeDict))
        uniDirectionalGraph[next.prev] = new();
    uniDirectionalGraph[next.prev].Add(current, edgeWeight);
    foreach (var surroundingPath in surroundingPaths.Where(path => path != prev))
        if(!graphQueue.Contains((current, surroundingPath)) && !alreadyExplored.Contains(surroundingPath))
            graphQueue.Enqueue((current, surroundingPath));

    if(false) // print map in ascii for debugging purposes
    {
        var debugMap = map
            .Select(
                row => row.Select(tile => charToTile.ToList().Single(kvp => kvp.Value == tile).Key).ToArray()
                ).ToArray();
        foreach ((var row, var col) in alreadyExplored)
            debugMap[row][col] = 'O';
        foreach ((var row, var col) in uniDirectionalGraph.Keys)
            debugMap[row][col] = 'N';
        Console.Clear();
        for (var i = 0; i < debugMap.Length; i++)
            Console.WriteLine(new string(debugMap[i]));
    }
}

// we have found all edges, but to make lookups easier, add both sides to the dictionary
// A -> B as well as B -> A
var biDirectionalGraph = new Dictionary<(int row, int col), Dictionary<(int row, int col), int>>();
foreach((var src, var dests) in uniDirectionalGraph)
{
    foreach((var dest, var dist) in dests)
    {
        // orig
        if (!biDirectionalGraph.ContainsKey(src))
            biDirectionalGraph[src] = new();
        if (!biDirectionalGraph[src].ContainsKey(src))
            biDirectionalGraph[src][dest] = dist;

        // inverted
        if (!biDirectionalGraph.ContainsKey(dest))
            biDirectionalGraph[dest] = new();
        if (!biDirectionalGraph[dest].ContainsKey(src))
            biDirectionalGraph[dest][src] = dist;
    }
}

// now that we have the graph, we can brute force the solution (no polynomial time solution as this is a NP-hard problem)
// brute force with simple DFS

int DFS((int row, int col) currentVertex, HashSet<(int row, int col)> visited, int pathLength = 0)
{
    if (currentVertex == (rowMax - 1, colMax - 2)) // end node
        return pathLength;

    var longestPath = int.MinValue;
    var neighbours = biDirectionalGraph[currentVertex]
        .Where(dest => !visited.Contains(dest.Key));
    foreach((var neighbour, var dist) in neighbours)
    {
        visited.Add(neighbour);
        var neighbourPath = DFS(neighbour, visited, pathLength + dist);
        visited.Remove(neighbour);
        longestPath = Math.Max(longestPath, neighbourPath);
    }
    return longestPath;
}

Console.WriteLine("Part2: " + DFS((0, 1), new()));
sw.Stop();
Console.WriteLine($"Took {sw.ElapsedMilliseconds} ms");
Console.ReadKey();

enum Tile
{
    Start,
    End,
    Forest,
    Path,
    SlopeUp,
    SlopeDown,
    SlopeLeft,
    SlopeRight,
}

struct State
{
    public (int row, int col) CurrentPos;
    public HashSet<(int row, int col)> History; 
}