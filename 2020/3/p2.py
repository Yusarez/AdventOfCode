
with open('input.txt') as f:
    lines = [line.strip() for line in f.readlines() if line.strip() != ""]

ans = []

for dx, dy in [(1, 1), (3, 1), (5, 1), (7, 1), (1, 2)]:
    currPosX = 0
    currPosY = 0
    ansi = 0
    while currPosY < len(lines):
        if lines[currPosY][currPosX] == "#":
            ansi += 1
        currPosY += dy
        currPosX = (currPosX + dx) % len(lines[0])
    ans.append(ansi)

prod = 1
for an in ans:
    prod *= an

print(prod)