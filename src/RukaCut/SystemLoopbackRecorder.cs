using NAudio.Wave;

namespace RukaCut;

internal sealed class SystemLoopbackRecorder : IDisposable
{
    private WasapiLoopbackCapture? capture;
    private WaveFileWriter? writer;

    public bool IsRecording => capture is not null;
    public event Action<Exception?>? Stopped;

    public void Start(string path)
    {
        if (IsRecording) return;
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path))!);
        try
        {
            capture = new WasapiLoopbackCapture();
            writer = new WaveFileWriter(path, capture.WaveFormat);
            capture.DataAvailable += OnDataAvailable;
            capture.RecordingStopped += OnRecordingStopped;
            capture.StartRecording();
        }
        catch
        {
            DisposeResources();
            throw;
        }
    }

    public void Stop() => capture?.StopRecording();

    private void OnDataAvailable(object? sender, WaveInEventArgs e) => writer?.Write(e.Buffer, 0, e.BytesRecorded);

    private void OnRecordingStopped(object? sender, StoppedEventArgs e)
    {
        DisposeResources();
        Stopped?.Invoke(e.Exception);
    }

    private void DisposeResources()
    {
        writer?.Dispose();
        writer = null;
        capture?.Dispose();
        capture = null;
    }

    public void Dispose()
    {
        if (IsRecording) Stop();
        else DisposeResources();
    }
}
