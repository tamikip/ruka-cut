# Ruka Cut

[![build](https://github.com/tamikip/ruka-cut/actions/workflows/build.yml/badge.svg)](https://github.com/tamikip/ruka-cut/actions/workflows/build.yml)
[![license](https://img.shields.io/badge/license-MIT-white.svg)](LICENSE)

一款开源、轻量的 Windows 电脑内录与 MP3 裁剪工具。

## 功能

- WASAPI 回环录制电脑声音
- 录音自动输出 192 kbps MP3
- 打开 MP3/WAV，拖动波形两端完成裁剪
- 只试听当前选择区域，并显示播放进度
- 自适应波形振幅显示
- 中文 / English 即时切换
- 黑白圆角界面，无账号、广告和遥测

所有音频处理均在本机完成。录音默认保存到 Windows 的 `音乐/RukaCut`。

## 下载

从 [Releases](https://github.com/tamikip/ruka-cut/releases) 下载最新压缩包。运行环境为 Windows 10/11 和 [.NET 8 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0)。

## 构建

```powershell
dotnet restore
dotnet test RukaCut.slnx -c Release
dotnet publish src/RukaCut/RukaCut.csproj -c Release --self-contained false -o release/RukaCut
```

## English

Ruka Cut is an open-source, lightweight Windows app for recording system audio and trimming MP3 files.

- WASAPI loopback recording with 192 kbps MP3 output
- MP3/WAV waveform trimming and selection-only preview
- Adaptive waveform scaling and playback progress
- Instant Chinese / English switching
- Minimal monochrome rounded UI with no accounts, ads, or telemetry

Download the latest package from [Releases](https://github.com/tamikip/ruka-cut/releases). Windows 10/11 and the [.NET 8 Desktop Runtime](https://dotnet.microsoft.com/download/dotnet/8.0) are required.

## Contributing

Issues and pull requests are welcome. See [CONTRIBUTING.md](CONTRIBUTING.md).

## License

[MIT](LICENSE) · See [third-party notices](THIRD-PARTY-NOTICES.md).
