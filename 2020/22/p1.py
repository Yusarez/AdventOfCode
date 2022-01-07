
with open('input.txt') as f:
    lines = [line.strip() for line in f.readlines()]

decks = [[], []]
flag = False
for line in lines[1::]:
    if line == "":
        continue
    if line.startswith("Player"):
        flag = True
        continue
    decks[flag].append(int(line))


while len(decks[0]) > 0 and len(decks[1]) > 0:
    n1 = decks[0].pop(0)
    n2 = decks[1].pop(0)
    ext = []
    if n1 > n2:
        decks[0].extend([n1, n2])
    else:
        decks[1].extend([n2, n1])

winDeck = decks[0] if len(decks[0]) > 0 else decks[1]
ans = 0
for i, val in enumerate(winDeck):
    ans += val * (len(winDeck) - i)

print(ans)
