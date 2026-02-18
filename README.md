# PhonoArk

音律方舟，将"音标法则"比作"声音的韵律"，而"方舟"是承载希望的工具。

一款跨平台英语学习应用，帮助你掌握国际音标（IPA）和发音。

## 功能特性

### 🎯 IPA 学习
- **交互式 IPA 图表**：按元音、双元音和辅音分类的完整音标表
- **双口音支持**：在美式发音（GenAm）和英式发音（RP）之间切换
- **示例单词**：每个音素配有 4-6 个带 IPA 标注的示例单词
- **音频播放**：聆听纯音素发音和单词发音
- **收藏系统**：收藏音素并将其整理到自定义分组中

### 📝 练习与考试
- **随机考试**：通过自定义题目数量来测试你的知识
- **范围选择**：练习所有音素或专注于收藏的音素
- **即时反馈**：选择后立即查看正确答案
- **进度追踪**：查看包含分数、日期和用时的考试历史
- **统计分析**：追踪你的平均表现趋势

### ⚙️ 设置
- **默认口音**：选择美式或英式发音
- **音量控制**：调整音频播放音量
- **考试配置**：设置默认题目数量
- **主题切换**：在浅色和深色模式之间切换
- **学习提醒**：可选的学习提醒功能（即将推出）

### 📖 附加功能
- **单词学习模块**：未来词汇学习功能的占位模块
- **本地持久化**：所有数据通过 SQLite 本地存储
- **跨平台**：可在 Windows、Android、iOS 上运行（需安装相应工作负载）

## 技术栈

- **框架**：.NET 10.0 搭配 Avalonia UI 11.3
- **UI 渲染**：SkiaSharp 图形引擎
- **数据库**：SQLite 搭配 Entity Framework Core
- **架构**：MVVM 模式，使用 CommunityToolkit.Mvvm
- **平台支持**：桌面端（Windows/Linux/macOS）、移动端（Android/iOS）、Web 端（浏览器）

## 快速开始

### 前置条件
- .NET 10.0 SDK 或更高版本
- Visual Studio 2022 或 JetBrains Rider（推荐）

### 构建应用

1. 克隆仓库：
```bash
git clone https://github.com/kukisama/PhonoArk.git
cd PhonoArk
```

2. 还原依赖：
```bash
cd PhonoArk
dotnet restore
```

3. 构建桌面应用：
```bash
dotnet build PhonoArk.Desktop/PhonoArk.Desktop.csproj
```

4. 运行应用：
```bash
dotnet run --project PhonoArk.Desktop/PhonoArk.Desktop.csproj
```

### 移动端构建

Android 端：
```bash
# 首先安装 Android 工作负载
dotnet workload install android
dotnet build PhonoArk.Android/PhonoArk.Android.csproj
```

iOS 端：
```bash
# 首先安装 iOS 工作负载（仅限 macOS）
dotnet workload install ios
dotnet build PhonoArk.iOS/PhonoArk.iOS.csproj
```

## 项目结构

```
PhonoArk/
├── PhonoArk/              # 核心共享库
│   ├── Models/            # 数据模型
│   ├── ViewModels/        # MVVM 视图模型
│   ├── Views/             # Avalonia XAML 视图
│   ├── Services/          # 业务逻辑服务
│   ├── Data/              # 数据库上下文
│   └── Converters/        # 值转换器
├── PhonoArk.Desktop/      # 桌面端平台项目
├── PhonoArk.Android/      # Android 平台项目
├── PhonoArk.iOS/          # iOS 平台项目
└── PhonoArk.Browser/      # WebAssembly 项目
```

## 数据模型

- **Phoneme**：包含类型、描述和示例单词的 IPA 音素符号
- **ExampleWord**：包含 IPA 标注和音频路径的单词
- **FavoritePhoneme**：用户收藏的音素及其分组
- **ExamResult**：包含分数和统计信息的考试历史
- **AppSettings**：用户偏好与配置

## 贡献

欢迎贡献！请随时提交 Issue 和 Pull Request。

## 许可证

本项目基于 [MIT License](LICENSE) 许可协议发布。

## 路线图

- [ ] 添加音素和单词的实际音频文件
- [ ] 实现带闪卡功能的单词学习模块
- [ ] 添加间隔重复算法
- [ ] 加入发音录制与对比功能
- [ ] 添加更全面的考试模式
- [ ] 实现跨设备进度的云同步
- [ ] 支持更多语言和口音
- [ ] 添加游戏化元素（成就、连续学习天数）

## 致谢

使用 Avalonia UI 和 .NET 用 ❤️ 开发
