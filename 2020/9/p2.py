
with open('input.txt') as f:
    lines = [int(line.strip()) for line in f.readlines()]

goal = 375054920

for i in range(len(lines) - 1):
    for j in range(i+1, len(lines)):
        if sum(lines[i:j+1]) == goal:
            print(min(lines[i:j+1]) + max(lines[i:j+1]))
            exit()

