namespace RukaCut;

public readonly record struct SelectionRange(TimeSpan Start, TimeSpan End)
{
    public TimeSpan Length => End - Start;

    public static SelectionRange FromRatios(TimeSpan duration, float startRatio, float endRatio) =>
        new(At(duration, startRatio), At(duration, endRatio));

    public float RatioAt(TimeSpan elapsed, TimeSpan duration)
    {
        if (duration <= TimeSpan.Zero) return 0;
        var position = Start + (elapsed > Length ? Length : elapsed);
        return (float)Math.Clamp(position / duration, 0, 1);
    }

    private static TimeSpan At(TimeSpan duration, float ratio) =>
        TimeSpan.FromMilliseconds(Math.Round(duration.TotalMilliseconds * Math.Clamp(ratio, 0, 1)));
}
