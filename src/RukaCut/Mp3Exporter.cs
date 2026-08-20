using NAudio.Wave;

namespace RukaCut;

public static class Mp3Exporter
{
    public static void ExportSegment(string sourcePath, string outputPath, TimeSpan start, TimeSpan end)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourcePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(outputPath);
        if (start < TimeSpan.Zero || end <= start)
            throw new ArgumentOutOfRangeException(nameof(end), "结束时间必须晚于开始时间。");
        if (Path.GetFullPath(sourcePath).Equals(Path.GetFullPath(outputPath), StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("输出文件不能覆盖原文件。", nameof(outputPath));

        using var reader = new AudioFileReader(sourcePath);
        if (start >= reader.TotalTime)
            throw new ArgumentOutOfRangeException(nameof(start), "开始时间超出音频长度。");

        end = end > reader.TotalTime ? reader.TotalTime : end;
        reader.CurrentTime = start;
        var segment = new LimitedWaveProvider(reader, end - start);
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(outputPath))!);
        MediaFoundationEncoder.EncodeToMp3(segment, outputPath, 192_000);
    }

    private sealed class LimitedWaveProvider(IWaveProvider source, TimeSpan duration) : IWaveProvider
    {
        private long remaining = Align((long)(duration.TotalSeconds * source.WaveFormat.AverageBytesPerSecond), source.WaveFormat.BlockAlign);

        public WaveFormat WaveFormat => source.WaveFormat;

        public int Read(byte[] buffer, int offset, int count)
        {
            if (remaining <= 0) return 0;
            count = (int)Math.Min(count, remaining);
            count -= count % WaveFormat.BlockAlign;
            var read = source.Read(buffer, offset, count);
            remaining -= read;
            return read;
        }

        private static long Align(long value, int blockAlign) => value - value % blockAlign;
    }
}
