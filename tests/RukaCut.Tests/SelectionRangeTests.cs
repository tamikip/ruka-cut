using Xunit;

namespace RukaCut.Tests;

public sealed class SelectionRangeTests
{
    [Fact]
    public void FromRatios_ReturnsSelectedTimes()
    {
        var range = SelectionRange.FromRatios(TimeSpan.FromSeconds(10), 0.2f, 0.7f);

        Assert.Equal(TimeSpan.FromSeconds(2), range.Start);
        Assert.Equal(TimeSpan.FromSeconds(7), range.End);
        Assert.Equal(TimeSpan.FromSeconds(5), range.Length);
    }

    [Fact]
    public void RatioAt_MapsPlaybackTimeIntoSelection()
    {
        var range = new SelectionRange(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(7));

        Assert.Equal(0.45f, range.RatioAt(TimeSpan.FromSeconds(2.5), TimeSpan.FromSeconds(10)), 3);
        Assert.Equal(0.7f, range.RatioAt(TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(10)), 3);
    }
}
