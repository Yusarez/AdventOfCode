
with open('input.txt') as f:
    lines = [line.strip() for line in f.readlines() if line.strip() != ""]

lines = [int(line) for line in lines]

for i in lines:
    for j in lines:
        if i + j == 2020 and i != j:
            print(i*j)