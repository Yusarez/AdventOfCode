
with open('input.txt') as f:
    lines = [line.strip() for line in f.readlines()]

lines = [(line.split(" ")[0], (int(line.split(" ")[1][1::]) if line.split(" ")[1][0] == "+" else int(line.split(" ")[1]))) for line in lines]

done = []
acc = 0
curr = -1
while True:
    curr += 1
    if curr in done:
        break
    done.append(curr)
    operation, amount = lines[curr]
    if operation == "nop":
        continue
    elif operation == "acc":
        acc += amount
    elif operation == "jmp":
        curr += amount - 1

print(acc)