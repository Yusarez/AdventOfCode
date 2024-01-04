namespace Day20;

internal interface IModule
{
    public string Name { get; init; }
    public string[] Destinations { get; init; }

    IEnumerable<Pulse> GetNextPulses(Pulse incomingPulse);
    void Reset();
}
