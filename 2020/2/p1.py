
with open('input.txt') as f:
    lines = [line.strip() for line in f.readlines() if line.strip() != ""]

rules = []
for line in lines:
    parts = line.split(" ")
    minmax = parts[0].split("-")
    rules.append([int(minmax[0]), int(minmax[1]), parts[1][0], parts[2]])

ans = 0
for minc, maxc, char, password in rules:
    count = password.count(char)
    if minc <= count and count <= maxc:
        ans += 1

print(ans)
