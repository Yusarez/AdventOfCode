
with open('input.txt') as f:
    lines = [line.strip() for line in f.readlines()]

ingredients = []
allergens = []
allergenSet = set()
for line in lines:
    parts = line.split(" (")
    ingredients.append(parts[0].split(" "))
    allergens.append(parts[1].replace("contains ", "").replace(")", "").split(", "))
for a in allergens:
    allergenSet.update(a)

while len(allergenSet) > 0:
    allergensToRemove = []
    for allergen in allergenSet:
        relevantIngredients = []
        for i in range(len(allergens)):
            if allergen in allergens[i]:
                relevantIngredients.append(ingredients[i])

        commonElements = set(relevantIngredients[0])
        for ing in relevantIngredients[1::]:
            commonElements.intersection_update(ing)
        
        if len(commonElements) == 1:
            mapEl = list(commonElements)[0]
            allergensToRemove.append(allergen)
            for ingredient in ingredients:
                if mapEl in ingredient:
                    ingredient.remove(mapEl)
    allergenSet.difference_update(allergensToRemove)


print(sum([len(ingredient) for ingredient in ingredients]))
