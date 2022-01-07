
with open('input.txt') as f:
    lines = [line.strip() for line in f.readlines()]

lines = [line.strip(".") for line in lines]

rules = {}
for line in lines:
    parts = line.split(" bags contain ")
    bag = parts[0]
    containBags = parts[1].split(", ")
    for containBag in containBags:
        parts = containBag.split(" ")
        n = parts[0]
        if n != "no":
            bagType = parts[1] + " " + parts[2]
            if bagType not in rules.keys():
                rules[bagType] = []
            rules[bagType].append(bag)

ans = 0
typesTodo = ["shiny gold"]
bagsUsed = []
while len(typesTodo) > 0:
    nTypesTodo = []
    for typeTodo in typesTodo:
        if typeTodo in rules.keys():
            options = rules[typeTodo]
            for option in options:
                if option not in bagsUsed:
                    bagsUsed.append(option)
                    ans += 1
                    nTypesTodo.append(option)
    typesTodo = nTypesTodo

print(ans)
