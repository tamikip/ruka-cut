using System.Diagnostics;
using System.Globalization;

namespace RukaCut.Linux;

internal sealed class LinuxAudioTools
{
    private Process? recording;
    public bool IsRecording => recording is { HasExited: false };

    public async Task StartRecordingAsync(string path)
    {
        if (!OperatingSystem.IsLinux()) throw new PlatformNotSupportedException("Linux build only");
        var sink = (await RunTextAsync("pactl", "get-default-sink")).Trim();
        if (sink.Length == 0) throw new InvalidOperationException("No PipeWire/PulseAudio output found");
        recording = Start("ffmpeg", ["-y", "-f", "pulse", "-i", $"{sink}.monitor", "-codec:a", "libmp3lame", "-b:a", "192k", path], true);
    }

    public async Task StopRecordingAsync()
    {
        if (!IsRecording) return;
        await recording!.StandardInput.WriteLineAsync("q");
        await recording.WaitForExitAsync();
        recording.Dispose();
        recording = null;
    }

    public async Task<double> ProbeDurationAsync(string path)
    {
        var text = await RunTextAsync("ffprobe", "-v", "error", "-show_entries", "format=duration", "-of", "default=nw=1:nk=1", path);
        return double.Parse(text.Trim(), CultureInfo.InvariantCulture);
    }

    public Task PreviewAsync(string path, double start, double end) => RunAsync("ffplay", "-nodisp", "-autoexit", "-loglevel", "error", "-ss", Number(start), "-t", Number(end - start), path);
    public Task ExportAsync(string source, string output, double start, double end) => RunAsync("ffmpeg", "-y", "-ss", Number(start), "-t", Number(end - start), "-i", source, "-codec:a", "libmp3lame", "-b:a", "192k", output);

    private static string Number(double value) => value.ToString("0.###", CultureInfo.InvariantCulture);
    private static async Task RunAsync(string file, params string[] args) { using var process = Start(file, args, false); await process.WaitForExitAsync(); if (process.ExitCode != 0) throw new InvalidOperationException($"{file} failed"); }
    private static async Task<string> RunTextAsync(string file, params string[] args) { using var process = Start(file, args, false, true); var text = await process.StandardOutput.ReadToEndAsync(); await process.WaitForExitAsync(); if (process.ExitCode != 0) throw new InvalidOperationException($"{file} is required"); return text; }
    private static Process Start(string file, IEnumerable<string> args, bool input, bool output = false)
    {
        var info = new ProcessStartInfo(file) { UseShellExecute = false, RedirectStandardInput = input, RedirectStandardOutput = output, CreateNoWindow = true };
        foreach (var arg in args) info.ArgumentList.Add(arg);
        return Process.Start(info) ?? throw new InvalidOperationException($"Unable to start {file}");
    }
}
