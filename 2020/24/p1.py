
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

print(len(flipped))
