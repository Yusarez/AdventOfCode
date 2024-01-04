using System.Data;

var lines = File.ReadAllLines("Input.txt");

const int START_SIZE = 3;

var inputs = new List<(string dir, int dist, string color)>();
foreach (var line in lines)
{
    var parts = line.Split(' ');
    inputs.Add((parts[0], int.Parse(parts[1]), parts[2].TrimStart('(').TrimEnd(')')));
}

var map = new bool[START_SIZE][];
for (int i = 0; i < map.Length; i++)
    map[i] = new bool[START_SIZE];
map[0][0] = true;

void ShowMap()
{
    Console.Clear();
    foreach (var arr in map)
        Console.WriteLine(new string(arr.Select(b => b ? '#' : '.').ToArray()));
}

bool IsInMap(int row, int col)
    => 0 <= row && row < map.Length && 0 <= col && col < map[0].Length;

void ExpandMap(int size, bool vertical, bool beginning)
{
    if(vertical)
    {
        var newMap = new List<bool[]>();
        if(beginning)
            for (var i = 0; i < size; i++)
                newMap.Add((new bool[map.First().Count()]).ToArray());
        newMap.AddRange(map);
        if (!beginning)
            for (var i = 0; i < size; i++)
                newMap.Add((new bool[map.First().Count()]).ToArray());
        map = newMap.ToArray();
    }
    else
    {
        for(var i = 0; i < map.Length; i++)
            map[i] = (
                beginning ? 
                    (new bool[size]).Concat(map[i]) : 
                    map[i].Concat(new bool[size])
                )
                .ToArray();
    }
}

(int row, int col) currentPos = (0, 0);
foreach((var dir, var dist, var color) in inputs)
{
    switch(dir)
    {
        case "U":
            {
                if (currentPos.row - dist < 0)
                {
                    ExpandMap(dist, vertical: true, beginning: true);
                    currentPos = (currentPos.row + dist, currentPos.col);
                }
                for (var i = 0; i < dist; i++)
                    map[currentPos.row - i][currentPos.col] = true;
                currentPos = (currentPos.row - dist, currentPos.col);
                break;
            }
        case "D":
            {
                if (currentPos.row + dist >= map.Length)
                    ExpandMap(dist, vertical: true, beginning: false);
                for (var i = 0; i < dist; i++)
                    map[currentPos.row + i][currentPos.col] = true;
                currentPos = (currentPos.row + dist, currentPos.col);
                break;
            }
        case "L":
            {
                if (currentPos.col - dist < 0)
                {
                    ExpandMap(dist, vertical: false, beginning: true);
                    currentPos = (currentPos.row, currentPos.col + dist);
                }
                for (var i = 0; i < dist; i++)
                    map[currentPos.row][currentPos.col - i] = true;
                currentPos = (currentPos.row, currentPos.col - dist);
                break;
            }
        case "R":
            {
                if (currentPos.col + dist >= map[0].Length)
                    ExpandMap(dist, vertical: false, beginning: false);
                for (var i = 0; i < dist; i++)
                    map[currentPos.row][currentPos.col + i] = true;
                currentPos = (currentPos.row, currentPos.col + dist);
                break;
            }
    }
}

//flood sides
// count the false bools
var emptySpace = 0;
var alreadyVisited = new bool[map.Length][];
for (var i = 0; i < map.Length; i++)
    alreadyVisited[i] = new bool[map[0].Length];
var queue = new Queue<(int row, int col)>();

void Explore(int row, int col)
{
    if (IsInMap(row, col)
        && !alreadyVisited[row][col]
        && !map[row][col]) // is not a wall
    {
        alreadyVisited[row][col] = true;
        queue.Enqueue((row, col));
    }
}

for(var row = 0; row < map.Length; row++)
{
    Explore(row, 0);
    Explore(row, map[0].Length - 1);
}
for (var col = 0; col < map[0].Length; col++)
{
    Explore(0, col);
    Explore(map.Length - 1, col);
}

while (queue.TryDequeue(out var nextPos))
{
    var toExplore = new (int row, int col)[]
    {
        (nextPos.row + 1, nextPos.col),
        (nextPos.row - 1, nextPos.col),
        (nextPos.row, nextPos.col + 1),
        (nextPos.row, nextPos.col - 1),
    };

    foreach(var posToExplore in toExplore)
        Explore(posToExplore.row, posToExplore.col);
}

Console.WriteLine("Part1: " + (map.Length * map[0].Length - alreadyVisited.SelectMany(i=>i).Count(b=>b)));

// gonna have to be smarter about part2...
// numbers are too big to keep the map fully in memory
// also flooding the map would take way too long
// use Shoelace formula https://en.wikipedia.org/wiki/Shoelace_formula

const char RIGHT = '0';
const char DOWN = '1';
const char LEFT = '2';
const char UP = '3';

var vertices = new List<(long row, long col)>();
(long row, long col) pos = (0, 0);
foreach ((_, _, string color) in inputs)
{
    var distance = Convert.ToInt64("0x" + color[1..^1], 16);
    var dir = color[^1];
    pos = dir switch
    {
        UP => (pos.row - distance, pos.col),
        RIGHT => (pos.row, pos.col + distance),
        DOWN => (pos.row + distance, pos.col),
        LEFT => (pos.row, pos.col - distance),
    };
    vertices.Add(pos);
}

// for shoelace theorem, the vertices need to be anti clockwise in normal coordinate axis
// https://stackoverflow.com/questions/1165647
var clockwise = vertices.Select(
    (point, idx) => (vertices[(idx + 1) % vertices.Count()].row - point.row)
                    * (vertices[(idx + 1) % vertices.Count()].col - point.col)
    ).Sum() > 0;

if(clockwise)
    vertices.Reverse();

long area = 0;
for(var i = 0; i < vertices.Count; i++)
{
    var vertex = vertices[i];
    var nextVertex = vertices[(i + 1) % vertices.Count()];
    area += (vertex.row * nextVertex.col) - (nextVertex.row * vertex.col);
}

area = Math.Abs(area) / 2L; //shoelace

// we are working in discrete space so the formula doesnt apply directly
// pretend we are drawing a line in continuous space throught he boundary of the path
// then we have in a straight line half of the square patch each time
// this is also true for the inner corners but not the 4 ultimate outer corners which add the extra square
long circumference = 0;
for (var i = 0; i < vertices.Count; i++)
{
    var vertex = vertices[i];
    var nextVertex = vertices[(i + 1) % vertices.Count()];
    circumference += Math.Abs(nextVertex.row - vertex.row) + Math.Abs(nextVertex.col - vertex.col);
}

area += circumference / 2L + 1L; // +1 for starting space

Console.WriteLine("Part2: " + area);
Console.ReadKey();
