
with open('input.txt') as f:
    lines = [line.strip() for line in f.readlines()]


currDeg = 0
currPos = [0, 0]

for line in lines:
    action = line[0]
    n = int(line[1::])
    if action == "N":
        currPos[0] -= n
    elif action == "S":
        currPos[0] += n
    elif action == "E":
        currPos[1] += n
    elif action == "W":
        currPos[1] -= n
    elif action == "L":
        currDeg += n
    elif action == "R":
        currDeg -= n
    elif action == "F":
        while currDeg < 0:
            currDeg += 360
        while currDeg >= 360:
            currDeg -= 360
        if currDeg == 0:
            currPos[1] += n
        elif currDeg == 90:
            currPos[0] -= n
        elif currDeg == 180:
            currPos[1] -= n
        elif currDeg == 270:
            currPos[0] += n
        else:
            print("unsuported deg:", currDeg)
    else:
        print("invalid action")
        exit()

print(abs(currPos[0]) + abs(currPos[1]))