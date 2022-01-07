
with open('input.txt') as f:
    lines = [line.strip() for line in f.readlines()]

inputLine = lines[0]
state = {}
for i in range(len(inputLine) - 1):
    state[int(inputLine[i])] = int(inputLine[i+1])
state[int(inputLine[-1])] = 10
for i in range(10, 1_000_000):
    state[i] = i + 1
state[1_000_000] = int(inputLine[0])


size = len(state)

from tqdm import trange

curr = int(inputLine[0])
for _ in trange(10_000_000):
    pickup1 = state[curr]
    pickup2 = state[pickup1]
    pickup3 = state[pickup2]

    destination = curr - 1
    if destination == 0:
            destination += size
    while destination in (pickup1, pickup2, pickup3): #speedup is mainly here, lookup in dict vs array of 1M
        destination -= 1
        if destination == 0:
            destination += size

    state[curr] = state[pickup3]
    state[pickup3] = state[destination]
    state[destination] = pickup1

    curr = state[curr]


print(state[1] * state[state[1]])
