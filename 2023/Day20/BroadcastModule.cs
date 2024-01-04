namespace Day20;

internal class BroadcastModule : IModule
{
    public string Name { get; init; }
    public string[] Destinations { get; init; }

    public BroadcastModule(string name, string[] destinations)
    {
        Name = name;
        Destinations = destinations;
    }

    public IEnumerable<Pulse> GetNextPulses(Pulse incomingPulse)
    {
        foreach (var dest in Destinations)
            yield return new Pulse
            {
                Source = Name,
                Destination = dest,
                IsHigh = incomingPulse.IsHigh,
            };
    }

    public void Reset(){} // does nothing
}
