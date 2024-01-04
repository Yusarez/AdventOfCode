using Day20;
using System.Diagnostics;

var lines = File.ReadAllLines("Input.txt");

var sw = Stopwatch.StartNew();

// parse inputs
var modules = new Dictionary<string, IModule>();
foreach(var line in lines)
{
    var parts = line.Split(" -> ");
    var destinations = parts[1].Split(", ");
    if (parts[0].StartsWith('%'))
    {
        var name = parts[0][1..];
        modules[name] = new FlipFlopModule(name, destinations);
    }
    else if (parts[0].StartsWith('&'))
    {
        var name = parts[0][1..];
        modules[name] = new ConjunctionModule(name, destinations);
    }
    else
    {
        var name = parts[0];
        modules[name] = new BroadcastModule(name, destinations);
    }
}
//for every conjunction module, find its inputs
foreach((var name, var module) in modules)
    foreach(var dest in module.Destinations)
        if (modules.TryGetValue(dest, out var destModule) && destModule is ConjunctionModule conjunction)
            conjunction.RegisterInputModule(name);

void ResetModules()
{
    foreach ((_, var module) in modules)
        module.Reset();
}

(long highPulseCount, long lowPulseCount) ProcessStartingPulse(Pulse startingPulse)
{
    var highPulseCount = 0L;
    var lowPulseCount = 0L;
    var queue = new Queue<Pulse>();
    queue.Enqueue(startingPulse);
    while (queue.TryDequeue(out var pulse))
    {
        if (pulse.IsHigh)
            highPulseCount++;
        else
            lowPulseCount++;

        if (!modules.TryGetValue(pulse.Destination, out var destModule))
            continue; // can be void destination, like 'output' in example 2

        foreach (var nextPulse in destModule.GetNextPulses(pulse))
            queue.Enqueue(nextPulse);
    }
    return (highPulseCount, lowPulseCount);
}

// part 1
int buttonPresses = 1000;
var highPulseCount = 0L;
var lowPulseCount = 0L;
for (var i = 0; i < buttonPresses; i++)
{
    var startingPulse = new Pulse
    {
        Source = "button",
        Destination = "broadcaster",
        IsHigh = false
    };
    (var highCount, var lowCount) = ProcessStartingPulse(startingPulse);
    highPulseCount += highCount;
    lowPulseCount += lowCount;
}
sw.Stop();
Console.WriteLine("Part1: " + (highPulseCount * lowPulseCount));

// part 2
// impossible to go through the sequence for part 2, it takes too long
// looking at the input enough, I notice there is a 4-way symmetry in the modules
// rx only has 1 input node which has 4 input nodes which come from an independent cycle
// can evaluate these cycles independent and then take the LCM to find the total number of presses
// the 4 cycles start from the 4 destinations of the broadcast module

// in my case it has the following (reordered)

// broadcaster -> km, xt, pk, vk
// ...
// &hn -> xn
// &mp -> xn
// &xf -> xn
// &fz -> xn
// &xn -> rx

// in order for rx to get a low pulse, the 4 inputs "hn", "mp", "xf", "fz" need to send a high pulse

// by doing some debugging, found the group
// a circle of 12 flipflops
// km, lm, bh, ms, bg, js, zb, lr, pt, jt, hx, qd
// and 2 collector conjunction nodes, mp only has jl as input and jl has some of the flipflops as input
// <<jl>>, <<mp>>
// jl has km, bh, js, pt, jt, hx, qd as input
// which means for this group the bits 1 (lm), 3 (ms), 4 (bg), 6 (zb), 7 (lr)
// since we need mp to send a high pulse we need jl to send a low pulse, which means all the flipflops inputs need to be high
// this means we can model this circle of flipflops as a 12 bit register that keeps increasing with 1 every button press
// where the conjunction sends a signal when certain bits are set and is repeated every 2**12=4096 buttons
// Note: km send the input first to jl! otherwise the answer would have been 4096...
// the destinations of jl, are the bits that are initially set

static long GCF(long a, long b)
{
    while (b != 0)
    {
        long temp = b;
        b = a % b;
        a = temp;
    }
    return a;
}

static long LCM(long a, long b)
{
    return (a / GCF(a, b)) * b;
}

// determined from looking at the input manually
var cycleNodes = new (string start, string stop)[]
{
    ("km", "jl"),
    ("xt", "jn"),
    ("pk", "gp"),
    ("vk", "fb"),
};

var cycleBits = new List<List<int>>();
foreach((var start, var end) in cycleNodes)
{
    var idx = 0;
    var current = start;
    var bits = new List<int>();
    while (current != end)
    {
        var dests = modules[current].Destinations;
        Debug.Assert(dests.Count() <= 2);

        current = dests.Count() == 1 
            ? dests.Single() 
            : dests.Single(dest => dest != end);

        if (dests.Contains(end))
            bits.Add(idx);

        idx++;
    }
    cycleBits.Add(bits);
}

// convert the bit positions to actual numbers
var bitValues = cycleBits.Select(bits => (long)bits.Select(bit => Math.Pow(2, bit)).Sum());
// take LCM of all values (pair wise)
var totalCycle = bitValues.Aggregate(1L, (prev, value) => LCM(prev, value));

Console.WriteLine("Part2: " + totalCycle);
Console.WriteLine($"Took {sw.ElapsedMilliseconds} ms");
Console.ReadKey();
