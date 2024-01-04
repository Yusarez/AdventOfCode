using System.Diagnostics;

namespace Day20;

[DebuggerDisplay("{Source, nq} -{(IsHigh ? \"high\" : \"low\"), nq}-> {Destination, nq}")]
struct Pulse
{
    public string Source;
    public string Destination;
    public bool IsHigh;
}