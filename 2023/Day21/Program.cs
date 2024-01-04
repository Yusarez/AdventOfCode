using System.Diagnostics;

var lines = File.ReadAllLines("Input.txt");
var sw = Stopwatch.StartNew();

var rowMax = lines.Length;
var colMax = lines.First().Length;

var map = lines
    .Select(line => line.Select(ch => ch == '#' ? true : false).ToArray())
    .ToArray();

(int row, int col) start = (-1, -1);
for(var row = 0; row < rowMax; row++)
{
    for(var col = 0; col < colMax; col++)
    {
        if (lines[row][col] == 'S')
        {
            start = (row, col);
            break;
        }
    }
    if (start != (-1, -1))
        break;
}

// simple bruteforce for part 1

var nSteps = 64;

var current = new HashSet<(int row, int col)> { start };
for(var i = 0; i < nSteps; i++)
{
    var next = new HashSet<(int row, int col)>();
    foreach((var row, var col) in current)
    {
        var diffs = new (int drow, int dcol)[4]
        {
            (-1, 0),
            (1, 0),
            (0, 1),
            (0, -1),
        };
        foreach ((var drow, var dcol) in diffs)
        {
            var nextRow = row + drow;
            if (!(0 <= nextRow && nextRow < rowMax))
                continue;
            var nextCol = col + dcol;
            if (!(0 <= nextCol && nextCol < colMax))
                continue;
            if (next.Contains((nextRow, nextCol)))
                continue;
            if (map[nextRow][nextCol])
                continue; //is rock

            next.Add((nextRow, nextCol));
        }
    }
    current = next;
}


Console.WriteLine("Part1: " + current.Count());

// too big to simulate, need to do input analysis to find a pattern

// important observation:
// if we ignore all rocks for a second and think about which positions are possible to reach in n steps
// eg for n = 1
//  0
// 0.0
//  0
//
// eg for n = 2
//   0
//  0.0
// 0.0.0
//  0.0
//   0

// we notice this diamond pattern, it expands on the previous pattern for even and odd n's
// so for our n = 64 we already know the upper bound of the solution as we know all possible positions (if no rocks are present)

// there must be a pattern to this so we can interpolate to the large value 26501365
// note that the given map is 131x131 and that 26501365 = 65 + k * 131 with k = 202300
// which means we can walk in a straight line (north/east/south/west) to 131 times the map far
// also notice the input has the properties where there is this apparent diamond shape and unobstructed path in all 4 directions

bool GetAtMap(int row, int col)
{
    var inMapRow = row;
    while (inMapRow < 0) inMapRow += rowMax;
    inMapRow = inMapRow % rowMax;

    var inMapCol = col;
    while (inMapCol < 0) inMapCol += colMax;
    inMapCol = inMapCol % colMax;

    return map[inMapRow][inMapCol];
}

// copy pasted part of below, to do this properly, merge and put in shared function
// not doing it now for simplicity and to clearly see the difference between part 1 & part 2
long SolveForK(int k)
{
    var steps = 65 + k * 131;
    var current = new HashSet<(int row, int col)> { start };
    for (var i = 0; i < steps; i++)
    {
        var next = new HashSet<(int row, int col)>();
        foreach ((var row, var col) in current)
        {
            var diffs = new (int drow, int dcol)[4]
            {
                (-1, 0),
                (1, 0),
                (0, 1),
                (0, -1),
            };
            foreach ((var drow, var dcol) in diffs)
            {
                // no boundary checks!
                var nextRow = row + drow;
                var nextCol = col + dcol;
                if (next.Contains((nextRow, nextCol)))
                    continue;
                if (GetAtMap(nextRow, nextCol))
                    continue; //is rock

                next.Add((nextRow, nextCol));
            }
        }
        current = next;
    }
    return current.LongCount();
}

// after some searching we find that this represents a quadratic polynomial
// a*x*x + b*x + c
// so putting in k=0  gives us f(0) = c
// and we can derive a and b

var c = SolveForK(0);

var solution1 = SolveForK(1); // a + b + c
var solution2 = SolveForK(2); // 4a + 2b + c

// solving this equation on paper gives us
var a = (solution2 - 2 * solution1 + c) / 2;
var b = solution1 - c - a;

var x = 202300;
var part2 = a * x * x + b * x + c;
Console.WriteLine("Part2: " + part2);

sw.Stop();
Console.WriteLine($"Took {sw.ElapsedMilliseconds} ms");
Console.ReadKey();