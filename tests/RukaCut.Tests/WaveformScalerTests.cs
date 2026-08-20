using Xunit;

namespace RukaCut.Tests;

public sealed class WaveformScalerTests
{
    [Fact]
    public void CalculateGain_MakesQuietAudioVisible()
    {
        float[] peaks = [0.01f, 0.015f, 0.02f, 0.012f, 0.018f];

        var gain = WaveformScaler.CalculateGain(peaks);

        Assert.InRange(0.02f * gain, 0.75f, 1f);
    }

    [Fact]
    public void CalculateGain_DoesNotAmplifyNearSilence()
    {
        float[] peaks = [0f, 0.00001f, 0.00002f];

        Assert.Equal(1f, WaveformScaler.CalculateGain(peaks));
    }

    [Fact]
    public void CalculateGain_IgnoresSingleOutlier()
    {
        var peaks = Enumerable.Repeat(0.1f, 39).Append(1f).ToArray();

        Assert.InRange(WaveformScaler.CalculateGain(peaks), 7f, 9f);
    }
}
