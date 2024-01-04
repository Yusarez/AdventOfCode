var lines = File.ReadAllLines("Input.txt");

var sequences = lines.Select(line => line.Split(" ").Select(long.Parse).ToList()).ToList();

List<long> UpdateSequence(List<long> sequence)
    => sequence.Select((val, pos) => pos == sequence.Count - 1 ? 0L : sequence[pos + 1] - val)
        .SkipLast(1)
        .ToList();

var sum = 0L;

foreach (var sequence in sequences)
{
    var historyOfLastNumber = new List<long>();
    var current = sequence;
    while(!current.All(l => l == 0L))
    {
        historyOfLastNumber.Add(current.Last());
        current = UpdateSequence(current);
    }
    sum += historyOfLastNumber.Sum();
}

Console.WriteLine("Part1: " + sum);

sum = 0L;

foreach (var sequence in sequences)
{
    var historyOfFirstNumber = new List<long>();
    var current = sequence;
    while (!current.All(l => l == 0L))
    {
        historyOfFirstNumber.Add(current.First());
        current = UpdateSequence(current);
    }
    sum += historyOfFirstNumber.Where((_, pos) => pos % 2 == 0).Sum()
        - historyOfFirstNumber.Where((_, pos) => pos % 2 != 0).Sum();
}

Console.WriteLine("Part2: " + sum);
Console.ReadKey();