
# startTime = 939
# buses = [7, 13, "x", "x", 59, "x", 31, 19]

startTime = 1000495
buses = [19,"x","x","x","x","x","x","x","x",41,"x","x","x","x","x","x","x","x","x",521,"x","x","x","x","x","x","x",23,"x","x","x","x","x","x","x","x",17,"x","x","x","x","x","x","x","x","x","x","x",29,"x",523,"x","x","x","x","x",37,"x","x","x","x","x","x",13]

minTime = startTime
minBus = 0

found = False
while not found:
    for bus in buses:
        if bus == "x":
            continue
        if minTime % bus == 0:
            found = True
            print((minTime - startTime) * bus)
    minTime += 1




