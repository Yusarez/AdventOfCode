
with open('input.txt') as f:
    lines = [line.strip() for line in f.readlines()]

lines = [line.strip(".") for line in lines]

rules = {}
for line in lines:
    parts = line.split(" bags contain ")
    bag = parts[0]
    if bag not in rules.keys():
        rules[bag] = {}
    containBags = parts[1].split(", ")
    for containBag in containBags:
        parts = containBag.split(" ")
        n = parts[0]
        if n != "no":
            bagType = parts[1] + " " + parts[2]
            rules[bag][bagType] = int(n)

ans = 0
typesTodo = {"shiny gold" : 1}
while len(typesTodo.keys()) > 0:
    nTypesTodo = {}
    for typeTodo, amount in typesTodo.items():
        ans += amount
        if typeTodo in rules.keys():
            options = rules[typeTodo]
            for option, optionAmount in options.items():
                if option in nTypesTodo.keys():
                    nTypesTodo[option] += optionAmount * amount
                else:
                    nTypesTodo[option] = optionAmount * amount

    typesTodo = nTypesTodo

print(ans - 1) #remove shiny bag itself
