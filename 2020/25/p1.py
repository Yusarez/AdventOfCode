
subj = 7
pub  = 19241437
pub2 = 17346587

p = 20201227
val = 1

i = 0
while val != pub:
    val *= subj
    val = val % p
    i += 1

val = 1
for _ in range(i):
    val *= pub2
    val = val % p

print(val)