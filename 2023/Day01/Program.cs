var lines = File.ReadAllLines("Input.txt");

Part2();

void Part1()
{
    var sum = 0;
    foreach (var line in lines)
    {
        var firstDigitChar = 'a';
        var lastDigitChar = 'a';
        foreach (var ch in line)
        {
            if (char.IsDigit(ch))
            {
                lastDigitChar = ch;
                if (firstDigitChar == 'a')
                    firstDigitChar = ch;
            }
        }
        sum += int.Parse(firstDigitChar.ToString() + lastDigitChar.ToString());
    }

    Console.WriteLine("(Part1) Sum: " + sum);
}

void Part2()
{
    var dict = new Dictionary<string, char>
    {
        ["one"] = '1',
        ["two"] = '2',
        ["three"] = '3',
        ["four"] = '4',
        ["five"] = '5',
        ["six"] = '6',
        ["seven"] = '7',
        ["eight"] = '8',
        ["nine"] = '9',
    };

    var sum = 0;
    foreach (var line in lines)
    {
        var firstDigitChar = 'a';
        var lastDigitChar = 'a';
        for (var i = 0; i < line.Length; i++)
        {
            var foundDigit = 'a';
            if (char.IsDigit(line[i]))
            {
                foundDigit = line[i];
            }
            else
            {
                foreach ((var letters, var number) in dict)
                {
                    var subline = line[i..(Math.Min(line.Length, i + letters.Length))];
                    if (string.Equals(subline, letters, StringComparison.InvariantCultureIgnoreCase))
                    {
                        foundDigit = number;
                        break;
                    }
                }
            }
            if(foundDigit != 'a')
            {
                lastDigitChar = foundDigit;
                if (firstDigitChar == 'a')
                    firstDigitChar = foundDigit;
            }
        }
        sum += int.Parse(firstDigitChar.ToString() + lastDigitChar.ToString());
    }

    Console.WriteLine("(Part2) Sum: " + sum);
}