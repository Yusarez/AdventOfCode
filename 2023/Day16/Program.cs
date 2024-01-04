var lines = File.ReadAllLines("Input.txt");

var rowMax = lines.Length;
var colMax = lines.First().Length;

bool IsInBounds(int row, int col)
    => 0 <= row && row < rowMax && 0 <= col && col < colMax;

(Direction dir, int row, int col) GetNextPos(Direction dir, int currentRow, int currentCol)
    => dir switch
    {
        Direction.Up => (Direction.Up, currentRow - 1, currentCol),
        Direction.Down => (Direction.Down, currentRow + 1, currentCol),
        Direction.Left => (Direction.Left, currentRow, currentCol - 1),
        Direction.Right => (Direction.Right, currentRow, currentCol + 1),
    };

int SolveForStartingPosition((Direction dir, int row, int col) startPos)
{
    var energyMap = new bool[rowMax][];
    for (var i = 0; i < rowMax; i++)
        energyMap[i] = new bool[colMax];

    var queue = new Queue<(Direction dir, int row, int col)>();
    queue.Enqueue(startPos);
    var history = new HashSet<(Direction dir, int row, int col)>();
    while (queue.TryDequeue(out var current))
    {
        history.Add(current);
        (var dir, var row, var col) = current;
        energyMap[row][col] = true;
        var nextPositions = new List<(Direction dir, int row, int col)>(2);
        switch (lines[row][col])
        {
            case '.':
                {
                    nextPositions.Add(GetNextPos(dir, row, col));
                    break;
                }
            case '|' when dir.IsHorizontal():
                {
                    nextPositions.Add(GetNextPos(Direction.Up, row, col));
                    nextPositions.Add(GetNextPos(Direction.Down, row, col));
                    break;
                }
            case '|' when dir.IsVertical():
                {
                    nextPositions.Add(GetNextPos(dir, row, col));
                    break;
                }
            case '-' when dir.IsVertical():
                {
                    nextPositions.Add(GetNextPos(Direction.Left, row, col));
                    nextPositions.Add(GetNextPos(Direction.Right, row, col));
                    break;
                }
            case '-' when dir.IsHorizontal():
                {
                    nextPositions.Add(GetNextPos(dir, row, col));
                    break;
                }
            case '/':
                {
                    nextPositions.Add(GetNextPos(dir switch
                    {
                        Direction.Up => Direction.Right,
                        Direction.Down => Direction.Left,
                        Direction.Left => Direction.Down,
                        Direction.Right => Direction.Up,
                    }, row, col));
                    break;
                }
            case '\\':
                {
                    nextPositions.Add(GetNextPos(dir switch
                    {
                        Direction.Up => Direction.Left,
                        Direction.Down => Direction.Right,
                        Direction.Left => Direction.Up,
                        Direction.Right => Direction.Down,
                    }, row, col));
                    break;
                }
        }
        foreach (var nextPos in nextPositions)
            if (IsInBounds(nextPos.row, nextPos.col) && !history.Contains(nextPos))
                queue.Enqueue(nextPos);
    }
    return energyMap.SelectMany(i => i).Count(b => b);
}

Console.WriteLine("Part1: " + SolveForStartingPosition((Direction.Right, 0, 0)));

var possibleStartingLocations = new List<(Direction dir, int row, int col)>();
for(var i = 0; i < rowMax; i++)
{
    possibleStartingLocations.Add((Direction.Right, i, 0));
    possibleStartingLocations.Add((Direction.Left, i, colMax - 1));
}
for (var i = 0; i < colMax; i++)
{
    possibleStartingLocations.Add((Direction.Down, 0, i));
    possibleStartingLocations.Add((Direction.Up, rowMax - 1, i));
}

Console.WriteLine("Part2: " + possibleStartingLocations.Select(SolveForStartingPosition).Max());
Console.ReadKey();

enum Direction
{
    Up,
    Down,
    Left,
    Right,
}

static class DirectionExt
{
    public static bool IsHorizontal(this Direction dir)
        => dir == Direction.Left || dir == Direction.Right;

    public static bool IsVertical(this Direction dir)
        => dir == Direction.Up || dir == Direction.Down;
}