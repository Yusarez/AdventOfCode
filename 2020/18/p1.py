
with open('input.txt') as f:
    lines = [line.strip() for line in f.readlines()]



def calc(parts):
    currOperation = "+"
    ans = 0
    i = 0
    while i < len(parts):
        part = parts[i]
        if part == "+":
            currOperation = part
            i += 1
        elif part == "*":
            currOperation = part
            i += 1
        elif part == "(":
            j = i + 1
            oppositeBracketCount = 0
            while not (parts[j] == ")" and oppositeBracketCount == 0):
                if parts[j] == "(":
                    oppositeBracketCount += 1
                elif parts[j] == ")":
                    oppositeBracketCount -= 1
                j += 1
            subparts = parts[i+1:j]
            subans = calc(subparts)
            if currOperation == "+":
                ans += subans
            elif currOperation == "*":
                ans *= subans
            i = j + 1
        else:
            if currOperation == "+":
                ans += int(part)
                i += 1
            elif currOperation == "*":
                ans *= int(part)
                i += 1
    return ans

ans = 0
for line in lines:
    input = line.replace("(", "( ").replace(")", " )")
    ans += calc(input.split(" "))

print(ans)



