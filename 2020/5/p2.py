
with open('input.txt') as f:
    lines = [line.strip() for line in f.readlines()]

ids = []

for line in lines:
    row = int("".join([("1" if c == "B" else "0") for c in line[:7]]), 2)
    col = int("".join([("1"  if c == "R" else "0") for c in line[7:]]), 2)
    ids.append(row * 8 + col)

ids.sort()

for id in range(min(ids), max(ids) + 1):
    if id - 1 in ids and id + 1  in ids and id not in ids:
        print(id)
