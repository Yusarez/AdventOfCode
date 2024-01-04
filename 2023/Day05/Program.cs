using Range = Day05.Range;

var lines = File.ReadAllLines("Input.txt");

// seeds
var seeds = lines
    .First()
    .Split(':')[1]
    .Trim()
    .Split(" ")
    .Where(s => !string.IsNullOrWhiteSpace(s))
    .Select(long.Parse)
    .ToArray();

var transforms = new List<List<(long destination, long source, long length)>>();

foreach(var line in lines.Skip(2).Where(s => !string.IsNullOrWhiteSpace(s)))
{
    if (!char.IsDigit(line.FirstOrDefault()))
    {
        transforms.Add(new List<(long, long, long)>());
        continue;
    }

    var rangeInfo = line.Split(' ').Select(long.Parse).ToArray();
    transforms.Last().Add((rangeInfo[0], rangeInfo[1], rangeInfo[2]));
}

long Part1(List<long> startSeeds)
{
    foreach (var transform in transforms)
    {
        for (var j = 0; j < startSeeds.Count; j++)
        {
            foreach (var rangeInfo in transform)
            {
                if (rangeInfo.source <= startSeeds[j]
                    && startSeeds[j] < rangeInfo.source + rangeInfo.length)
                {
                    startSeeds[j] += rangeInfo.destination - rangeInfo.source;
                    break;
                }
            }
        }
    }
    return startSeeds.Min();
}

Console.WriteLine("Part1: " + Part1(new List<long>(seeds)));

// add in missing ranges (the ones that map to itself) + sort by source
for(var i = 0; i < transforms.Count;i++)
{
    var transform = transforms[i];
    transform.Sort((x, y) => x.source.CompareTo(y.source));
    var transformQueue = new Queue<(long destination, long source, long length)>(transform);
    var newRanges = new List<(long destination, long source, long length)>();
    // fill in the blanks
    var current = 0L;
    while(current < long.MaxValue)
    {
        var toAdd = long.MinValue;
        var next = transformQueue.Peek().source;
        if(current != next)
        {
            toAdd = next;
        }
        else
        {
            var dequeued = transformQueue.Dequeue();
            newRanges.Add(dequeued);
            current += dequeued.length;
            if (transformQueue.Count == 0)
                toAdd = long.MaxValue;
        }
        if(toAdd != long.MinValue)
        {
            var diff = toAdd - current;
            newRanges.Add((current, current, diff));
            current += diff;
        }
    }
    transforms[i] = newRanges;
}

for (var i = 0; i < transforms.Count; i++)
{
    var transform = transforms[i];
    transform.Sort((x, y) => x.destination.CompareTo(y.destination));
}

List<long> Part2(int level, Range range)
{
    if(level < 0) 
    {
        // check seed ranges
        var seedOptions = new List<long>();
        for(var i = 0; i < seeds.Length; i += 2)
        {
            var seedRange = Range.FromStartAndSize(seeds[i], seeds[i + 1]);
            var overlap = seedRange.OverlappingRange(range);
            if (overlap != Range.Invalid)
                seedOptions.Add(overlap.Start);
        }

        return seedOptions;
    }

    var results = new List<long>();
    foreach(var rangeInfo in transforms[level])
    {
        var overlappingRange = Range.FromStartAndSize(rangeInfo.destination, rangeInfo.length)
            .OverlappingRange(range);
        if(overlappingRange != Range.Invalid)
        {
            overlappingRange.Shift(rangeInfo.source - rangeInfo.destination);
            var result = Part2(level - 1, overlappingRange);
            results.AddRange(result);
        }
    }
    return results;
}

Console.WriteLine("Part2: " + Part1(Part2(transforms.Count - 1, Range.Positive)));
Console.ReadKey();