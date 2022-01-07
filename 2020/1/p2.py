
with open('input.txt') as f:
    lines = [line.strip() for line in f.readlines() if line.strip() != ""]

lines = [int(line) for line in lines]

for i in lines:
    for j in lines:
        for k in lines:
            if i + j + k== 2020:
                print(i*j*k)