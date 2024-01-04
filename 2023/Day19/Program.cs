using System.Diagnostics;

var sw = Stopwatch.StartNew();
var lines = File.ReadAllLines("Input.txt");

const string ACCEPTED = "A";
const string REJECTED = "R";
const string START = "in";

// parse input
var workflows = new Dictionary<string, Workflow>();
var parts = new List<int[]>();
foreach(var line in lines)
{
    if (string.IsNullOrEmpty(line))
        continue;

    if (line[0] == '{')
    {
        var values = line[1..^1].Split(',').Select(eval => int.Parse(eval.Split("=")[1])).ToArray();
        Debug.Assert(values.Length == 4);
        parts.Add(values);
    }
    else
    {
        var split = line.Split('{');
        var name = split[0];
        var workflow = new Workflow
        {
            Name = name,
        };
        foreach(var unparsedRule in split[1].TrimEnd('}').Split(','))
        {
            var ruleSplit = unparsedRule.Split(":");
            if (ruleSplit.Length == 1)
            {
                // unconditional rule
                workflow.Rules.Add(new WorkflowRule { Destination = ruleSplit[0] });
                continue;
            }
            var destination = ruleSplit[1];
            var greater = ruleSplit[0].Contains('>');
            string[]? ruleParts;
            if (greater)
                ruleParts = ruleSplit[0].Split('>');
            else
                ruleParts = ruleSplit[0].Split('<');

            workflow.Rules.Add(new WorkflowRule
            {
                Greater = greater,
                idx = "xmas".IndexOf(ruleParts[0]),
                Value = int.Parse(ruleParts[1]),
                Destination = destination,
            });
        }
        workflows.Add(name, workflow);
    }
}

// Part 1
var totalRating = 0;
foreach(var part in parts)
{
    var destination = START;
    while(destination != ACCEPTED && destination != REJECTED)
        destination = workflows[destination].GetDestination(part);
    if(destination == ACCEPTED)
        totalRating += part.Sum();
}

Console.WriteLine("Part1: " + totalRating);

// Part 2
var queue = new Queue<(string workflow, Range[])>();
queue.Enqueue((START, new Range[4]
{
    1..4001,
    1..4001,
    1..4001,
    1..4001,
}));
var combinations = 0L;
while (queue.TryDequeue(out var next))
{
    (var destination, Range[] xmas) = next;
    var newRanges = workflows[destination].GetRanges(xmas);
    foreach(var newRange in newRanges)
    {
        if (newRange.Destination == ACCEPTED)
            combinations += newRange.ranges.Aggregate(1L, (counter, range) => counter * (range.End.Value - range.Start.Value));
        else if (newRange.Destination != REJECTED)
            queue.Enqueue(newRange);
    }
}

Console.WriteLine("Part2: " + combinations);
sw.Stop();
Console.WriteLine("Finished in " + sw.ElapsedMilliseconds + " ms");
Console.ReadKey();

// Helper classes
class WorkflowRule
{
    public int Value = -1;
    public bool Greater;
    public string Destination = string.Empty;
    public int idx = -1;

    public string ApplySingle(int[] xmas)
    {
        if (Value == -1)
            return Destination;

        Func<int, int, bool> apply;
        if (Greater)
            apply = (a, b) => a > b;
        else
            apply = (a, b) => a < b;

        return apply(xmas[idx], Value) ? Destination : string.Empty;
    }

    public (string destination, Range[] ranges)? ApplyRange(Range[] xmas, bool success)
    {
        if (Value == -1)
            if (success)
                return (Destination, xmas);
            else
                return null;


        var newXmas = new Range[4];
        Array.Copy(xmas, newXmas, 4);
        if (!(Greater ^ success))
        {
            if (Value < newXmas[idx].End.Value)
                newXmas[idx] = (Value + (success ? 1 : 0))..newXmas[idx].End;
            else
                return null;
        }
        else
        {
            if (newXmas[idx].Start.Value < Value)
                newXmas[idx] = newXmas[idx].Start.Value..(Value + (success ? 0 : 1));
            else
                return null;
        }
        return (Destination, newXmas);
    }
}

class Workflow
{
    public string Name;
    public List<WorkflowRule> Rules = new();

    public string GetDestination(int[] xmas)
    {
        foreach(var rule in Rules)
        {
            var dest = rule.ApplySingle(xmas);
            if (!string.IsNullOrEmpty(dest))
                return dest;
        }
        return string.Empty;
    }

    public List<(string Destination, Range[] ranges)> GetRanges(Range[] xmas)
    {
        var newRanges = new List<(string Destination, Range[])>();
        (string Destination, Range[] ranges)? currentXmas = (string.Empty, xmas);
        foreach(var rule in Rules)
        {
            if (currentXmas == null)
                break;
            var success = rule.ApplyRange(currentXmas.Value.ranges, success: true);
            if (success.HasValue)
                newRanges.Add(success.Value);
            currentXmas = rule.ApplyRange(currentXmas.Value.ranges, success: false);
        }
        return newRanges;
    }
}