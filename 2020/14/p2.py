with open('input.txt') as f:
    lines = [line.strip() for line in f.readlines()]
memory = {}
for line in lines:
    lineparts = line.split(" = ")
    if lineparts[0] == "mask":
        mask = lineparts[1]
    else:
        memadress = int(lineparts[0].replace("mem[", "").replace("]", ""))
        n = int(lineparts[1])
        memadress |= int(mask.replace("X", "0"), 2)
        memadressBin = bin(memadress)[2::]
        memadressBin = "0" * (36 - len(memadressBin)) + memadressBin
        options = [""]
        for i, char in enumerate(mask):
            nOptions = []
            if char == "1" or char == "0":
                for option in options:
                    nOptions.append(option + memadressBin[i])
            else:
                for option in options:
                    nOptions.append(option + "0")
                    nOptions.append(option + "1")
            options = nOptions
        for option in options:
            memory[int(option, 2)] = n

print(sum(memory.values()))
