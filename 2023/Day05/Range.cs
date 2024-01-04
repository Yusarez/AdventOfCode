namespace Day05;

internal class Range
{
    public static Range Invalid = new Range(long.MinValue, long.MinValue);
    public static Range Positive = new Range(0, long.MaxValue);

    public long Start { get; private set; }
    public long End { get; private set; }

    private Range(long start, long end) 
    {
        Start = start;
        End = end;
    }

    public static Range FromStartToEnd(long start, long end) => new Range(start, end);
    public static Range FromStartAndSize(long start, long size) => new Range(start, start + size - 1);

    public void Shift(long distance)
    {
        Start += distance;
        End += distance;
    }

    public Range OverlappingRange(Range other)
    {
        if (Start <= other.Start && other.End <= End)
            return new Range(other.Start, other.End);

        else if (other.Start <= Start && End <= other.End)
            return new Range(Start, End);

        else if (other.Start <= Start && Start <= other.End)
            return new Range(Start, other.End);

        else if (Start <= other.Start && other.Start <= End)
            return new Range(other.Start, End);

        return Invalid;
    }
}
