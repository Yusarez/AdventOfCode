
var lines = File.ReadAllLines("Input.txt");

Part2();

void Part1()
{
    var sum = 0;
    for (var currentRow = 0; currentRow < lines.Length; currentRow++)
    {
        var line = lines[currentRow];
        var numberFirstColumnIndex = -1;
        for (var currentColumn = 0; currentColumn < line.Length; currentColumn++)
        {
            var isDigit = char.IsDigit(line[currentColumn]);

            if (isDigit && numberFirstColumnIndex == -1)
                numberFirstColumnIndex = currentColumn;

            if ((!isDigit && numberFirstColumnIndex != -1)
                || (isDigit && currentColumn == line.Length - 1))
            {
                if(isDigit && currentColumn == line.Length - 1)
                    currentColumn++;
                if (CheckSurroundingsForSymbol(lines, currentRow, numberFirstColumnIndex, currentColumn))
                    sum += int.Parse(line[numberFirstColumnIndex..currentColumn]);
                numberFirstColumnIndex = -1;
            }
        }
    }
    Console.WriteLine("Part1: " + sum);
    Console.ReadKey();
}

void Part2()
{
    var sum = 0;
    var numbers = new List<List<(int column, string number)>>();
    var gears = new List<List<int>>();
    for (var currentRow = 0; currentRow < lines.Length; currentRow++)
    {
        var line = lines[currentRow];
        var numberFirstColumnIndex = -1;
        var numbersOnRow = new List<(int column, string number)>();
        var gearsOnRow = new List<int>();
        for (var currentColumn = 0; currentColumn < line.Length; currentColumn++)
        {
            if (line[currentColumn] == '*')
                gearsOnRow.Add(currentColumn);

            var isDigit = char.IsDigit(line[currentColumn]);

            if (isDigit && numberFirstColumnIndex == -1)
                numberFirstColumnIndex = currentColumn;

            if ((!isDigit && numberFirstColumnIndex != -1)
                || (isDigit && currentColumn == line.Length - 1))
            {
                if (isDigit && currentColumn == line.Length - 1)
                    currentColumn++;
                numbersOnRow.Add((numberFirstColumnIndex, line[numberFirstColumnIndex..currentColumn]));
                numberFirstColumnIndex = -1;
            }
        }
        numbers.Add(numbersOnRow);
        gears.Add(gearsOnRow);
    }
    for (var currentRow = 0; currentRow < lines.Length; currentRow++)
    {
        foreach(var gearPos in gears[currentRow])
        {
            var adjacentNumbers = new List<int>();

            // check above
            if(currentRow > 0)
            {
                foreach((var column, var number) in numbers[currentRow - 1])
                {
                    if(gearPos >= column - 1 && gearPos <= column + number.Length)
                        adjacentNumbers.Add(int.Parse(number));
                }
            }

            // check left & right
            foreach ((var column, var number) in numbers[currentRow])
            {
                // left
                if (gearPos == column + number.Length)
                    adjacentNumbers.Add(int.Parse(number));
                // right
                if (gearPos == column - 1)
                    adjacentNumbers.Add(int.Parse(number));
            }

            // check below
            if (currentRow < lines.Length - 1)
            {
                foreach ((var column, var number) in numbers[currentRow + 1])
                {
                    if (gearPos >= column - 1 && gearPos <= column + number.Length)
                        adjacentNumbers.Add(int.Parse(number));
                }
            }

            if(adjacentNumbers.Count() == 2)
                sum += adjacentNumbers[0] * adjacentNumbers[1];
        }
    }
    Console.WriteLine("Part2: " + sum);
    Console.ReadKey();
}

bool CheckSurroundingsForSymbol(string[] lines, int currentRow, int firstDigitIndex, int firstNonDigitIndex)
{
    // check above
    if(currentRow > 0)
        for (var i = firstDigitIndex - 1; i < firstNonDigitIndex + 1; i++)
            if (IsPartSymbol(lines, currentRow - 1, i))
                return true;

    //check left & right
    if (IsPartSymbol(lines, currentRow, firstDigitIndex - 1))
        return true;
    if (IsPartSymbol(lines, currentRow, firstNonDigitIndex))
        return true;

    // check below
    if (currentRow < lines.Length - 1)
        for (var i = firstDigitIndex - 1; i < firstNonDigitIndex + 1; i++)
            if (IsPartSymbol(lines, currentRow + 1, i))
                return true;

    return false;
}

bool IsPartSymbol(string[] lines, int row, int col)
{
    if(row < 0 || col < 0) return false;
    if(row >= lines.Length) return false;
    if(col >= lines[0].Length) return false;
    var ch = lines[row][col];
    return !char.IsDigit(ch) && ch != '.';
}