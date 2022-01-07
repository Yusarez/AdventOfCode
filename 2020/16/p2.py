
with open('input.txt') as f:
    lines = [line.strip() for line in f.readlines()]

ranges = []
rangesNames = []
myTicket = []
nearbyTickets = []

step = 0
for line in lines:
    if line == "":
        step += 1
        continue
    if step == 0:
        parts = line.split(": ")[1].split(" or ")
        rangeI = []
        for part in parts:
            rangeI.append([int(c) for c in part.split("-")])
        ranges.append(rangeI)
        rangesNames.append(line.split(": ")[0])
    elif step == 1:
        if line.startswith("your ticket"):
            continue
        myTicket = [int(c) for c in line.split(",")]
    else:
        if line.startswith("nearby tickets:"):
            continue
        nearbyTickets.append([int(c) for c in line.split(",")])

validTickets = []
for ticket in nearbyTickets:
    for ticketVal in ticket:
        validVal = False
        for rangeI in ranges:
            for rangeMin, rangeMax in rangeI:
                if ticketVal >= rangeMin and rangeMax >= ticketVal:
                    validVal = True
        if not validVal:
            break
    else:
        validTickets.append(ticket)

def areAllValuesValidForSubRanges(values, subRanges):
    for val in values:
        for subRange in subRanges:
            if subRange[0] <= val and val <= subRange[1]:
                break
        else:
            return False
    return True

colMapping = {}
while len(colMapping) < len(myTicket):
    for i in range(len(myTicket)):
        if i in colMapping.keys():
            continue
        values = [ticket[i] for ticket in validTickets]
        options = []
        for j, rangeJ in enumerate(ranges):
            if areAllValuesValidForSubRanges(values, rangeJ):
                options.append(j)
        options = [option for option in options if option not in colMapping.values()]
        if len(options) == 1:
            colMapping[i] = options[0]
            #print("column", i, "maps to", rangesNames[options[0]])

ans = 1
for i, val in enumerate(myTicket):
    if rangesNames[colMapping[i]].startswith("departure"):
        ans *= val
print(ans)
