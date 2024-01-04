var lines = File.ReadAllLines("Input.txt");

var input = lines.Select(line =>
{
    var parts = line.Split(" ");
    return (parts[0], parts[1].Split(",").Select(int.Parse).ToArray());
}).ToList();

int Part1_BruteForce(string pattern, int[] counts)
{
    var arrangements = 0;
    var queue = new Queue<char[]>();
    queue.Enqueue(pattern.ToCharArray());
    while (queue.TryDequeue(out var nextPattern))
    {
        if (nextPattern.Contains('?'))
        {
            // not fully determined, expand into options en requeue
            for (var i = 0; i < nextPattern.Length; i++)
            {
                if (nextPattern[i] == '?')
                {
                    var newPattern1 = new char[nextPattern.Length];
                    var newPattern2 = new char[nextPattern.Length];
                    Array.Copy(nextPattern, newPattern1, nextPattern.Length);
                    Array.Copy(nextPattern, newPattern2, nextPattern.Length);
                    newPattern1[i] = '.';
                    newPattern2[i] = '#';
                    queue.Enqueue(newPattern1);
                    queue.Enqueue(newPattern2);
                    break;
                }
            }
            continue;
        }

        // fully determined, does it satisfy the counts?
        if (string.Join("", nextPattern)
            .Split(".")
            .Select(substr => substr.Length)
            .Where(c => c != 0)
            .SequenceEqual(counts))
            arrangements++;
    }
    return arrangements;
}

var arrangements = 0L;
foreach ((var pattern, var counts) in input)
    arrangements += Part1_BruteForce(pattern, counts);

Console.WriteLine("Part1: " + arrangements);

var duplication = 5;
var expandedInput = input
    .Select(entry =>
    {
        var expandedPattern = entry.Item1;
        var expandedCounts = entry.Item2.ToList();
        for(var i = 0; i < duplication - 1; i++)
        {
            expandedPattern += "?" + entry.Item1;
            expandedCounts.AddRange(entry.Item2);
        }
        return (expandedPattern, expandedCounts.ToArray());
    })
    .ToList();

long Part2_LinearTime(string patternStr, int[] counts)
{
    // for every spring, go over the whole input and count how many possible positions it can contain
    // in the positions before this one
    // eg
    // .??..??...?##. 1,1,3
    // 001222344440000 (first spring of length 1)
    // then do the same for all springs combining the inputs
    // eg lets say we are calculating pos 7
    // .??..??
    // 0012223
    // 00000024 (<-- the 2 is calculated because the first spring can have 2 possible positions and the next spring only 1
    //               the 4 is then the combination, 2 previous options + 2 new positions = 4 total positions
    // keep repeating this for every spring
    // the memory is 1 length greater than the pattern length to make boundary calculations easier

    var pattern = patternStr.ToCharArray();
    var memoization = new long[pattern.Length + 1];
    var prevMemoization = new long[pattern.Length + 1];
    Array.Fill(prevMemoization, 1);
    for(var countIdx = 0; countIdx < counts.Count(); countIdx++)
    {
        var count = counts[countIdx];
        Array.Fill(memoization, 0);
        for (var i = 1; i < memoization.Length; i++)
        {
            if ((i < pattern.Length
                && pattern[i] != '#')
                || i == pattern.Length)
                memoization[i] = memoization[i - 1];

            if (i < count)
                continue;

            // if first spring, all left must be empty
            if (countIdx == 0
                && !pattern[..(i - count)].All(ch => ch == '.' || ch == '?'))
                continue;

            // the previous count+1 spaces must fit ".#######" (or however long it is)

            // first check the space in between (must be . or ?)
            if (i - count - 1 >= 0 //in case we are in front and dont need the space
                && pattern[i - count - 1] == '#')
                continue;

            // then check if we can fit the spring of count long (all # or ?)
            if (!pattern[(i - count)..i].All(ch => ch == '#' || ch == '?'))
                continue;

            // then check if the next space isnt also spring (except for last place)
            if (i < pattern.Length
                && pattern[i] == '#')
                continue;

            // if we are at the last numbers, the remaining chars cannot contain a #
            if (countIdx == counts.Length - 1
                && pattern[i..].Contains('#'))
                continue;

            if (i - 1 - count >= 0)
                memoization[i] += prevMemoization[i - 1 - count];
            else if (countIdx == 0) // we are at the front but are the first spring
                memoization[i]++;
        }
        Array.Copy(memoization, prevMemoization, memoization.Length);
    }
    return memoization[^1];
}

arrangements = 0;
foreach ((var pattern, var counts) in expandedInput)
    arrangements += Part2_LinearTime(pattern, counts);

Console.WriteLine("Part2: " + arrangements);
Console.ReadKey();