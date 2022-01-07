from copy import deepcopy

def getOriginalInput():
    with open('input.txt') as f:
        lines = [line.strip() for line in f.readlines()]

    lines = [(line.split(" ")[0], (int(line.split(" ")[1][1::]) if line.split(" ")[1][0] == "+" else int(line.split(" ")[1]))) for line in lines]

    return lines

def getAcc(lines):
    done = []
    acc = 0
    curr = -1
    while True:
        curr += 1
        if curr in done:
            break
        if curr >= len(lines):
            return acc, curr
        done.append(curr)
        operation, amount = lines[curr]
        if operation == "nop":
            continue
        elif operation == "acc":
            acc += amount
        elif operation == "jmp":
            curr += amount - 1

    return acc, curr

orig = getOriginalInput()
goal = len(orig)
for i in range(len(orig)):
    if orig[i][0] == "acc":
        continue
    linescopy = deepcopy(orig)
    if orig[i][0] == "nop":
        linescopy[i] = ("jmp", orig[i][1])
    else:
        linescopy[i] = ("nop", orig[i][1])

    acc, eip = getAcc(linescopy)
    if eip == goal:
        print(acc)

