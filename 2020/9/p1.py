
with open('input.txt') as f:
    lines = [int(line.strip()) for line in f.readlines()]

preamblelen = 25


for i in range(preamblelen, len(lines)):
    opts = lines[i - preamblelen: i]
    found = False
    for opt1 in opts:
        for opt2 in opts:
            if opt1 == opt2:
                continue
            if opt1 + opt2 == lines[i]:
                found = True
                break
        if found:
            break
    else:
        print(lines[i])
        break

