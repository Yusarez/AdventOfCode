
with open('input.txt') as f:
    lines = [line.strip() for line in f.readlines()]

lines.append("")

counts = []
currList = []
for line in lines:
    if line == "":
        counts.append(len(currList))
        currList = []
        continue

    for char in line:
        if char not in currList:
            currList.append(char)

print(sum(counts))