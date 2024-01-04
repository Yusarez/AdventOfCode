using System.Diagnostics;

var lines = File.ReadAllLines("Input.txt");

//find the starting position
(int row, int col) startingPos = (-1, -1);
for(var i = 0; i < lines.Length; i++)
{
    for(var j = 0; j < lines.First().Length; j++)
    {
        if (lines[i][j] == 'S')
        {
            startingPos = (i, j);
            break;
        }
    }
    if (startingPos.Item1 != -1) break;
}

var possibleTopPieces = new char[] { '|', 'F', '7' };
var possibleBotPieces = new char[] { '|', 'L', 'J' };
var possibleLeftPieces = new char[] { '-', 'F', 'L' };
var possibleRightPieces = new char[] { '-', '7', 'J' };

(int row, int col) GetNextPos((int row, int col) prev, (int row, int col) current, bool isStart = false)
{
    // Top
    if (current.row > 0
        && prev != (current.row - 1, current.col)
        && (isStart || possibleBotPieces.Contains(lines[current.row][current.col]))
        && possibleTopPieces.Contains(lines[current.row - 1][current.col]))
    {
        return (current.row - 1, current.col);
    }

    // Bot
    if (current.row < lines.Length - 1
        && prev != (current.row + 1, current.col)
        && (isStart || possibleTopPieces.Contains(lines[current.row][current.col]))
        && possibleBotPieces.Contains(lines[current.row + 1][current.col]))
    {
        return (current.row + 1, current.col);
    }

    // Left
    if (current.col > 0
        && prev != (current.row, current.col - 1)
        && (isStart || possibleRightPieces.Contains(lines[current.row][current.col]))
        && possibleLeftPieces.Contains(lines[current.row][current.col - 1]))
    {
        return (current.row, current.col - 1);
    }

    // Right
    if (current.col < lines.First().Length - 1
        && prev != (current.row, current.col + 1)
        && (isStart || possibleLeftPieces.Contains(lines[current.row][current.col]))
        && possibleRightPieces.Contains(lines[current.row][current.col + 1]))
    {
        return (current.row, current.col + 1);
    }

    // invalid
    Debug.Assert(false);
    return (-1, -1); // make compiler happy
}

//go down the loop in both directions at the same time
var branching1 = GetNextPos((-1, -1), startingPos, isStart: true);
var branching2 = GetNextPos(branching1, startingPos, isStart: true);
var prevBranching1 = startingPos;
var prevBranching2 = startingPos;
var currentDistance = 1;

var loop = new HashSet<(int row, int col)> { startingPos, branching1, branching2 };

while(branching1 != branching2 //same pos
    || (branching1.row == branching2.row && Math.Abs(branching2.col - branching1.col) == 1) // next to each other
    || (branching1.col == branching2.col && Math.Abs(branching2.row - branching1.row) == 1)) // next to each other
{
    currentDistance++;

    var nextBranching1 = GetNextPos(prevBranching1, branching1);
    prevBranching1 = branching1;
    branching1 = nextBranching1;

    var nextBranching2 = GetNextPos(prevBranching2, branching2);
    prevBranching2 = branching2;
    branching2 = nextBranching2;

    loop.Add(branching1);
    loop.Add(branching2);
}

Console.WriteLine("Part1: " + currentDistance);

// add extra space between all tiles and around the field
var extendedLines = new List<List<char>>();
for (var i = 0; i < 2 * lines.Length + 1; i++)
    extendedLines.Add(new string('.', 2 * lines.First().Length + 1).ToCharArray().ToList());

// fill in the original loop
foreach((var i, var j) in loop)
    extendedLines[2 * i + 1][2 * j + 1] = lines[i][j];

// fill in the gaps to connect the loop
void FillInGap(int i, int j)
{
    if ((possibleLeftPieces.Contains(extendedLines[i][j - 1]) || extendedLines[i][j - 1] == 'S')
        && (possibleRightPieces.Contains(extendedLines[i][j + 1]) || extendedLines[i][j + 1] == 'S'))
        extendedLines[i][j] = '-';
    if ((possibleTopPieces.Contains(extendedLines[i - 1][j]) || extendedLines[i - 1][j] == 'S')
        && (possibleBotPieces.Contains(extendedLines[i + 1][j]) || extendedLines[i - 1][j] == 'S'))
        extendedLines[i][j] = '|';
}

// horizontal
for (var i = 1; i < 2 * lines.Length + 1; i += 2)
    for (var j = 2; j < 2 * lines.First().Length + 1; j += 2)
        FillInGap(i, j);

// vertical
for (var i = 2; i < 2 * lines.Length + 1; i += 2)
    for (var j = 1; j < 2 * lines.First().Length + 1; j += 2)
        FillInGap(i, j);

// start flooding the outside with zeros
var floodQueue = new Queue<(int row, int col)>();
for (var i = 0; i < 2 * lines.Length + 1; i++)
{
    floodQueue.Enqueue((i, 0));
    floodQueue.Enqueue((i, 2 * lines.First().Length));
}

for (var i = 0; i < 2 * lines.First().Length + 1; i++)
{
    floodQueue.Enqueue((0, i));
    floodQueue.Enqueue((2 * lines.Length, i));
}

while(floodQueue.TryDequeue(out var next))
{
    (var i, var j) = next;
    if (extendedLines[i][j] == '0') continue; // already flooded
    extendedLines[i][j] = '0'; // flood tile

    // queue neighbours if unexplored
    if (i > 0 && extendedLines[i - 1][j] == '.') floodQueue.Enqueue((i - 1, j));
    if (j > 0 && extendedLines[i][j - 1] == '.') floodQueue.Enqueue((i, j - 1));
    if (i < extendedLines.Count - 1 && extendedLines[i + 1][j] == '.') floodQueue.Enqueue((i + 1, j));
    if (j < extendedLines.First().Count - 1 && extendedLines[i][j + 1] == '.') floodQueue.Enqueue((i, j + 1));
}

if(false)
{
    //print visual when debugging small examples
    foreach (var line in extendedLines.Where((_, pos) => pos % 2 == 1))
        Console.WriteLine(string.Join("", line.Where((_, pos) => pos % 2 == 1)));
}

// count the not flooded tiles that are part of the original tiles
var enclosedCount = 0;
for(var i = 1; i < extendedLines.Count; i += 2)
    for(var j = 1; j < extendedLines.First().Count; j += 2)
        if (extendedLines[i][j] == '.')
            enclosedCount++;

Console.WriteLine("Part2: " + enclosedCount);
Console.ReadKey();