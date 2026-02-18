# PhonoArk 开发者文档

## 架构概述

PhonoArk 采用 MVVM (Model-View-ViewModel) 模式，使用 Avalonia UI 框架和 CommunityToolkit.Mvvm 进行代码生成。

### 项目结构

```
PhonoArk/
├── Models/              - 领域模型和实体
├── ViewModels/          - MVVM 视图模型及业务逻辑
├── Views/               - Avalonia XAML 用户界面
├── Services/            - 业务逻辑和数据访问
├── Data/                - Entity Framework Core DbContext
├── Converters/          - XAML 值转换器
└── Assets/              - 图片、图标和资源
```

### 核心组件

#### Models
- **Phoneme**：表示一个 IPA 音素及其示例单词
- **ExampleWord**：包含 IPA 转写和音频路径的单词
- **FavoritePhoneme**：用户收藏的音素
- **ExamResult**：考试历史记录
- **AppSettings**：应用程序配置

#### Services
- **PhonemeDataService**：提供 IPA 数据（44 个音素及示例）
- **AudioService**：处理音频播放（占位实现）
- **FavoriteService**：管理音素收藏
- **ExamService**：生成和管理考试题目
- **ExamHistoryService**：跟踪考试结果
- **SettingsService**：管理应用设置

#### ViewModels
- **MainViewModel**：根视图模型，负责导航
- **IpaChartViewModel**：IPA 图表展示和交互
- **ExamViewModel**：考试流程和题目管理
- **ExamHistoryViewModel**：历史记录展示
- **FavoritesViewModel**：收藏管理
- **SettingsViewModel**：设置配置

## 数据库架构

使用 SQLite 和 Entity Framework Core：

```sql
-- FavoritePhonemes（收藏音素）
Id (INTEGER PRIMARY KEY)
PhonemeSymbol (TEXT)
GroupName (TEXT DEFAULT 'Default')
CreatedAt (TEXT)

-- ExamResults（考试结果）
Id (INTEGER PRIMARY KEY)
ExamDate (TEXT)
TotalQuestions (INTEGER)
CorrectAnswers (INTEGER)
ExamScope (TEXT DEFAULT 'All')
Duration (TEXT)

-- Settings（设置）
Id (INTEGER PRIMARY KEY)
DefaultAccent (INTEGER)
Volume (REAL)
ExamQuestionCount (INTEGER)
DarkMode (INTEGER)
RemindersEnabled (INTEGER)
```

## 添加新功能

### 添加新音素

编辑 `Services/PhonemeDataService.cs`：
```csharp
_phonemes.Add(new Phoneme
{
    Symbol = "ə",
    Type = PhonemeType.Vowel,
    Description = "Schwa vowel",
    ExampleWords = new List<ExampleWord>
    {
        new() { Word = "about", IpaTranscription = "/əˈbaʊt/" }
    }
});
```

### 添加新视图

1. 在 `Views/` 文件夹中创建 XAML 文件
2. 在 `ViewModels/` 中创建对应的 ViewModel
3. 在 `MainViewModel.cs` 中注册导航
4. 在 `MainView.axaml` 中添加 DataTemplate

### 实现音频播放

目前 AudioService 是一个占位实现。要添加真实音频：

1. 将音频文件添加到 `Assets/Audio/` 文件夹
2. 更新 `Phoneme` 和 `ExampleWord` 模型，设置正确的路径
3. 在 `AudioService.cs` 中实现平台特定的音频功能
4. 使用 Avalonia 的 AssetLoader 进行跨平台文件访问

示例：
```csharp
var uri = new Uri($"avares://PhonoArk/Assets/Audio/{audioPath}");
var stream = AssetLoader.Open(uri);
// 使用平台音频 API 播放音频流
```

## 测试

### 手动测试清单

1. **IPA 图表**
   - [ ] 所有音素正确显示
   - [ ] 点击音素显示详情面板
   - [ ] 口音切换功能正常
   - [ ] 收藏切换功能正常

2. **考试**
   - [ ] 能够使用不同设置开始考试
   - [ ] 题目正确显示
   - [ ] 答案选择功能正常
   - [ ] 反馈显示正确/错误
   - [ ] 结果保存到历史记录

3. **设置**
   - [ ] 所有设置正确保存/加载
   - [ ] 音量滑块功能正常
   - [ ] 主题切换功能正常（待实现）

## 常见问题

### 数据库未创建
- 检查 LocalApplicationData 文件夹的写入权限
- 验证 App.axaml.cs 中的连接字符串
- 如果存在迁移，运行 `dotnet ef database update`

### UI 未更新
- 确保属性使用了 `[ObservableProperty]` 特性
- 检查 DataContext 是否正确设置
- 验证 XAML 中的绑定使用了正确的属性名称

### 导航不工作
- 检查 MainViewModel 是否注册了所有 ViewModel
- 验证 MainView.axaml 中的 DataTemplate
- 确保 MainView.axaml.cs 中的事件处理程序调用了正确的方法

## 构建与部署

### 桌面端 (Windows/Linux/macOS)
```bash
# 调试构建
dotnet build PhonoArk.Desktop/PhonoArk.Desktop.csproj

# 发布构建
dotnet publish PhonoArk.Desktop/PhonoArk.Desktop.csproj -c Release -o ./publish

# 自包含部署（包含 .NET 运行时）
dotnet publish PhonoArk.Desktop/PhonoArk.Desktop.csproj \
  -c Release \
  -r win-x64 \
  --self-contained true \
  -o ./publish
```

### Android
```bash
# 安装工作负载
dotnet workload install android

# 构建 APK
dotnet build PhonoArk.Android/PhonoArk.Android.csproj -c Release

# APK 位置：PhonoArk.Android/bin/Release/net10.0-android/
```

### iOS（仅限 macOS）
```bash
# 安装工作负载
dotnet workload install ios

# 构建
dotnet build PhonoArk.iOS/PhonoArk.iOS.csproj -c Release
```

## 性能优化

1. **延迟加载数据**：按需加载音素
2. **虚拟化**：对长列表使用 VirtualizingStackPanel
3. **图片缓存**：缓存已加载的图片/资源
4. **数据库索引**：为频繁查询的列添加索引
5. **异步操作**：I/O 操作始终使用 async/await

## 未来增强功能

1. **音频集成**
   - 添加真实的音素和单词音频文件
   - 实现平台特定的音频播放
   - 添加录音功能

2. **高级考试模式**
   - 限时考试
   - 音素识别多选题
   - 带发音评分的口语练习

3. **云同步**
   - 用户认证
   - 云数据库 (Firebase/Azure)
   - 跨设备进度同步

4. **数据分析**
   - 跟踪学习进度
   - 识别薄弱环节
   - 个性化推荐

## 贡献指南

1. Fork 本仓库
2. 创建功能分支
3. 进行更改
4. 编写测试（待测试基础设施完善后）
5. 提交 Pull Request

## 许可证

MIT 许可证 - 详情请参阅 LICENSE 文件
