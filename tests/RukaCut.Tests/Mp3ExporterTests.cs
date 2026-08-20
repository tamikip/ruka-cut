using NAudio.Wave;
using Xunit;

namespace RukaCut.Tests;

public sealed class Mp3ExporterTests : IDisposable
{
    private readonly string directory = Path.Combine(Path.GetTempPath(), $"RukaCut-Mp3-{Guid.NewGuid():N}");

    public Mp3ExporterTests() => Directory.CreateDirectory(directory);

    [Trait("Category", "WindowsIntegration")]
    [Fact]
    public void ExportSegment_CreatesPlayableMp3NearRequestedDuration()
    {
        var source = Path.Combine(directory, "source.wav");
        var output = Path.Combine(directory, "clip.mp3");
        CreateTone(source, TimeSpan.FromSeconds(2));

        Mp3Exporter.ExportSegment(source, output, TimeSpan.FromMilliseconds(400), TimeSpan.FromMilliseconds(1400));

        Assert.True(new FileInfo(output).Length > 1_000);
        using var reader = new Mp3FileReader(output);
        Assert.InRange(reader.TotalTime.TotalMilliseconds, 900, 1150);
    }

    [Trait("Category", "WindowsIntegration")]
    [Fact]
    public void ExportSegment_RejectsInvalidRange()
    {
        var source = Path.Combine(directory, "source.wav");
        CreateTone(source, TimeSpan.FromSeconds(1));

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            Mp3Exporter.ExportSegment(source, Path.Combine(directory, "clip.mp3"), TimeSpan.FromSeconds(1), TimeSpan.Zero));
    }

    private static void CreateTone(string path, TimeSpan duration)
    {
        var format = new WaveFormat(44_100, 16, 2);
        using var writer = new WaveFileWriter(path, format);
        var frames = (int)(duration.TotalSeconds * format.SampleRate);
        for (var i = 0; i < frames; i++)
        {
            var sample = (short)(Math.Sin(2 * Math.PI * 440 * i / format.SampleRate) * short.MaxValue / 4);
            writer.WriteByte((byte)(sample & 0xff));
            writer.WriteByte((byte)(sample >> 8));
            writer.WriteByte((byte)(sample & 0xff));
            writer.WriteByte((byte)(sample >> 8));
        }
    }

    public void Dispose() => Directory.Delete(directory, true);
}
