using System.Diagnostics;

var lines = File.ReadAllLines("Input.txt");

Debug.Assert(lines.Length == 2);

long[] ParseLine(string line)
    => line.Split(":")[1]
        .Trim()
        .Split(" ")
        .Where(s => !string.IsNullOrWhiteSpace(s))
        .Select(long.Parse)
        .ToArray();

var times = ParseLine(lines[0]);
var distances = ParseLine(lines[1]);

long Solve(long time, long distance)
{
    // the distance the toy gets can be described as (time - pushTime) * pushTime
    // where pushTime is the time the button is pushed
    // if we wanna know when we beat the record distance we subtract this
    // this gives us a quadratic equation
    // time * pushTime - pushTime**2 - distance = solutions (which are the min and max of the range)

    var d = Math.Sqrt(time * time - 4 * distance);
    var solution1 = (-1 * time - d) / (-2);
    var solution2 = (-1 * time + d) / (-2);

    // do the +/-1 + ceiling/floor to deal with exact matches (must win so must be higher)
    // eg if the min solution is 10 we actually need 11 seconds to win so we do floor(10 + 1) to get 11
    return (long)Math.Ceiling(solution1 - 1) - (long)Math.Floor(solution2 + 1) + 1L;
}

var result = times
    .Zip(distances)
    .Select(x => Solve(x.First, x.Second))
    .Aggregate((x, y) => x * y);

Console.WriteLine("Part1: " + result);

long ConcatLongsStringWise(long[] nums)
    => long.Parse(string.Join("", nums.Select(l => l.ToString())));

var singleTime = ConcatLongsStringWise(times);
var singleDistance = ConcatLongsStringWise(distances);

Console.WriteLine("Part2: " + Solve(singleTime, singleDistance));
Console.ReadKey();