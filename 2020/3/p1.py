
with open('input.txt') as f:
    lines = [line.strip() for line in f.readlines() if line.strip() != ""]

dy = 1
dx = 3

currPosX = 0
currPosY = 0

ans = 0
while currPosY < len(lines):
    if lines[currPosY][currPosX] == "#":
        ans += 1
    currPosY += dy
    currPosX = (currPosX + dx) % len(lines[0])

print(ans)
