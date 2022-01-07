
with open('input.txt') as f:
    lines = [line.strip() for line in f.readlines()]



def calc(parts):
    #first do parentheses
    nParts = []
    i = 0
    while i < len(parts):
        part = parts[i]
        if part == "(":
            j = i + 1
            oppositeBracketCount = 0
            while not (parts[j] == ")" and oppositeBracketCount == 0):
                if parts[j] == "(":
                    oppositeBracketCount += 1
                elif parts[j] == ")":
                    oppositeBracketCount -= 1
                j += 1
            subparts = parts[i+1:j]
            subans = calc(subparts)
            nParts.append(subans)
            i = j + 1
        else:
            nParts.append(part)
            i += 1
    parts = nParts
    #second do addition
    nParts = []
    i = 0
    while i < len(parts):
        part = parts[i]
        if part == "+":
            nParts = nParts[:-1] + [(nParts[-1] + parts[i+1])] #TODO what is nparts len 0
            i += 2
        else:
            nParts.append(part)
            i += 1
    parts = nParts
    # then do multiplication
    i = 0
    nParts = []
    while i < len(parts):
        part = parts[i]
        if part == "*":
            nParts = nParts[:-1] + [(nParts[-1] * parts[i+1])] #TODO
            i += 2
        else:
            nParts.append(part)
            i += 1
    assert len(nParts) == 1, "invalid sequence"
    return int(nParts[0])

ans = 0
for line in lines:
    input = line.replace(" ", "")
    input = [(int(c) if c.isdecimal() else c) for c in input]
    ans += calc(input)

print(ans)



