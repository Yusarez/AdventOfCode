
with open('input.txt') as f:
    lines = [line.strip() for line in f.readlines()]


currPos = [0, 0]
currWaypointPos = [-1, 10]

for line in lines:
    action = line[0]
    n = int(line[1::])
    if action == "N":
        currWaypointPos[0] -= n
    elif action == "S":
        currWaypointPos[0] += n
    elif action == "E":
        currWaypointPos[1] += n
    elif action == "W":
        currWaypointPos[1] -= n
    elif action == "L":
        for _ in range((n//90)):
            currWaypointPos = [-1 * currWaypointPos[1], currWaypointPos[0]]
    elif action == "R":
        for _ in range((n//90)):
            currWaypointPos = [currWaypointPos[1], -1 * currWaypointPos[0]]
    elif action == "F":
        currPos[0] += n * currWaypointPos[0]
        currPos[1] += n * currWaypointPos[1]
    else:
        print("invalid action")
        exit()

print(abs(currPos[0]) + abs(currPos[1]))