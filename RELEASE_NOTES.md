# PhonoArk v1.1.0

## 更新

- **2026-02-25（最新）**：构建脚本 v2 重构。`android-dev-oneclick-v2.ps1` 由 6 固定场景改为 3 位置参数自由组合 + 无参数交互式菜单，新增 P 核亲和性、超线程控制、Arm64Only 加速打包等构建调优能力，VS Code 诊断零误报。
- **2026-02-24**：考试页 Android 适配加固。明确分离 PC 与 Android 答题区宽度策略，移动端按钮改为动态宽度 + 圆角卡片观感，反馈区与选项区同边界对齐，解决"样式互相影响"问题。
- **2026-02-24**：IPA 首页批量收藏功能上线。新增按类型（元音 / 双元音 / 辅音）一键收藏与一键清空，收藏功能合并至 IPA 首页，移除独立收藏 Tab。
- **2026-02-23**：Android 无障碍闪退修复。绕过 Avalonia 11.3.x 框架 Bug（#20453），遍历 View 树移除无障碍委托，解决历史记录页 100% 崩溃问题。
- **2026-02-22**：US-Jenny 真人发音接入。构建期自动从 zip 解压 WAV 录音包，播放链路升级为"真人录音 → TTS 回退"，音标与单词均支持真人发音。
- **2026-02-21**：Windows TTS 引擎接入。通过 `System.Speech` 反射调用 + STA 线程修复，桌面端获得语音合成能力；Android 端同步接入系统 TextToSpeech API。
- **2026-02-20**：DbContext 生命周期统一。全部 Service 改为 short-lived DbContext 模式，消除并发冲突；fire-and-forget 统一使用 `SafeFireAndForget` 扩展方法。
- **2026-02-18**：i18n 全面迁移。所有页面文案迁移至 `.resx` 资源文件，支持简体中文与 English 双语切换，语言设置即时生效并持久化。

---


### 运行前提

本版本为 **FDD（Framework-dependent）** 发布，需要预先安装：

- [**.NET 10 Runtime**](https://dotnet.microsoft.com/download/dotnet/10.0)
- Android 用户需要允许安装 APK（不同系统版本入口可能为"允许此来源安装应用"）

### 使用方式

| 平台 | 包名 | 快速使用 |
|------|------|----------|
| Windows | `PhonoArk-win-x64-fdd.zip` | 解压后运行 `PhonoArk.Desktop.exe` |
| Android | `PhonoArk-android*.apk` | 手机安装 APK；若拦截，开启"允许未知来源安装"后重试 |

