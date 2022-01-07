from tqdm import trange
with open('input.txt') as f:
    lines = [line.strip() for line in f.readlines()]

field = []
for line in lines:
    first = []
    for char in line:
        first.append([[1] if char == "#" else [0]])
    field.append(first)

def padField(field):
    for i, first in enumerate(field):
        for j, second in enumerate(field[i]):
            for k, third in enumerate(field[i][j]):
                field[i][j][k] = [0] + third + [0]
            field[i][j] = [[0] * len(field[0][0][0])] + second + [[0] * len(field[0][0][0])]
        field[i] = [[[0] * len(field[0][0][0])] * len(field[0][0])] + first + [[[0] * len(field[0][0][0])] * len(field[0][0])]
    field = [[[[0] * len(field[0][0][0])] * len(field[0][0])] * len(field[0])] + field + [[[[0] * len(field[0][0][0])] * len(field[0][0])] * len(field[0])]
    return field

def validCoord(field, i, j, k, l):
    if i < 0 or j < 0 or k < 0 or l < 0:
        return False
    if i >= len(field) or j >= len(field[0]) or k >= len(field[0][0]) or l >= len(field[0][0][0]):
        return False
    return True

def getActiveNeighbors(field, i, j, k, l):
    nActiveNeighbors = 0
    for di in range(-1, 2):
        for dj in range(-1, 2):
            for dk in range(-1, 2):
                for dl in range(-1, 2):
                    if di == 0 and dj == 0 and dk == 0 and dl == 0:
                        continue
                    if validCoord(field, i + di, j + dj, k + dk, l + dl):
                        nActiveNeighbors += field[i + di][j + dj][k + dk][l + dl]
    return nActiveNeighbors

for _ in trange(6):
    field = padField(field)
    nField = []
    for i in range(len(field)):
        first = []
        for j in range(len(field[0])):
            second = []
            for k in range(len(field[0][0])):
                third = []
                for l in range(len(field[0][0][0])):
                    nActiveNeighbors = getActiveNeighbors(field, i, j, k, l)
                    third.append((nActiveNeighbors == 2 and field[i][j][k][l]) or nActiveNeighbors == 3)
                second.append(third)
            first.append(second)
        nField.append(first)
    field = nField

print(sum([sum([sum([sum(third) for third in second]) for second in first]) for first in field]))

