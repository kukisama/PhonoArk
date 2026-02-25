# purecodex

该目录用于承载 **PhonoArk 安卓原生重构版**（非 Avalonia/跨平台 UI）。

当前提供安卓原生实现（Jetpack Compose + Navigation），并对齐原应用的核心页面入口：

- 音标图表（IpaChartView）
- 收藏（FavoritesView）
- 测验（ExamView）
- 历史（ExamHistoryView）
- 设置（SettingsView）

当前已完成的能力（继续向 1:1 收敛）：

- 从 `phoneme-word-bank.json` 加载音标与单词数据；
- 音标图表页面支持分类筛选、详情查看、收藏切换、TTS 播放；
- 收藏页面支持查看、取消、清空；
- 测验页面支持按题量生成题目、作答反馈、完成后写入历史；
- 历史页面支持成绩展示、高频错误音标统计、清空；
- 设置页面支持口音、音量、默认题量、深色模式、语言参数持久化（SharedPreferences）。

## 本地构建（需要 Android SDK）

```bash
cd <仓库根目录>
gradle -p purecodex assembleDebug
```
