# ⚡ PhonoArk（音标方舟）

> 基于 **.NET 10 + Avalonia** 的跨平台 IPA 音标学习应用，覆盖桌面端与 Android。

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![.NET 10](https://img.shields.io/badge/.NET-10.0-purple.svg)](https://dotnet.microsoft.com/download/dotnet/10.0)
[![Avalonia UI](https://img.shields.io/badge/Avalonia-11.3-blue.svg)](https://avaloniaui.net/)

PhonoArk 当前重点在于：**IPA 交互图表、真人发音播放、听音辨词测试、错题统计与收藏管理**。  
项目采用清晰分层（Models / Services / ViewModels / Views），便于持续迭代与测试回归。

---

## ✨ 当前能力（基于现有代码）

| 模块 | 说明 |
| --- | --- |
| 🗂️ IPA 音标图表 | 元音 / 双元音 / 辅音分组展示，点击即播发音，选中查看详情与示例单词 |
| 🔊 多口音发音 | US-Jenny 真人录音优先、GenAm 美式 TTS、RP 英式 TTS，三种口音自由切换 |
| 📝 听音辨词测试 | 可配题数与范围（全部 / 仅收藏），四选一即时反馈，1.2 秒自动跳转，round-robin 出题去重 |
| 📊 历史记录与错题统计 | 分页加载考试记录、逐题回顾、平均分统计、按音标维度错误频次分析、仅错题筛选 |
| ⭐ 收藏管理 | 单个音标收藏 / 取消，按类型批量收藏，一键清空，收藏已合并至 IPA 首页 |
| 🩺 语音诊断 | 运行诊断输出平台信息、目标语言、当前语音引擎、语音条目数 |
| ⚙️ 设置项 | 默认口音、音量调节、题目数量、深色模式、学习提醒、界面语言（中/英） |

---

## 🖥️ 平台支持

- **Desktop**：Windows（`PhonoArk/PhonoArk.Desktop`）
- **Android**：`net10.0-android`（`PhonoArk/PhonoArk.Android`）

> 说明：桌面端与 Android 端共用核心业务层，UI 层按平台做交互适配。  
> Avalonia 框架本身支持 macOS 与 Linux，从源码构建后理论上可运行（TTS 回退在非 Windows 平台不可用，真人录音播放不受影响）。作者日常仅使用 Windows 与 Android，因此不提供 macOS / Linux 的打包与测试。

---

## 🚀 快速开始（使用发行包）

前往 [Releases](../../releases) 下载对应平台包。

桌面端启动方式：

- Windows：运行 `PhonoArk.Desktop.exe`

---

## 🛠️ 从源码构建

### 1) 通用（桌面）

```bash
git clone https://github.com/kukisama/PhonoArk.git
cd PhonoArk

dotnet build PhonoArk/PhonoArk.sln
dotnet run --project PhonoArk/PhonoArk.Desktop
```

---

## 🧭 主要页面

当前主导航包含（桌面侧栏 / 移动端底栏）：

- 国际音标（IPA Chart）
- 练习测试（Exam）
- 历史记录（History）
- 设置（Settings）

Android 端采用底部导航方式，功能分组与桌面端保持一致。

---

## 🧱 架构与目录

```text
PhonoArk/
├── PhonoArk/
│   ├── PhonoArk/                  # 共享核心库（Models / Services / ViewModels / Views）
│   ├── PhonoArk.Desktop/          # 桌面 UI 入口
│   ├── PhonoArk.Android/          # Android UI 入口
│   └── scripts/                   # 构建与联调脚本
├── tools/
│   └── msspeechcmd/               # Azure TTS 命令行工具
└── docs/                          # 变更日志与文档
```

---

## 🔊 语音包与构建

- 真人发音采用 US-Jenny 录音包（WAV 格式），构建期自动从 `Data/US-Jenny.zip` 解压并校验一致性。
- Windows 端通过 `System.Speech` 提供 TTS 回退；Android 端通过系统 `TextToSpeech` API 回退。
- 播放优先级：真人录音 WAV → 系统 TTS。

---

## 📚 相关文档

- 发布说明：[`RELEASE_NOTES.md`](RELEASE_NOTES.md)

---

## 📝 许可证

本项目基于 [MIT 许可证](LICENSE) 开源。
