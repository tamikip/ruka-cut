namespace RukaCut;

public enum AppLanguage { Chinese, English }

public static class UiText
{
    private static readonly IReadOnlyDictionary<string, string> Chinese = new Dictionary<string, string>
    {
        ["Record"] = "录音", ["Stop"] = "停止", ["Open"] = "打开", ["Preview"] = "试听",
        ["Export"] = "导出 MP3", ["Ready"] = "就绪", ["Trim"] = "裁剪",
        ["NoAudio"] = "尚未选择音频", ["Finalizing"] = "正在完成文件…", ["Recording"] = "录制中",
        ["Encoding"] = "正在编码 MP3…", ["RecordFailed"] = "录制失败", ["SavedMp3"] = "已保存 MP3",
        ["EncodeFailed"] = "MP3 编码失败", ["Loading"] = "正在读取波形…", ["ReadFailed"] = "无法读取音频",
        ["ClipSaved"] = "裁剪已保存", ["OpenFailed"] = "打开失败", ["SaveFailed"] = "保存失败",
        ["PreviewFailed"] = "试听失败", ["CannotRecord"] = "无法开始录音：", ["OpenAudio"] = "打开音频",
        ["AudioFiles"] = "音频文件", ["Mp3Audio"] = "MP3 音频", ["WavAudio"] = "WAV 音频",
        ["EmptyWave"] = "录制或打开音频", ["ClipSuffix"] = "裁剪"
    };

    private static readonly IReadOnlyDictionary<string, string> English = new Dictionary<string, string>
    {
        ["Record"] = "Record", ["Stop"] = "Stop", ["Open"] = "Open", ["Preview"] = "Preview",
        ["Export"] = "Export MP3", ["Ready"] = "Ready", ["Trim"] = "Trim",
        ["NoAudio"] = "No audio selected", ["Finalizing"] = "Finalizing…", ["Recording"] = "Recording",
        ["Encoding"] = "Encoding MP3…", ["RecordFailed"] = "Recording failed", ["SavedMp3"] = "MP3 saved",
        ["EncodeFailed"] = "MP3 encoding failed", ["Loading"] = "Loading waveform…", ["ReadFailed"] = "Unable to read audio",
        ["ClipSaved"] = "Clip saved", ["OpenFailed"] = "Open failed", ["SaveFailed"] = "Save failed",
        ["PreviewFailed"] = "Preview failed", ["CannotRecord"] = "Unable to start recording:", ["OpenAudio"] = "Open audio",
        ["AudioFiles"] = "Audio files", ["Mp3Audio"] = "MP3 audio", ["WavAudio"] = "WAV audio",
        ["EmptyWave"] = "Record or open audio", ["ClipSuffix"] = "trimmed"
    };

    public static IReadOnlyCollection<string> MissingKeys => Chinese.Keys.Except(English.Keys).Concat(English.Keys.Except(Chinese.Keys)).ToArray();

    public static string Get(AppLanguage language, string key) => (language == AppLanguage.Chinese ? Chinese : English)[key];
}
