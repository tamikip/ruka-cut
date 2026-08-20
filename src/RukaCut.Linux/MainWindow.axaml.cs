using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Threading;

namespace RukaCut.Linux;

public partial class MainWindow : Window
{
    private readonly LinuxAudioTools audio = new();
    private readonly DispatcherTimer timer = new() { Interval = TimeSpan.FromMilliseconds(100) };
    private DateTime started;
    private string? currentFile;
    private double duration;
    private bool chinese = true;

    public MainWindow()
    {
        InitializeComponent();
        timer.Tick += (_, _) => RecordTime.Text = Format((DateTime.Now - started).TotalSeconds);
        Closed += async (_, _) => await audio.StopRecordingAsync();
    }

    private async void ToggleRecord(object? sender, RoutedEventArgs e)
    {
        if (audio.IsRecording)
        {
            RecordButton.IsEnabled = false;
            await audio.StopRecordingAsync();
            timer.Stop();
            RecordButton.IsEnabled = true;
            RecordButton.Content = chinese ? "●  录音" : "●  Record";
            StatusText.Text = chinese ? "已保存 MP3" : "MP3 saved";
            return;
        }
        try
        {
            var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), "RukaCut");
            Directory.CreateDirectory(folder);
            currentFile = Path.Combine(folder, $"recording-{DateTime.Now:yyyyMMdd-HHmmss}.mp3");
            await audio.StartRecordingAsync(currentFile);
            started = DateTime.Now;
            timer.Start();
            StatusText.Text = chinese ? "录制中" : "Recording";
            RecordButton.Content = chinese ? "■  停止" : "■  Stop";
        }
        catch (Exception ex) { StatusText.Text = ex.Message; }
    }

    private async void OpenAudio(object? sender, RoutedEventArgs e)
    {
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions { AllowMultiple = false, FileTypeFilter = [new FilePickerFileType("Audio") { Patterns = ["*.mp3", "*.wav"] }] });
        if (files.Count == 0) return;
        currentFile = files[0].TryGetLocalPath();
        if (currentFile is null) return;
        try
        {
            duration = await audio.ProbeDurationAsync(currentFile);
            StartSlider.Maximum = EndSlider.Maximum = duration;
            StartSlider.Value = 0;
            EndSlider.Value = duration;
            FileText.Text = $"{Path.GetFileName(currentFile)}  ·  {Format(duration)}";
            PreviewButton.IsEnabled = ExportButton.IsEnabled = duration > 0;
            UpdateSelection();
        }
        catch (Exception ex) { FileText.Text = ex.Message; }
    }

    private async void Preview(object? sender, RoutedEventArgs e) { if (currentFile is not null) await audio.PreviewAsync(currentFile, StartSlider.Value, EndSlider.Value); }

    private async void Export(object? sender, RoutedEventArgs e)
    {
        if (currentFile is null) return;
        var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions { SuggestedFileName = $"{Path.GetFileNameWithoutExtension(currentFile)}-trimmed.mp3", DefaultExtension = "mp3", FileTypeChoices = [new FilePickerFileType("MP3") { Patterns = ["*.mp3"] }] });
        var path = file?.TryGetLocalPath();
        if (path is null) return;
        try { await audio.ExportAsync(currentFile, path, StartSlider.Value, EndSlider.Value); StatusText.Text = chinese ? "裁剪已保存" : "Clip saved"; }
        catch (Exception ex) { StatusText.Text = ex.Message; }
    }

    private void SelectionChanged(object? sender, Avalonia.Controls.Primitives.RangeBaseValueChangedEventArgs e) => UpdateSelection();
    private void UpdateSelection()
    {
        if (StartSlider is null || EndSlider is null) return;
        if (StartSlider.Value > EndSlider.Value) EndSlider.Value = StartSlider.Value;
        StartText.Text = Format(StartSlider.Value);
        EndText.Text = Format(EndSlider.Value);
    }

    private void ToggleLanguage(object? sender, RoutedEventArgs e)
    {
        chinese = !chinese;
        LanguageButton.Content = chinese ? "EN" : "中";
        RecordTitle.Text = chinese ? "录音" : "Record";
        TrimTitle.Text = chinese ? "裁剪" : "Trim";
        OpenButton.Content = chinese ? "打开" : "Open";
        PreviewButton.Content = chinese ? "▶  试听" : "▶  Preview";
        ExportButton.Content = chinese ? "导出 MP3" : "Export MP3";
        HintText.Text = chinese ? "左侧选择开始，右侧选择结束" : "Choose start on the left and end on the right";
        ProjectLink.Content = chinese ? "作者：TamikiP  ·  GitHub 开源" : "Author: TamikiP  ·  Open source on GitHub";
    }

    private static void OpenProject(object? sender, RoutedEventArgs e) => Process.Start(new ProcessStartInfo("https://github.com/tamikip/ruka-cut") { UseShellExecute = true });
    private static string Format(double seconds) => $"{(int)(seconds / 60):00}:{(int)seconds % 60:00}.{(int)(seconds * 10) % 10}";
}
