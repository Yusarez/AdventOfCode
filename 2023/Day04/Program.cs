var lines = File.ReadAllLines("Input.txt");

var cards = new List<(List<int>, List<int>)>();

foreach (var line in lines)
{
    var bothSetsOfNumbers = line.Split(':')[1].Split("|");
    var parsedNumbers = bothSetsOfNumbers
        .Select(set => set
            .Trim()
            .Split(" ")
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(int.Parse).ToList())
        .ToArray();

    cards.Add((parsedNumbers[0], parsedNumbers[1]));
}

var cardScores = new List<int>(cards.Count);
var cardCounts = new List<int>(new int[cards.Count])
    .Select(i => 1).ToList();

for(var i = 0; i < cards.Count;i++)
{
    var card = cards[i];
    var matchCount = card.Item1.Intersect(card.Item2).Count();
    if(matchCount == 0) continue;
    cardScores.Add((int)Math.Pow(2, matchCount - 1));

    for (var j = i + 1; j < i + 1 + matchCount; j++)
        cardCounts[j] += cardCounts[i];
}

Console.WriteLine("Part1: " + cardScores.Sum());
Console.WriteLine("Part2: " + cardCounts.Sum());

Console.ReadKey();