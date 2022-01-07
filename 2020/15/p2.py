from tqdm import trange
input = [0,3,1,6,7,5]
memory = {}
for i, j in enumerate(input):
    memory[j] = i
nextVal = 0
for i in trange(len(input), 30_000_000 - 1):
    if nextVal in memory.keys():
        prev = memory[nextVal]
        memory[nextVal] = i
        nextVal = i - prev
    else:
        memory[nextVal] = i
        nextVal = 0
print(nextVal)