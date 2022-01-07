
with open('input.txt') as f:
    lines = [int(line.strip()) for line in f.readlines()]

lines.append(0) #outlet
lines.append(max(lines) + 3) #my device

lines.sort()

ans = 0

curr = [[0, 1]]
while len(curr) > 0:
    jolts, count = curr.pop(0)
    for i in range(1, 4):
        nJolts = jolts + i
        if nJolts in lines: #binary search would be faster but not needed here, only 100 elements
            for curri in curr:
                if curri[0] == nJolts:
                    curri[1] += count
                    break
            else:
                curr.append([nJolts, count])


print(count)