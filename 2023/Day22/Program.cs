using System.Diagnostics;

var lines = File.ReadAllLines("Input.txt");
var sw = Stopwatch.StartNew();

var bricks = lines
    .Select(line => line
        .Split('~')
        .Select(subline => subline.Split(',')
            .Select(int.Parse)
            .ToArray())
        .ToArray())
    .ToArray();

var xMax = bricks.Max(brick => Math.Max(brick[0][0], brick[1][0])) + 1;
var yMax = bricks.Max(brick => Math.Max(brick[0][1], brick[1][1])) + 1;
var zMax = bricks.Max(brick => Math.Max(brick[0][2], brick[1][2])) + 2;

var space = new int[xMax][][];
for (var x = 0; x < xMax; x++)
{
    space[x] = new int[yMax][];
    for (var y = 0; y < yMax; y++)
        space[x][y] = new int[zMax];
}

var bricksOrdered = bricks.OrderBy(brick => Math.Min(brick[0][2], brick[1][2])).ToArray();

var settledBricks = new List<(int[], int[])>();

for(var i = 0; i < bricksOrdered.Length; i++)
{
    var brick = bricksOrdered[i];
    var brickStart = brick[0];
    var brickEnd = brick[1];
    if (brickStart[2] == brickEnd[2])
    {
        // horizontal brick
        var z = brickStart[2];
        var xStart = Math.Min(brickStart[0], brickEnd[0]);
        var xEnd = Math.Max(brickStart[0], brickEnd[0]);
        var yStart = Math.Min(brickStart[1], brickEnd[1]);
        var yEnd = Math.Max(brickStart[1], brickEnd[1]);

        var keepDropping = true;
        while(keepDropping && z > 1)
        {
            for (var xi = xStart; xi <= xEnd; xi++)
            {
                for (var yi = yStart; yi <= yEnd; yi++)
                {
                    if (space[xi][yi][z - 1] != 0)
                    {
                        keepDropping = false;
                        break;
                    }
                }
                if (!keepDropping)
                    break;
            }
            if (keepDropping)
                z--;
        }
        for (var xi = xStart; xi <= xEnd; xi++)
            for (var yi = yStart; yi <= yEnd; yi++)
                space[xi][yi][z] = i + 1;

        settledBricks.Add((new int[] { brickStart[0], brickStart[1], z }, new int[] { brickEnd[0], brickEnd[1], z }));
    }
    else
    {
        // vertical brick
        var x = brickStart[0];
        var y = brickStart[1];
        var z = Math.Min(brickStart[2], brickEnd[2]);
        while (space[x][y][z - 1] == 0 && z > 1)
            z--;
        var newZMax = z + Math.Abs(brickStart[2] - brickEnd[2]);
        for (var zi = z; zi <= newZMax; zi++)
            space[x][y][zi] = i + 1;

        settledBricks.Add((new int[] { x, y, z }, new int[] { x, y, newZMax }));
    }
}

List<int> GetTopBricks(int brickId)
{
    (var brickStart, var brickEnd) = settledBricks[brickId - 1];
    var brickIdsToCheck = new List<int>();

    var xStart = Math.Min(brickStart[0], brickEnd[0]);
    var xEnd = Math.Max(brickStart[0], brickEnd[0]);
    var yStart = Math.Min(brickStart[1], brickEnd[1]);
    var yEnd = Math.Max(brickStart[1], brickEnd[1]);
    var zStart = Math.Min(brickStart[2], brickEnd[2]);
    var zEnd = Math.Max(brickStart[2], brickEnd[2]);

    for (var xi = xStart; xi <= xEnd; xi++)
        for (var yi = yStart; yi <= yEnd; yi++)
            for (var zi = zStart; zi <= zEnd; zi++)
                // place above is not empty and not itself
                if (space[xi][yi][zi + 1] != 0 && space[xi][yi][zi + 1] != brickId) 
                    brickIdsToCheck.Add(space[xi][yi][zi + 1]);

    return brickIdsToCheck;
}

bool IsBrickSupportedByAnotherBrick(HashSet<int> brickIdsBot, int brickIdTop)
{
    (var topBrickStart, var topBrickEnd) = settledBricks[brickIdTop - 1];

    var xTopStart = Math.Min(topBrickStart[0], topBrickEnd[0]);
    var xTopEnd = Math.Max(topBrickStart[0], topBrickEnd[0]);
    var yTopStart = Math.Min(topBrickStart[1], topBrickEnd[1]);
    var yTopEnd = Math.Max(topBrickStart[1], topBrickEnd[1]);
    var zTopStart = Math.Min(topBrickStart[2], topBrickEnd[2]);
    var zTopEnd = Math.Max(topBrickStart[2], topBrickEnd[2]);

    for (var xi = xTopStart; xi <= xTopEnd; xi++)
        for (var yi = yTopStart; yi <= yTopEnd; yi++)
            for (var zi = zTopStart; zi <= zTopEnd; zi++)
                // place below is not empty and not the brick we are checking
                if (space[xi][yi][zi - 1] != 0
                        && !brickIdsBot.Contains(space[xi][yi][zi - 1])
                        && space[xi][yi][zi - 1] != brickIdTop) 
                    return true;
    return false;
}

var disintigratableBrickIds = new List<int>();
for(var i = 0; i < settledBricks.Count; i++)
{
    var canDisintigrate = true;
    foreach(var brickId in GetTopBricks(i + 1))
    {
        if(!IsBrickSupportedByAnotherBrick(new HashSet<int>{ i + 1 }, brickId))
        {
            canDisintigrate = false;
            break; // no need to check the other top bricks
        }
    }
    if(canDisintigrate)
        disintigratableBrickIds.Add(i + 1);
}

Console.WriteLine("Part1: " + disintigratableBrickIds.Count());

// iterating becomes too slow, represent the bricks as a node in a graph
// where each edge means it depends on the brick below it
// this makes the logic much simpler and lookups much faster

// build the graphs
var childGraph = new Dictionary<int, HashSet<int>>();
var parentGraph = new Dictionary<int, HashSet<int>>();
for (var i = 0; i < settledBricks.Count; i++)
{
    var brickId = i + 1;
    childGraph.Add(brickId, new HashSet<int>());
    foreach (var topBrickId in GetTopBricks(brickId))
    {
        childGraph[brickId].Add(topBrickId);
        if(!parentGraph.ContainsKey(topBrickId))
            parentGraph.Add(topBrickId, new HashSet<int>());
        parentGraph[topBrickId].Add(brickId);
    }
}

// check bricks
var count = 0;
for (var i = 0; i < settledBricks.Count; i++)
{
    var brickId = i + 1;

    if (disintigratableBrickIds.Contains(brickId))
        continue;

    var disintigrated = new HashSet<int> { brickId };
    var queue = new Queue<int>();
    queue.Enqueue(brickId);
    while(queue.TryDequeue(out var nextBrickId))
    {
        foreach (var child in childGraph[nextBrickId])
        {
            if (parentGraph[child].All(parent => disintigrated.Contains(parent)))
            {
                // all parents are disintigrated, so this one will disintigrate too
                disintigrated.Add(child);
                queue.Enqueue(child);
            }
        }
    }
    count += disintigrated.Count() - 1;
}

Console.WriteLine("Part2: " + count);

sw.Stop();
Console.WriteLine($"Took {sw.ElapsedMilliseconds} ms");
Console.ReadKey();