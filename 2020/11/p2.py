
with open('input.txt') as f:
    lines = [line.strip() for line in f.readlines()]

FLOOR = 0
EMPTY = 1
OCCUPIED = 2

seats = []
for row in lines:
    seatRow = []
    for char in row:
        if char == ".":
            seatRow.append(FLOOR)
        elif char == "L":
            seatRow.append(EMPTY)
        elif char == "#":
            seatRow.append(OCCUPIED)
        else:
            print("invalid char", char)
            exit()
    seats.append(seatRow)

def validCoords(i, j):
    if i < 0 or j < 0:
        return False
    if i >= len(seats) or j >= len(seats[0]):
        return False
    return True

def getAdjacentSeats(i, j, seats):
    nOccupied = 0
    for di in range(-1, 2):
        for dj in range(-1, 2):
            if di == 0 and dj == 0:
                continue
            ni = i + di
            nj = j + dj
            while validCoords(ni, nj):
                if seats[ni][nj] == FLOOR:
                    pass
                elif seats[ni][nj] == EMPTY:
                    break
                else:
                    nOccupied += 1
                    break
                ni += di
                nj += dj
    return nOccupied


changed = True
while changed:
    changed = False
    nSeats = []
    for i in range(len(seats)):
        nSeatsRow = []
        for j in range(len(seats[0])):
            if seats[i][j] == FLOOR:
                nSeatsRow.append(FLOOR)
            elif seats[i][j] == EMPTY:
                nOccupied = getAdjacentSeats(i, j, seats)
                if nOccupied == 0:
                    nSeatsRow.append(OCCUPIED)
                    changed = True
                else:
                    nSeatsRow.append(EMPTY)
            else: #occupied
                nOccupied = getAdjacentSeats(i, j, seats)
                if nOccupied >= 5:
                    nSeatsRow.append(EMPTY)
                    changed = True
                else:
                    nSeatsRow.append(OCCUPIED) 
        nSeats.append(nSeatsRow)
    seats = nSeats

ans = 0
for row in seats:
    for n in row:
        if n == OCCUPIED:
            ans += 1
print(ans)

