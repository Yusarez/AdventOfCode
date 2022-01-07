
with open('input.txt') as f:
    lines = [line.strip() for line in f.readlines()]


def parse(line):
    output = []
    i = 0
    while i < len(line):
        c = line[i]
        if c == "e" or c == "w":
            output.append(c)
            i += 1
        else:
            output.append(line[i:i+2])
            i += 2
    
    return output

flipped = []

for line in lines:
    cmds = parse(line)
    curr = [0, 0]
    for cmd in cmds:
        if cmd == "e" or cmd == "ne":
            curr[1] += 1
        elif cmd == "w" or cmd == "sw":
            curr[1] -= 1
        if "n" in cmd:
            curr[0] -= 1
        elif "s" in cmd:
            curr[0] += 1
    if curr in flipped:
        flipped.remove(curr)
    else:
        flipped.append(curr)

size = 301
field = [[0 for _ in range(size)] for __ in range(size)]
for i, j in flipped:
    field[i + size//2][j + size//2] = 1

for day in range(100):
    nfield = [[0 for _ in range(size)] for __ in range(size)]
    for i in range(1, len(field) - 1):
        for j in range(1, len(field[0]) - 1):
            blackNeighbors = 0
            blackNeighbors += field[i-1][j]
            blackNeighbors += field[i-1][j+1]
            blackNeighbors += field[i][j-1]
            blackNeighbors += field[i][j+1]
            blackNeighbors += field[i+1][j]
            blackNeighbors += field[i+1][j-1]
            if field[i][j]:
                nfield[i][j] = int(not (blackNeighbors == 0 or blackNeighbors > 2))
            else:
                nfield[i][j] = int(blackNeighbors == 2)

    field = nfield
    print(f"Day {day+1}:", sum([sum(row) for row in field]))
