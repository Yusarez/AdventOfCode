using System.Diagnostics;

var lines = File.ReadAllLines("Input.txt");
var sw = Stopwatch.StartNew();

const string STARTNODE = "AAA";
const string FINALNODE = "ZZZ";

var instructions = lines.First();

var nodes = lines.Skip(2).Select(line =>
{
    var parts = line.Split(" = ");
    var targets = parts[1][1..^1].Split(", ");
    return new KeyValuePair<string, (string Left, string Right)>(parts[0], (targets[0], targets[1]));
}).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

var currentNode = STARTNODE;
var steps = 0;
while(currentNode != FINALNODE)
{
    currentNode = instructions[steps % instructions.Length] == 'L' ? 
        nodes[currentNode].Left : 
        nodes[currentNode].Right;
    steps++;
}

Console.WriteLine("Part1: " + steps);

// for each start node, find a cycle
var startNodes = nodes.Keys.Where(key => key.EndsWith('A')).ToArray();
var cycleTimes = new List<(long Offset, long Cycle)>(startNodes.Length);
foreach (var startNode in startNodes)
{
    var instruction = 0;
    var historyNodes = new List<(string Node, int InstructionIdx)>();
    (string Node, int InstructionIdx) current = (startNode, 0);
    while (!historyNodes.Contains(current))
    {
        var nextNode = instructions[current.InstructionIdx] == 'L' 
            ? nodes[current.Node].Left
            : nodes[current.Node].Right;

        historyNodes.Add(current);

        var nextInstructionIdx = (current.InstructionIdx + 1) % instructions.Length;
        current = (nextNode, nextInstructionIdx);
    }
    var offset = historyNodes.FindIndex(historyNode => historyNode.Node.EndsWith('Z'));
    var cycle = historyNodes.Count() - historyNodes.IndexOf(current);
    cycleTimes.Add((offset, cycle));
}

static long GCF(long a, long b)
{
    while (b != 0)
    {
        long temp = b;
        b = a % b;
        a = temp;
    }
    return a;
}

static long LCM(long a, long b)
{
    return (a / GCF(a, b)) * b;
}

static (long GCD, long s, long t) ExtendedEuclid(long a, long b)
{
    // https://en.wikipedia.org/wiki/Extended_Euclidean_algorithm
    // see pseudocode

    var (old_r, r) = (a, b);
    var (old_s, s) = (1L, 0L);
    var (old_t, t) = (0L, 1L);

    long quotient;
    while (r != 0)
    {
        quotient = old_r / r;
        (old_r, r) = (r, old_r - quotient * r);
        (old_s, s) = (s, old_s - quotient * s);
        (old_t, t) = (t, old_t - quotient * t);
    }

    return (old_r, old_s, old_t);
}

static (long combinedOffset, long combinedCycle) CombineCyclesWithOffset((long Offset, long Cycle) a, (long Offset, long Cycle) b)
{
    // see https://math.stackexchange.com/questions/2218763/how-to-find-lcm-of-two-numbers-when-one-starts-with-an-offset

    // new cycle length is just the least common multiple of both cycles
    long combinedCycle = LCM(a.Cycle, b.Cycle);

    // the offset is the first point they both meet
    // a.Offset + k * a.Cycle = b.Offset + l * b.Cycle
    // how to find k and l?

    // rewrite as 
    // k * a.Cycle - l * b.Cycle = b.Offset - a.Offset

    // use Extended Euclid Algorithm
    // to find s and t so that 
    // s * a.Cycle + t * b.Cycle = GCD(a.Cycle, b.Cycle)

    // this already looks familiar but we can rewrite it to match it
    // multiply both sides with (b.Offset - a.Offset) / (GCD(a.Cycle, b.Cycle))
    // this gets us
    // (s * a.Cycle + t * b.Cycle) * (b.Offset - a.Offset) / (GCD(a.Cycle, b.Cycle)) = b.Offset - a.Offset

    // if we compare again with the original
    // k * a.Cycle - l * b.Cycle = b.Offset - a.Offset

    // we see that
    // k = s * (b.Offset - a.Offset) / (GCD(a.Cycle, b.Cycle)
    // l = - t * (b.Offset - a.Offset) / (GCD(a.Cycle, b.Cycle)

    // the first point they meet up (the new offset) is then
    // a.Offset + k * a.Cycle
    // => a.Offset + s * a.Cycle * (b.Offset - a.Offset) / (GCD(a.Cycle, b.Cycle) 

    var (gcd, s, t) = ExtendedEuclid(a.Cycle, b.Cycle);

    // no need to check if they have a cycle at all, we know they do in this case

    var combinedOffset = a.Offset + s * a.Cycle * (b.Offset - a.Offset) / (gcd);

    // make sure combinedOffset is positive and < combinedCycle
    while (combinedOffset < 0) combinedOffset += combinedCycle;
    combinedOffset %= combinedCycle;

    return (combinedOffset, combinedCycle);
}

var globalCycleAndOffset = cycleTimes.Aggregate((a, b) => CombineCyclesWithOffset(a, b));
var nSteps = globalCycleAndOffset.Offset == 0 
    ? globalCycleAndOffset.Cycle 
    : globalCycleAndOffset.Offset;
Console.WriteLine("Part2: " + nSteps);
sw.Stop();
Console.WriteLine($"Took {sw.ElapsedMilliseconds} ms");
Console.ReadKey();