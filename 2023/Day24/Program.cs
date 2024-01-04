using Fractions;
using MathNet.Numerics.LinearAlgebra;
using System.Diagnostics;

var lines = File.ReadAllLines("Input.txt");
var sw = Stopwatch.StartNew();

var hailstones = lines
    .Select(line =>
    {
        var parts = line.Split(" @ ");
        var position = parts[0].Split(", ").Select(long.Parse).ToArray();
        var velocity = parts[1].Split(", ").Select(long.Parse).ToArray();
        return (position, velocity);
    })
    .ToArray();

var minValue = 200_000_000_000_000L;
var maxValue = 400_000_000_000_000L;

// intersection of 2 lines in 2d space:
// https://www.cuemath.com/geometry/intersection-of-two-lines/

// first convert the position + velocity to algebraic equation: y = ax + b
var hailstoneEquations = new (Fraction a, Fraction b)[hailstones.Length];
for(var i = 0; i< hailstones.Length; i++)
{
    var hailstone = hailstones[i];

    // a = dy/dx
    Debug.Assert(hailstone.velocity[0] != 0L);
    var a = new Fraction(hailstone.velocity[1], hailstone.velocity[0]);

    // b = y1 - ax1
    var b = hailstone.position[1] - a * hailstone.position[0];
    
    hailstoneEquations[i] = (a, b);
}

// check every hailstone combination when they cross (if not parallel) and if the values are in boundary
var numberOfCrossings = 0;
for(var i = 0; i < hailstoneEquations.Length; i++)
{
    for (var j = i + 1; j < hailstoneEquations.Length; j++)
    {
        var hailstone1 = hailstoneEquations[i];
        var hailstone2 = hailstoneEquations[j];

        if (hailstone1.a == hailstone2.a)
        {
            // are parallel, is it the same path or different?
            if (hailstone1.b == hailstone2.b)
                numberOfCrossings++;
            else
                continue;
        }

        // equal the equations and calculate x and y where they cross
        // y = a1x + b1 = a2x + b2
        // => a1x - a2x = b2 - b1
        // => x = (b2 - b1) / (a1 - a2)
        var crosspointX = (hailstone2.b - hailstone1.b) / (hailstone1.a - hailstone2.a);

        // check if x is in boundary
        if (crosspointX < minValue || crosspointX > maxValue)
            continue;

        // is this cross point X in the future for both hailstones?
        if (hailstones[i].velocity[0] < 0 && crosspointX > hailstones[i].position[0])
            continue; // hail was going towards negative x and crosspoint is towards positive x
        else if (hailstones[i].velocity[0] > 0 && crosspointX < hailstones[i].position[0])
            continue; // hail was going towards positive x and crosspoint is towards negative x
        if (hailstones[j].velocity[0] < 0 && crosspointX > hailstones[j].position[0])
            continue; // hail was going towards negative x and crosspoint is towards positive x
        else if (hailstones[j].velocity[0] > 0 && crosspointX < hailstones[j].position[0])
            continue; // hail was going towards positive x and crosspoint is towards negative x

        // y = a1x + b1
        var crosspointY = hailstone1.a * crosspointX + hailstone1.b;

        // check if y is in boundary
        if (crosspointY < minValue || crosspointY > maxValue)
            continue;

        // is this cross point Y in the future for both hailstones?
        if (hailstones[i].velocity[1] < 0 && crosspointY > hailstones[i].position[1])
            continue; // hail was going towards negative y and crosspoint is towards positive y
        else if (hailstones[i].velocity[1] > 0 && crosspointY < hailstones[i].position[1])
            continue; // hail was going towards positive y and crosspoint is towards negative y
        if (hailstones[j].velocity[1] < 0 && crosspointY > hailstones[j].position[1])
            continue; // hail was going towards negative y and crosspoint is towards positive y
        else if (hailstones[j].velocity[1] > 0 && crosspointY < hailstones[j].position[1])
            continue; // hail was going towards positive y and crosspoint is towards negative y

        // everything checks out
        numberOfCrossings++;
    }
}

Console.WriteLine("Part1: " + numberOfCrossings);

// for part 2, the z axis is now included and we'll have to match specific timings
// we can make a bigger linear system and solve it

// lets say the starting pos is x0 y0 z0 with velocity a0 b0 c0
// lets say we hit the first hailstone (x1 y1 z1 & a1 b1 c1) at time t1, second hailstone at t2 etc
// if we put the equations for the first collision:
// 
// x0 + t1 * a0 = x1 + t1 * a1
// y0 + t1 * b0 = y1 + t1 * b1
// z0 + t1 * c0 = z1 + t1 * c1
//
// for this linear system we have 7 unknowns ( x0 y0 z0 a0 b0 c0 & t1)
// and 3 equations which is not solvable
// a linear system is solvable if we have the same number of unknowns as we have equations
//
// add the collision for the second hailstone
// 
// x0 + t2 * a0 = x2 + t2 * a2
// y0 + t2 * b0 = y2 + t2 * b2
// z0 + t2 * c0 = z2 + t2 * c2
//
// putting these 6 equations together, we have now 8 unknowns, still too many
// however if we do the same for the third hailstone, we have 9 equations and 9 unknowns (+ t3)
// which means it is solvable
// it has become too big to solve by hand so use matrix representation
// converting these 9 equations in our 9 unknowns we get
//
// we are running into a problem, have these multiplications which dont allow us to make a normal matrix for Ax = b
// however we can manipulate the equations more to get rid of the unknown ti's
// 
// x0 + t1 * a0 = x1 + t1 * a1
// => x0 - x1 = t1 * (a1 - a0)
// => t1 = (x0 - x1) / (a1 - a0)

// we can do the same for y and z
// => t1 = (y0 - y1) / (b1 - b0)
// => t1 = (z0 - z1) / (c1 - c0)

// putting these equal gives us a new equation (take x and y)
// (x0 - x1) / (a1 - a0) = (y0 - y1) / (b1 - b0)
// => (x0 - x1) * (b1 - b0) = (y0 - y1) * (a1 - a0)
// => b1 * x0 - b0 * x0 - b1 * x1 + b0 * x1 = a1 * y0 - a0 * y0 - a1 * y1 + a0 * y1
// we have the problem factors like b0 * x0 wich are both unknown (doesnt fit nice in a matrix) so we put them to the left
// and the remainainder on the right
// => a0 * y0 - b0 * x0 = a1 * y0 - a1 * y1 + a0 * y1 - b1 * x0 + b1 * x1 - b0 * x1
//
// we can do the same for hailstone 2 with our new hailstone and again extract the left part
// then we put them equal and have only linear terms
// for hailstone 2 (x2 y2 z2 a2 b2 c2)
// => a0 * y0 - b0 * x0 = a2 * y0 - a2 * y2 + a0 * y2 - b2 * x0 + b2 * x2 - b0 * x2
//
// together
// a1 * y0 - a1 * y1 + a0 * y1 - b1 * x0 + b1 * x1 - b0 * x1 = a2 * y0 - a2 * y2 + a0 * y2 - b2 * x0 + b2 * x2 - b0 * x2
// (b2 - b1) * x0 + (a1 - a2) * y0 + (0) * z0 + (y1 - y2) * a0 + (x2 - x1) * b0 + (0) * c0 = a1 * y1 - b1 * x1 - a2 * y2 + b2 * x2
//
// this finally gives us the first row of our matrix
// [ (b2-b1) (a1-a2) (0)   (y1-y2) (x2-x1) (0)   ] 
// 
// the right side is then filled in the b matrix
//
// to find the other 2 equations we cane use x with z and y with z

// [ (b2-b1) (a1-a2) (0)     (y1-y2) (x2-x1) (0)     ] 
// [ (c2-c1) (0)     (a1-a2) (z1-z2) (0)     (x2-x1) ] 
// [ (0)     (c1-c2) (b2-b1) (0)     (z2-z1) (y1-y2) ] 

// to find the final 3 equations we do the whole thing again with hailstone 3 instead of 2

var x1 = hailstones[0].position[0];
var y1 = hailstones[0].position[1];
var z1 = hailstones[0].position[2];

var a1 = hailstones[0].velocity[0];
var b1 = hailstones[0].velocity[1];
var c1 = hailstones[0].velocity[2];

var x2 = hailstones[1].position[0];
var y2 = hailstones[1].position[1];
var z2 = hailstones[1].position[2];

var a2 = hailstones[1].velocity[0];
var b2 = hailstones[1].velocity[1];
var c2 = hailstones[1].velocity[2];

var x3 = hailstones[2].position[0];
var y3 = hailstones[2].position[1];
var z3 = hailstones[2].position[2];

var a3 = hailstones[2].velocity[0];
var b3 = hailstones[2].velocity[1];
var c3 = hailstones[2].velocity[2];

// using MathNet.Numerics to solve the linear equation 
// see https://numerics.mathdotnet.com/LinearEquations

var matrixA = Matrix<double>.Build.DenseOfArray(new double[,] 
{
    { (b2-b1), (a1-a2), (0), (y1-y2), (x2-x1), (0) },
    { (c2-c1), (0), (a1-a2), (z1-z2), (0), (x2-x1) },
    { (0), (c1-c2), (b2-b1), (0), (z2-z1), (y1-y2) },

    { (b3-b1), (a1-a3), (0), (y1-y3), (x3-x1), (0) },
    { (c3-c1), (0), (a1-a3), (z1-z3), (0), (x3-x1) },
    { (0), (c1-c3), (b3-b1), (0), (z3-z1), (y1-y3) },
});

var matrixB = Vector<double>.Build.Dense(new double[] 
{
    a1 * y1 - b1 * x1 - a2 * y2 + b2 * x2,
    a1 * z1 - c1 * x1 - a2 * z2 + c2 * x2, //take first and replace y -> z and b -> c
    c1 * y1 - b1 * z1 - c2 * y2 + b2 * z2, //take first and replace x -> z and a -> c

    // copy first 3 and replace 2 -> 3
    a1 * y1 - b1 * x1 - a3 * y3 + b3 * x3,
    a1 * z1 - c1 * x1 - a3 * z3 + c3 * x3,
    c1 * y1 - b1 * z1 - c3 * y3 + b3 * z3,
});

var matrixX = matrixA.Solve(matrixB);

Console.WriteLine("Part2: " + matrixX.Take(3).Select(d => Math.Round(d)).Sum());
sw.Stop();
Console.WriteLine($"Took {sw.ElapsedMilliseconds} ms");
Console.ReadKey();