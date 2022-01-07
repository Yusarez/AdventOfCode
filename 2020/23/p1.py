
with open('input.txt') as f:
    lines = [line.strip() for line in f.readlines()]

state = [int(c) for c in lines[0]]
size = len(state)

curr = 0
for _ in range(100):
    val = state[curr] - 1
    if val == 0:
            val += size
    pickup = state[curr+1:curr+4]
    nstate = state[:curr+1] + state[curr+4:]
    if curr+4 >= size:
        nstate = nstate[(curr+4)%size:]
        pickup = pickup + state[:(curr+4)%size]
    while val not in nstate:
        val -= 1
        if val == 0:
            val += size
    destination = nstate.index(val)
    nstate = nstate[0:destination+1] + pickup + nstate[destination+1:]
    curr = (nstate.index(state[curr]) + 1) % size
    state = nstate


ans = state[state.index(1)+1:] + state[:state.index(1)]
ans = int("".join([str(c) for c in ans]))
print(ans)

