
with open('input.txt') as f:
    lines = [int(line.strip()) for line in f.readlines()]

lines.append(0) #outlet
lines.append(max(lines) + 3) #my device

lines.sort()

onediffs = 0
threediffs = 0

for i in range(len(lines) - 1):
    diff = lines[i+1] - lines[i]
    if diff == 3:
        threediffs += 1
    elif diff == 1:
        onediffs += 1
    else:
        print("inavlid diff", diff)

print(onediffs * threediffs)