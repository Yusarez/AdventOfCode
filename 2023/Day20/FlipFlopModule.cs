namespace Day20;

internal class FlipFlopModule : IModule
{
    private bool _isOn;
    public string Name { get; init; }
    public string[] Destinations { get; init; }

    public FlipFlopModule(string name, string[] destinations)
    {
        Name = name;
        Destinations = destinations;
        Reset();
    }

    public IEnumerable<Pulse> GetNextPulses(Pulse incomingPulse)
    {
        if (incomingPulse.IsHigh)
            yield break;

        // receives low pulse
        // flip between on and off
        _isOn = !_isOn;

        foreach (var dest in Destinations)
            yield return new Pulse
            {
                Source = Name,
                Destination = dest,
                IsHigh = _isOn,
            };
    }

    public void Reset()
    {
        _isOn = false;
    }
}
