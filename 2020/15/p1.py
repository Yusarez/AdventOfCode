
input = [0,3,1,6,7,5]
# input = [0,3,6]
# input = [3,1,2]

while len(input) < 2020:
    if input.count(input[-1]) == 1:
        input.append(0)
    else:
        lastindex = input[::-1].index(input[-1])
        secondtolastindex = input[::-1][lastindex+1::].index(input[-1]) + lastindex + 1
        input.append(secondtolastindex - lastindex)

print(input[-1])