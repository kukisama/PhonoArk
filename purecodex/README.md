# purecodex

该目录用于承载 **PhonoArk 安卓原生重构版**（非 Avalonia/跨平台 UI）。

当前提供最小可运行的安卓原生框架（Jetpack Compose + Navigation），并对齐原应用的核心页面入口：

- 音标图表（IpaChartView）
- 收藏（FavoritesView）
- 测验（ExamView）
- 历史（ExamHistoryView）
- 设置（SettingsView）

> 说明：当前阶段先完成原生工程骨架与页面结构对齐，后续在该框架内逐步迁移原项目功能逻辑与视觉细节，以实现 1:1 功能与界面一致性。

## 本地构建（需要 Android SDK）

```bash
cd /home/runner/work/PhonoArk/PhonoArk
gradle -p purecodex assembleDebug
```
