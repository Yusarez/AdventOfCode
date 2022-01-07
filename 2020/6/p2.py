
with open('input.txt') as f:
    lines = [line.strip() for line in f.readlines()]

lines.append("")

counts = []
currList = []
flag = True
for line in lines:
    if line == "":
        if flag:
            counts.append(len(currList))
        else:
            counts.append(0)
        currList = []
        flag = True
        continue

    if currList == []:
        for char in line:
            currList.append(char)
    else:
        charsToRemove = []
        for char in currList:
            if char not in line:
                charsToRemove.append(char)
        for char in charsToRemove:
            currList.remove(char)
        if currList == []:
            flag = False

print(sum(counts))