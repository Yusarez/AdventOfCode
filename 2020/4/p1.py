
with open('input.txt') as f:
    lines = [line.strip() for line in f.readlines()]

lines.append("")

ans = 0
currPass = []
for line in lines:
    if line == "":
        ans += len(currPass) == 7
        currPass = []
        continue

    for part in line.split(" "):
        key = part.split(":")[0]
        if key not in currPass and key != "cid":
            currPass.append(key)



print(ans)

