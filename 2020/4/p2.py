
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
        val = part.split(":")[1]
        if key not in currPass and key != "cid":
            if key == "byr" and 1920 <= int(val) and int(val) <= 2002:
                currPass.append(key)
            elif key == "iyr" and 2010 <= int(val) and int(val) <= 2020:
                currPass.append(key)
            elif key == "eyr" and 2020 <= int(val) and int(val) <= 2030:
                currPass.append(key)
            elif key == "hgt":
                n = int("".join([a for a in val if a in "1234567890"]))
                u = "".join([a for a in val if a not in "1234567890"])
                if (150 <= n and n <= 193 and u == "cm") or (59 <= n and n <= 76 and u == "in"):
                    currPass.append(key)
            elif key == "hcl" and val[0] == "#" and len(val) == 7 and val[1::].isalnum():
                currPass.append(key)
            elif key == "ecl" and val in ["amb", "blu", "brn", "gry", "grn", "hzl", "oth"]:
                currPass.append(key)
            elif key == "pid" and len(val) == 9 and val.isnumeric():
                currPass.append(key)



print(ans)

