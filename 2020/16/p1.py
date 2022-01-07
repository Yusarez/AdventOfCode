
with open('input.txt') as f:
    lines = [line.strip() for line in f.readlines()]


ranges = []
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
    elif step == 1:
        if line.startswith("your ticket"):
            continue
        myTicket = [int(c) for c in line.split(",")]
    else:
        if line.startswith("nearby tickets:"):
            continue
        nearbyTickets.append([int(c) for c in line.split(",")])

errorRate = 0
for ticket in nearbyTickets:
    for ticketVal in ticket:
        validVal = False
        for rangeI in ranges:
            for rangeMin, rangeMax in rangeI:
                if ticketVal >= rangeMin and rangeMax >= ticketVal:
                    validVal = True
        if not validVal:
            errorRate += ticketVal
            break

print(errorRate)
