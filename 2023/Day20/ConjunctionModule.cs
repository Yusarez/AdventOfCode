using System.Diagnostics;

namespace Day20;

internal class ConjunctionModule : IModule
{
    private Dictionary<string, bool> _inputModulesPulseCache = new Dictionary<string, bool>();

    public string Name { get; init; }
    public string[] Destinations { get; init; }

    public ConjunctionModule(string name, string[] destinations)
    {
        Name = name;
        Destinations = destinations;
    }

    public bool WillSendHighPulse()
    {
        return !_inputModulesPulseCache.All(kvp => kvp.Value);
    }

    public void RegisterInputModule(string input)
    {
        _inputModulesPulseCache[input] = false;
    }

    public IEnumerable<Pulse> GetNextPulses(Pulse incomingPulse)
    {
        Debug.Assert(_inputModulesPulseCache.ContainsKey(incomingPulse.Source));
        _inputModulesPulseCache[incomingPulse.Source] = incomingPulse.IsHigh;
        bool sendHighPulse = !_inputModulesPulseCache.All(kvp => kvp.Value);
        foreach (var dest in Destinations)
            yield return new Pulse
            {
                Source = Name,
                Destination = dest,
                IsHigh = sendHighPulse,
            };
    }

    public void Reset()
    {
        foreach (var key in _inputModulesPulseCache.Keys)
            _inputModulesPulseCache[key] = false;
    }
}
