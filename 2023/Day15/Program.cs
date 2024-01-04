using System.Reflection.Metadata.Ecma335;
using System.Text;

var lines = File.ReadAllLines("Input.txt");
var inputs = lines.Single().Trim().Split(',').ToArray();

int CustomHash(string input)
    => Encoding.ASCII.GetBytes(input).Aggregate(0, (hash, ascii) => ((hash + ascii) * 17) % 256);

Console.WriteLine("Part1: " + inputs.Select(CustomHash).Sum());

var boxes = new Dictionary<int, List<(string label, int focalLength)>>();
foreach(var input in inputs)
{
    if(input.Contains('='))
    {
        var parts = input.Split('=');
        var label = parts[0];
        var focalLength = int.Parse(parts[1]);
        var hash = CustomHash(label);
        if(!boxes.TryGetValue(hash, out var lenses))
        {
            boxes[hash] = new() { (label, focalLength) };
            continue;
        }

        var replacedLens = false;
        for(var i = 0; i < lenses.Count(); i++)
        {
            if (lenses[i].label == label)
            {
                lenses[i] = (label, focalLength);
                replacedLens = true;
                break;
            }
        }
        if (!replacedLens)
            lenses.Add((label, focalLength));
    }
    else
    {
        var label = input.Split('-')[0];
        var hash = CustomHash(label);
        if (boxes.TryGetValue(hash, out var lenses))
        {
            for (var i = 0; i < lenses.Count(); i++)
            {
                if (lenses[i].label == label)
                {
                    lenses.RemoveAt(i);
                    break;
                }
            }
        }
    }
}

var focusingPower = boxes
    .SelectMany(kvp => kvp.Value.Select((lens, slot) => (kvp.Key + 1) * (slot + 1) * lens.focalLength))
    .Sum();

Console.WriteLine("Part2: " + focusingPower);
Console.ReadKey();