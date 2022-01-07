
with open('input.txt') as f:
    lines = [line.strip() for line in f.readlines()]

class Tile:
    def __init__(self, tileId, tileData):
        self.tileId = tileId
        self.tileData = tileData
    
    def __repr__(self): # for easier debugging in vs code
        return "Tile " + str(self.tileId)
    
    def matchesTop(self, topTile):
        for ind in range(len(self.tileData[0])):
            if self.tileData[0][ind] != topTile.tileData[-1][ind]:
                return False
        return True

    def matchesLeft(self, leftTile):
        for ind in range(len(self.tileData)):
            if self.tileData[ind][0] != leftTile.tileData[ind][-1]:
                return False
        return True
    
    def generateOptions(self):
        options = []
        #0
        opt = []
        for i in range(len(self.tileData)):
            row = []
            for j in range(len(self.tileData[0])):
                row.append(self.tileData[i][j])
            opt.append(row)
        options.append(opt)
        for _ in range(3):
            options.append(list(zip(*options[-1][::-1])))
        opt = []
        for i in range(len(self.tileData)):
            row = []
            for j in range(len(self.tileData[0])):
                row.append(self.tileData[i][j])
            opt.append(row[::-1])
        options.append(opt)
        for _ in range(3):
            options.append(list(zip(*options[-1][::-1])))
        output = []
        for opt in options:
            output.append(Tile(self.tileId, opt))
        return output

lines.append("")
tiles = []
currTileData = []
currTileId = 0
for line in lines:
    if line == "":
        tiles.append(Tile(currTileId, currTileData))
        currTileData = []
    elif line.startswith("Tile"):
        currTileId = int(line.split(" ")[1][:-1])
    else:
        currTileData.append([(1 if c == "#" else 0) for c in line])

i = 1
while i * i != len(tiles):
    i += 1
size = i

from copy import deepcopy
def solve(tiles, completeImage = [], pos = (0, 0)):
    if len(tiles) == 0:
        return True, completeImage
    nextPos = (pos[0], pos[1] + 1)
    if nextPos[1] == size:
        nextPos = (nextPos[0] + 1, 0)
    for tile in tiles:
        for opt in tile.generateOptions():
            finalImage = deepcopy(completeImage)
            if pos == (0, 0):
                finalImage.append([opt])
            elif pos[0] == 0:
                if not opt.matchesLeft(finalImage[0][-1]):
                    continue
                finalImage[0].append(opt)
            elif pos[1] == 0:
                if not opt.matchesTop(finalImage[pos[0]-1][0]):
                    continue
                finalImage.append([opt])
            else:
                if not opt.matchesLeft(finalImage[pos[0]][-1]) or not opt.matchesTop(finalImage[pos[0]-1][pos[1]]):
                    continue
                finalImage[pos[0]].append(opt)

            nTiles = [nTile for nTile in tiles if nTile.tileId != tile.tileId]
            solved, finalImage = solve(nTiles, finalImage, nextPos)
            if solved:
                return True, finalImage
    return False, completeImage


_, image = solve(tiles)

print(image[0][0].tileId * image[0][-1].tileId * image[-1][0].tileId * image[-1][-1].tileId)




