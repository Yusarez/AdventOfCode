from copy import deepcopy

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


def playGame(decks):
    prevStates = []
    while len(decks[0]) > 0 and len(decks[1]) > 0:
        n1 = decks[0].pop(0)
        n2 = decks[1].pop(0)

        if decks in prevStates:
            winner = 0
            break
        else:
            prevStates.append(deepcopy(decks))

            if len(decks[0]) >= n1 and len(decks[1]) >= n2:
                nDeck = deepcopy(decks)
                nDeck[0] = nDeck[0][:n1]
                nDeck[1] = nDeck[1][:n2]
                winner, _ = playGame(nDeck) #playsubgame
            else:
                winner =  n1 < n2

        if not winner:
            decks[0].extend([n1, n2])
        else:
            decks[1].extend([n2, n1])

    return winner, decks


winner, decks = playGame(decks)

winDeck = decks[0] if len(decks[0]) > 0 else decks[1]
ans = 0
for i, val in enumerate(winDeck):
    ans += val * (len(winDeck) - i)

print(ans)
