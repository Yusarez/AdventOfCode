
with open('input.txt') as f:
    lines = [line.strip() for line in f.readlines()]

rules = {}
msgs = []
flag = False
for line in lines:
    if line == "":
        flag = True
        continue
    if flag:
        msgs.append(line)
    else:
        parts = line.split(": ")
        ind = int(parts[0])
        rules[ind] = []
        for subpart in parts[1].split(" | "):
            if "\"" in subpart:
                  rules[ind].append([subpart.replace("\"", "")])  
            else:
                rules[ind].append([int(c) for c in subpart.split(" ")])

def expandRule(currRules):
    output = []
    while len(currRules) > 0:
        currRule = currRules.pop(0)
        if isinstance(currRule[0], int):
            for opt in rules[currRule[0]]:
                currRules.append(opt + currRule[1::])
        else:
            output.append(currRule)
    return output


ans = 0
for msg in msgs:
    currRules = [c for c in rules[0]]
    for char in msg:
        if currRules == [[]]:
            currRules = []
            break
        currRules = expandRule(currRules)
        currRules = [rule[1:] for rule in currRules if rule[0] == char]
    if len(currRules) > 0:
        ans +=  1


print(ans)


