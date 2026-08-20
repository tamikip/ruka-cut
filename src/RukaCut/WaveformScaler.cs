namespace RukaCut;

public static class WaveformScaler
{
    public static float CalculateGain(IReadOnlyCollection<float> peaks)
    {
        var effective = peaks.Where(value => value >= 0.001f).Order().ToArray();
        if (effective.Length == 0) return 1;

        var index = (int)Math.Ceiling((effective.Length - 1) * 0.95);
        return Math.Clamp(0.85f / effective[index], 1, 100);
    }
}
