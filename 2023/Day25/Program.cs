using System.Diagnostics;

var lines = File.ReadAllLines("Input.txt");
var sw = Stopwatch.StartNew();

var graph = new Dictionary<string, Dictionary<string, int>>();
foreach(var line in lines)
{
    var parts = line.Split(": ");
    var src = parts[0];
    var dests = parts[1].Split(' ');
    foreach(var dest in dests)
    {
        if(!graph.ContainsKey(src))
            graph[src] = new();
        if(!graph.ContainsKey(dest))
            graph[dest] = new();

        graph[src].Add(dest, 1);
        graph[dest].Add(src, 1);
    }
}

// ideally use a library that implements this many times faster
// custom implementation based on the mincut algorithm in this paper https://dl.acm.org/doi/pdf/10.1145/263867.263872
int ModifiedMinCut(string a)
{
    var minCut = int.MaxValue;
    var finalGraphGroups = new string[2];
    while(graph.Count() > 2)
    {
        Console.WriteLine("Graph shrinked to size " + graph.Count() + " in " + sw.ElapsedMilliseconds + " ms");
        // find the most tightly coupled neighbouring vertices to set A in order
        var A = new HashSet<string> { a };
        string s = string.Empty;
        string t = string.Empty;
        while (A.Count() < graph.Count())
        {
            var neighbourConnectivity = new Dictionary<string, int>();
            foreach (var node in A)
            {
                foreach ((var neighbour, var weight) in graph[node])
                {
                    if (A.Contains(neighbour))
                        continue;

                    if (!neighbourConnectivity.ContainsKey(neighbour))
                        neighbourConnectivity[neighbour] = 0;

                    // too slow => implement manually instead of linq
                    // var newConnectivity = graph[neighbour].IntersectBy(A, kvp => kvp.Key).Sum(kvp => kvp.Value);

                    var newConnectivity = 0;
                    foreach ((var neighboursNeighbour, var neighboursWeight) in graph[neighbour])
                        if (A.Contains(neighboursNeighbour))
                            newConnectivity += neighboursWeight;

                    neighbourConnectivity[neighbour] = Math.Max(
                        neighbourConnectivity[neighbour],
                        newConnectivity
                        );
                }
            }
            var mostTightlyCoupledNode = neighbourConnectivity.OrderByDescending(kvp => kvp.Value).First().Key;
            s = t;
            t = mostTightlyCoupledNode;
            A.Add(mostTightlyCoupledNode);
        }

        var cut = graph[t].Sum(kvp => kvp.Value);
        if (cut < minCut)
        {
            minCut = cut;
            finalGraphGroups[0] = t;
            finalGraphGroups[1] = string.Join("/", A.SkipLast(1));
        }

        //merge s & t
        var newNode = string.Format("{0}/{1}", s, t);
        var newNeighbours = graph[s];
        foreach((var newNeighbour, var weight) in graph[t])
        {
            if (newNeighbours.ContainsKey(newNeighbour))
                newNeighbours[newNeighbour] += weight;
            else
                newNeighbours[newNeighbour] = weight;
        }
        graph.Add(newNode, newNeighbours);
        graph.Remove(s);
        graph.Remove(t);
        foreach(var node in graph.Keys)
        {
            var newWeight = 0;
            if(graph[node].ContainsKey(s))
            {
                newWeight += graph[node][s];
                graph[node].Remove(s);
            }
            if (graph[node].ContainsKey(t))
            {
                newWeight += graph[node][t];
                graph[node].Remove(t);
            }
            if (newWeight != 0)
                if (node != newNode)
                    graph[node][newNode] = newWeight;
        }
    }
    var group1Size = finalGraphGroups[0].Split('/').Count();
    var group2Size = finalGraphGroups[1].Split('/').Count();
    return group1Size * group2Size;
}

// it takes a ~~ 100 seconds in release mode
Console.WriteLine("Part1: " + ModifiedMinCut(graph.First().Key));
sw.Stop();
Console.WriteLine($"Took {sw.ElapsedMilliseconds} ms");
Console.ReadKey();