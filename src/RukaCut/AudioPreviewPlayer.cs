using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System.Diagnostics;

namespace RukaCut;

internal sealed class AudioPreviewPlayer : IDisposable
{
    private WaveOutEvent? output;
    private AudioFileReader? reader;
    private readonly Stopwatch clock = new();

    public bool IsPlaying => output?.PlaybackState == PlaybackState.Playing;
    public TimeSpan Elapsed => clock.Elapsed;
    public event Action<Exception?>? Stopped;

    public void Play(string path, SelectionRange range)
    {
        Stop(false);
        reader = new AudioFileReader(path);
        var segment = new OffsetSampleProvider(reader)
        {
            SkipOver = range.Start,
            Take = range.Length
        };
        output = new WaveOutEvent();
        output.PlaybackStopped += OnPlaybackStopped;
        output.Init(segment);
        output.Play();
        clock.Restart();
    }

    public void Stop() => Stop(true);

    private void Stop(bool notify)
    {
        clock.Stop();
        if (output is null) return;
        output.PlaybackStopped -= OnPlaybackStopped;
        output.Stop();
        output.Dispose();
        output = null;
        reader?.Dispose();
        reader = null;
        clock.Reset();
        if (notify) Stopped?.Invoke(null);
    }

    private void OnPlaybackStopped(object? sender, StoppedEventArgs e)
    {
        Stop(false);
        Stopped?.Invoke(e.Exception);
    }

    public void Dispose() => Stop(false);
}
