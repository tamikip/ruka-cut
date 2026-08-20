using NAudio.Wave;

namespace RukaCut;

internal sealed class MainForm : Form
{
    private readonly SystemLoopbackRecorder recorder = new();
    private readonly AudioPreviewPlayer preview = new();
    private readonly System.Windows.Forms.Timer timer = new() { Interval = 250 };
    private readonly System.Windows.Forms.Timer previewTimer = new() { Interval = 50 };
    private readonly ModernButton recordButton = new();
    private readonly ModernButton openButton = SecondaryButton();
    private readonly ModernButton previewButton = SecondaryButton();
    private readonly ModernButton saveButton = new();
    private readonly ModernButton languageButton = SecondaryButton();
    private readonly Label recordTitle = NewLabel("", 12, Color.White, FontStyle.Bold);
    private readonly Label editTitle = NewLabel("", 12, Color.White, FontStyle.Bold);
    private readonly Label statusLabel = NewLabel("", 10, Color.FromArgb(160, 160, 160));
    private readonly Label timeLabel = NewLabel("00:00.0", 26, Color.White, FontStyle.Bold);
    private readonly Label fileLabel = NewLabel("", 9, Color.FromArgb(128, 128, 128));
    private readonly Label rangeLabel = NewLabel("00:00.0  —  00:00.0", 10, Color.FromArgb(205, 205, 205));
    private readonly WaveformPanel waveform = new();
    private AppLanguage language = AppPreferences.LoadLanguage();
    private string statusKey = "Ready";
    private DateTime recordingStarted;
    private string? currentFile;
    private string? recordingFile;
    private string? recordingTempFile;
    private TimeSpan duration;

    public MainForm()
    {
        Text = "Ruka Cut";
        ClientSize = new Size(820, 540);
        MinimumSize = MaximumSize = new Size(836, 579);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.FromArgb(8, 8, 8);
        ForeColor = Color.White;
        Font = new Font(UiFonts.FamilyName, 10);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;

        var title = NewLabel("Ruka Cut", 24, Color.White, FontStyle.Bold);
        title.SetBounds(32, 24, 240, 44);
        languageButton.SetBounds(724, 24, 64, 36);
        Controls.AddRange([title, languageButton]);

        var recordCard = Card(32, 84, 756, 116);
        recordTitle.SetBounds(22, 18, 160, 28);
        statusLabel.SetBounds(22, 52, 230, 28);
        timeLabel.SetBounds(290, 32, 190, 48);
        timeLabel.TextAlign = ContentAlignment.MiddleCenter;
        recordButton.SetBounds(552, 31, 176, 56);
        recordCard.Controls.AddRange([recordTitle, statusLabel, timeLabel, recordButton]);

        var editCard = Card(32, 220, 756, 286);
        editTitle.SetBounds(22, 18, 180, 28);
        openButton.SetBounds(588, 14, 140, 40);
        fileLabel.SetBounds(22, 48, 530, 24);
        waveform.SetBounds(22, 80, 706, 112);
        rangeLabel.SetBounds(22, 202, 300, 30);
        previewButton.SetBounds(370, 216, 160, 48);
        saveButton.SetBounds(552, 216, 176, 48);
        editCard.Controls.AddRange([editTitle, openButton, fileLabel, waveform, rangeLabel, previewButton, saveButton]);
        Controls.AddRange([recordCard, editCard]);

        recordButton.Click += ToggleRecording;
        openButton.Click += async (_, _) => await PickFileAsync();
        previewButton.Click += TogglePreview;
        saveButton.Click += SaveClip;
        languageButton.Click += (_, _) => ToggleLanguage();
        waveform.SelectionChanged += (_, _) => { preview.Stop(); UpdateRange(); };
        recorder.Stopped += RecordingStopped;
        preview.Stopped += PreviewStopped;
        timer.Tick += (_, _) => timeLabel.Text = Format(DateTime.Now - recordingStarted);
        previewTimer.Tick += (_, _) => waveform.SetPlaybackRatio(CurrentSelection.RatioAt(preview.Elapsed, duration));
        FormClosing += (_, _) => { preview.Dispose(); recorder.Dispose(); };
        ApplyLanguage();
    }

    private string T(string key) => UiText.Get(language, key);

    private void ToggleLanguage()
    {
        language = language == AppLanguage.Chinese ? AppLanguage.English : AppLanguage.Chinese;
        AppPreferences.SaveLanguage(language);
        ApplyLanguage();
    }

    private void ApplyLanguage()
    {
        recordTitle.Text = T("Record");
        editTitle.Text = T("Trim");
        statusLabel.Text = T(statusKey);
        recordButton.Text = recorder.IsRecording ? $"■  {T("Stop")}" : $"●  {T("Record")}";
        openButton.Text = T("Open");
        previewButton.Text = preview.IsPlaying ? $"■  {T("Stop")}" : $"▶  {T("Preview")}";
        saveButton.Text = T("Export");
        languageButton.Text = language == AppLanguage.Chinese ? "EN" : "中";
        waveform.EmptyText = T("EmptyWave");
        if (currentFile is null) fileLabel.Text = T("NoAudio");
    }

    private void SetStatus(string key)
    {
        statusKey = key;
        statusLabel.Text = T(key);
    }

    private void ToggleRecording(object? sender, EventArgs e)
    {
        if (recorder.IsRecording)
        {
            recordButton.Enabled = false;
            SetStatus("Finalizing");
            recorder.Stop();
            return;
        }

        var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), "RukaCut");
        var prefix = language == AppLanguage.Chinese ? "录音" : "recording";
        var name = $"{prefix}-{DateTime.Now:yyyyMMdd-HHmmss}";
        recordingFile = Path.Combine(folder, $"{name}.mp3");
        recordingTempFile = Path.Combine(Path.GetTempPath(), $"RukaCut-{Guid.NewGuid():N}.wav");
        try
        {
            recorder.Start(recordingTempFile);
            recordingStarted = DateTime.Now;
            timer.Start();
            SetStatus("Recording");
            statusLabel.ForeColor = Color.White;
            recordButton.Text = $"■  {T("Stop")}";
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"{T("CannotRecord")}\n{ex.Message}", "Ruka Cut", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void RecordingStopped(Exception? error)
    {
        if (InvokeRequired) { BeginInvoke(() => RecordingStopped(error)); return; }
        timer.Stop();
        recordButton.Text = $"●  {T("Record")}";
        statusLabel.ForeColor = Color.FromArgb(160, 160, 160);
        SetStatus(error is null ? "Encoding" : "RecordFailed");
        if (error is not null)
        {
            MessageBox.Show(this, error.Message, T("RecordFailed"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            recordButton.Enabled = true;
            return;
        }
        if (recordingFile is null || recordingTempFile is null) return;
        try
        {
            using var wav = new WaveFileReader(recordingTempFile);
            var length = wav.TotalTime;
            await Task.Run(() => Mp3Exporter.ExportSegment(recordingTempFile, recordingFile, TimeSpan.Zero, length));
            SetStatus("SavedMp3");
            await LoadAudioAsync(recordingFile);
        }
        catch (Exception ex)
        {
            SetStatus("EncodeFailed");
            MessageBox.Show(this, ex.Message, T("EncodeFailed"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            File.Delete(recordingTempFile);
            recordingTempFile = null;
            recordButton.Enabled = true;
        }
    }

    private async Task PickFileAsync()
    {
        using var dialog = new OpenFileDialog
        {
            Filter = $"{T("AudioFiles")}|*.mp3;*.wav|{T("Mp3Audio")}|*.mp3|{T("WavAudio")}|*.wav",
            Title = T("OpenAudio")
        };
        if (dialog.ShowDialog(this) == DialogResult.OK) await LoadAudioAsync(dialog.FileName);
    }

    private async Task LoadAudioAsync(string path)
    {
        try
        {
            preview.Stop();
            openButton.Enabled = false;
            fileLabel.Text = T("Loading");
            var result = await Task.Run(() => ReadPeaks(path, 600));
            currentFile = path;
            duration = result.Duration;
            waveform.SetAudio(result.Peaks);
            fileLabel.Text = $"{Path.GetFileName(path)}   ·   {Format(duration)}";
            saveButton.Enabled = duration > TimeSpan.Zero;
            previewButton.Enabled = duration > TimeSpan.Zero;
            UpdateRange();
        }
        catch (Exception ex)
        {
            fileLabel.Text = T("ReadFailed");
            MessageBox.Show(this, ex.Message, T("OpenFailed"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally { openButton.Enabled = true; }
    }

    private void SaveClip(object? sender, EventArgs e)
    {
        if (currentFile is null) return;
        using var dialog = new SaveFileDialog
        {
            Filter = $"{T("Mp3Audio")}|*.mp3",
            FileName = $"{Path.GetFileNameWithoutExtension(currentFile)}-{T("ClipSuffix")}.mp3",
            InitialDirectory = Path.GetDirectoryName(currentFile)
        };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        try
        {
            var range = CurrentSelection;
            Mp3Exporter.ExportSegment(currentFile, dialog.FileName, range.Start, range.End);
            SetStatus("ClipSaved");
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, T("SaveFailed"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void TogglePreview(object? sender, EventArgs e)
    {
        if (preview.IsPlaying) { preview.Stop(); return; }
        if (currentFile is null) return;
        try
        {
            preview.Play(currentFile, CurrentSelection);
            waveform.SetPlaybackRatio(waveform.StartRatio);
            previewTimer.Start();
            previewButton.Text = $"■  {T("Stop")}";
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, T("PreviewFailed"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void PreviewStopped(Exception? error)
    {
        if (InvokeRequired) { BeginInvoke(() => PreviewStopped(error)); return; }
        previewTimer.Stop();
        waveform.SetPlaybackRatio(null);
        previewButton.Text = $"▶  {T("Preview")}";
        if (error is not null)
            MessageBox.Show(this, error.Message, T("PreviewFailed"), MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    private void UpdateRange() => rangeLabel.Text = $"{Format(duration * waveform.StartRatio)}  —  {Format(duration * waveform.EndRatio)}";
    private SelectionRange CurrentSelection => SelectionRange.FromRatios(duration, waveform.StartRatio, waveform.EndRatio);

    private static (float[] Peaks, TimeSpan Duration) ReadPeaks(string path, int count)
    {
        using var reader = new AudioFileReader(path);
        var peaks = new float[count];
        var totalSamples = Math.Max(1L, (long)(reader.TotalTime.TotalSeconds * reader.WaveFormat.SampleRate * reader.WaveFormat.Channels));
        var buffer = new float[8192];
        long seen = 0;
        int read;
        while ((read = reader.Read(buffer, 0, buffer.Length)) > 0)
        {
            for (var i = 0; i < read; i++, seen++)
            {
                var index = (int)Math.Min(count - 1, seen * count / totalSamples);
                peaks[index] = Math.Max(peaks[index], Math.Abs(buffer[i]));
            }
        }
        return (peaks, reader.TotalTime);
    }

    private static RoundedPanel Card(int x, int y, int width, int height) => new()
    {
        Bounds = new Rectangle(x, y, width, height),
        BackColor = Color.FromArgb(24, 24, 24),
        Radius = 18
    };

    private static ModernButton SecondaryButton() => new() { BackColor = Color.FromArgb(42, 42, 42), ForeColor = Color.White };

    private static Label NewLabel(string text, float size, Color color, FontStyle style = FontStyle.Regular) => new()
    {
        Text = text, Font = new Font(UiFonts.FamilyName, size, style), ForeColor = color, BackColor = Color.Transparent
    };

    private static string Format(TimeSpan value) => $"{(int)value.TotalMinutes:00}:{value.Seconds:00}.{value.Milliseconds / 100}";
}
