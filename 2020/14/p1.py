with open('input.txt') as f:
    lines = [line.strip() for line in f.readlines()]
memory = {}
for line in lines:
    lineparts = line.split(" = ")
    if lineparts[0] == "mask":
        onmask = int(lineparts[1].replace("X", "0"), 2)
        offmask = int(lineparts[1].replace("X", "1"), 2)
    else:
        memadress = int(lineparts[0].replace("mem[", "").replace("]", ""))
        memory[memadress] = int(lineparts[1]) | onmask & offmask
print(sum(memory.values()))
