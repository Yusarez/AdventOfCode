using System.Diagnostics;

var lines = File.ReadAllLines("Input.txt");

var bids = new Dictionary<string, long>();
var types = new Dictionary<string, HandType>();

HandType CountToType(int[] counts)
{
    if (counts.SequenceEqual(new int[] { 5 }))
        return HandType.FiveOfAKind;
    else if (counts.SequenceEqual(new int[] { 4, 1 }))
        return HandType.FourOfAKind;
    else if (counts.SequenceEqual(new int[] { 3, 2 }))
        return HandType.FullHouse;
    else if (counts.SequenceEqual(new int[] { 3, 1, 1 }))
        return HandType.ThreeOfAKind;
    else if (counts.SequenceEqual(new int[] { 2, 2, 1 }))
        return HandType.TwoPair;
    else if (counts.SequenceEqual(new int[] { 2, 1, 1, 1 }))
        return HandType.OnePair;
    else if (counts.SequenceEqual(new int[] { 1, 1, 1, 1, 1 }))
        return HandType.HighCard;
    else
        Debug.Assert(false);

    return HandType.HighCard; //make compiler happy
}

long HandsToWinnings(bool IsPart2)
    => types.ToList()
            .OrderBy(kvp => kvp.Value)
            .ThenByDescending(kvp => kvp.Key, new CardComparer(IsPart2))
            .Select((kvp, rank) => (rank + 1) * bids[kvp.Key])
            .Sum();

foreach (var line in lines)
{
    var parts = line.Split(" ");
    var hand = parts[0];
    bids[hand] = long.Parse(parts[1]);

    var counts = hand
        .GroupBy(ch => ch)
        .Select(group => group.Count())
        .OrderByDescending(i => i)
        .ToArray();

    types[hand] = CountToType(counts);
}

Console.WriteLine("Part1: " + HandsToWinnings(IsPart2: false));

// recalculate hands
foreach((var hand, _) in types)
{
    // more or less same as part1, just filter out the jokers
    var nJokers = hand.Count(ch => ch == 'J');
    var counts = hand
        .Where(ch => ch != 'J')
        .GroupBy(ch => ch)
        .Select(group => group.Count())
        .OrderByDescending(i => i)
        .ToArray();

    if (counts.Count() > 0)
        counts[0] += nJokers; // just add the jokers to the already highest count
    else
        counts = new int[] { 5 }; //all jokers => 5 of a kind

    types[hand] = CountToType(counts);
}

Console.WriteLine("Part2: " + HandsToWinnings(IsPart2: true));
Console.ReadKey();

enum HandType
{
    HighCard,
    OnePair,
    TwoPair,
    ThreeOfAKind,
    FullHouse,
    FourOfAKind,
    FiveOfAKind,
};

class CardComparer : IComparer<string>
{
    public static char[] Order = { 'A', 'K', 'Q', 'J', 'T', '9', '8', '7', '6', '5', '4', '3', '2' };
    public static char[] Order2 = { 'A', 'K', 'Q', 'T', '9', '8', '7', '6', '5', '4', '3', '2', 'J' };

    public bool IsPart2 { get; init; }

    public CardComparer(bool isPart2)
    {
        IsPart2 = isPart2;
    }

    public int Compare(string first, string second)
    {
        foreach((var x, var y) in first.Zip(second))
        {
            var idx = Array.IndexOf(IsPart2 ? Order2 : Order, x);
            var idy = Array.IndexOf(IsPart2 ? Order2 : Order, y);
            Debug.Assert(idx != -1 && idy != -1);
            if (idx < idy)
                return -1;
            else if (idx > idy)
                return 1;
        }
        return 0;
    }
}