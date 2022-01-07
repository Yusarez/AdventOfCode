from math import lcm #requires python 3.9+
buses = [19,"x","x","x","x","x","x","x","x",41,"x","x","x","x","x","x","x","x","x",521,"x","x","x","x","x","x","x",23,"x","x","x","x","x","x","x","x",17,"x","x","x","x","x","x","x","x","x","x","x",29,"x",523,"x","x","x","x","x",37,"x","x","x","x","x","x",13]
currState = time = lcmi = buses[0] #lcmi = current least common multiple
for i, nextValue in enumerate(buses[1::]):
    if nextValue != "x":
        while (time + i + 1) % nextValue != 0: # +1 to counter the slice
            time += lcmi
        lcmi = lcm(lcmi, nextValue)
        currState = nextValue
print(time)