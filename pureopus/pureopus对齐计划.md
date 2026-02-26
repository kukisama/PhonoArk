# pureopus → PhonoArk.Android 对齐计划

> 基准日期：2026-02-26（修订：2026-02-26）
> 对比范围：PhonoArk.Android (Avalonia C#) ↔ pureopus (Kotlin + Jetpack Compose)
> 结论先行：pureopus 架构层面已对齐，**剩余 22 项差异**可按 7 批次递进修复。

### 修订说明
基于实际安卓使用场景进行以下重大修正：
1. **收藏逻辑变更**：PhonoArk.Android 已移除独立"收藏"标签页，收藏功能完全内嵌于 IPA Chart 页（批量收藏按钮 + 单个音标收藏星标）。pureopus 的独立 Favorites 标签页需**删除**。
2. **音频策略修正**：安卓上因中国手机的自动 TTS 不可用，真人语音（USJenny）为**默认语音**，两个 TTS 口音（GenAm / RP）仅作为回退选项。
3. **语音包构建集成**：PhonoArk 通过 MSBuild 内联任务在构建时从 `US-Jenny.zip` 解压 WAV 文件，pureopus 需用 Gradle 等价实现。
4. **音标布局与交互差异**：音标按钮大小、排列方式、点击行为、详情面板定位存在差异需对齐。
5. **资源缺失**：pureopus 缺少自定义 App 图标（`ic_launcher`）和开屏大图（`logo.jpg`），需从 PhonoArk.Android 移植。
6. **考试页泄题**：pureopus 考试选项按钮同时显示单词和 IPA 音标转写，等同于开卷考试；PhonoArk 只显示单词。
7. **考试播放按钮反馈**：playPhoneme 按钮缺乏明显的按下视觉效果。

---

## 差异总览

| 批次 | 主题 | 差异数 | 优先级 | 影响面 |
|:---:|------|:---:|:---:|------|
| 1 | 音频服务抽象层 + 语音包构建 | 4 | 🔴 高 | IPA / Exam / Settings 全局 |
| 2 | IPA Chart 功能 + 收藏重构 | 4 | 🔴 高 | IPA Chart + 导航 |
| 3 | IPA Chart 布局与交互对齐 | 4 | 🔴 高 | IPA Chart 页面 UI |
| 4 | Exam 行为与显示对齐 | 4 | 🔴 高 | Exam 页面 |
| 5 | Settings 功能补齐 | 3 | 🟡 中 | Settings 页面 |
| 6 | History 页面增强 | 2 | 🟢 低 | History 页面 |
| 7 | 资源与启动打磨 | 3 | 🔴 高 | 全局 / 启动 / 图标 |

---

## 批次 1：音频服务抽象层 + 语音包构建（功能 + 构建）

### 现状
pureopus 的 TTS 调用散布在 `IpaChartViewModel`、`ExamViewModel`、`SettingsViewModel` 三处，各自持有 `TextToSpeech?` 实例。
PhonoArk 则有统一的 `AudioService`，封装了 TTS 播放、本地 WAV 播放、口音切换、音量控制、停止、诊断。

### 核心背景
**安卓上因中国手机的自动 TTS 引擎不可用或表现很差，真人语音（USJenny WAV）是默认语音**。
- `PlayPhonemeAsync` 逻辑：先查 `VoiceAudioPaths[CurrentAccent]` 是否有映射的 WAV 文件 → 有则用 `MediaPlayer` 播放 → 无则回退到 TTS。
- USJenny 的 WAV 路径映射由 `PhonemeDataService` 在加载 JSON 时自动注入：`Data/Exportfile/US-Jenny/phonemes/phonemesXX.wav` 和 `Data/Exportfile/US-Jenny/words/<word>.wav`。
- GenAm 和 RP 两个口音仅走 TTS，因此在中国手机上可能**不可用**，仅作为回退/可选项。

### 差异项

| # | 差异 | PhonoArk 行为 | pureopus 现状 |
|---|------|-------------|------------|
| 1.1 | **缺少 `USJenny` 口音及 WAV 播放** | `Accent` 枚举含 `GenAm / RP / USJenny`；`USJenny` 对应本地 WAV 真人录音，**是安卓默认口音** | `Accent` 仅有 `GEN_AM / RP`；无本地音频支持 |
| 1.2 | **无集中式 AudioService** | `AudioService` 统一管理 TTS / WAV / 音量 / 口音 / 停止 / 诊断 | TTS 实例和播放逻辑散布在 3 个 ViewModel |
| 1.3 | **默认口音不一致** | `AppSettings.DefaultAccent = USJenny`（真人语音优先，保证中国手机也能出声） | `AppSettings.defaultAccent = GEN_AM`（TTS 语音，中国手机可能不可用） |
| 1.4 | **无语音包构建集成** | `PhonoArk.csproj` 有 `EnsureVoicePackageTask` MSBuild 内联任务：构建前自动从 `Data/US-Jenny.zip` 解压 WAV 到 `Data/Exportfile/US-Jenny/`，SHA256 指纹缓存跳过重复解压，解压后文件作为 `AvaloniaResource` 打入产物 | 无语音包集成机制；WAV 音频文件需手动拷贝或完全缺失 |

### 建议方案
1. 创建 `AudioRepository`（或 `AudioService` 单例），通过 Hilt `@Singleton` 注入。
2. 管理共享的 `TextToSpeech` 实例 + `MediaPlayer`（播放本地 WAV）。
3. 在 `Accent` 枚举增加 `US_JENNY`，配合 assets 下的 WAV 文件。
4. 三个 ViewModel 改为依赖注入 `AudioRepository`，去除各自的 `tts` 字段。
5. 播放逻辑优先级：**WAV 映射 → TTS 回退**（与 PhonoArk 的 `PlayPhonemeAsync` / `PlayWordAsync` 一致）。
6. 将 `AppSettings.defaultAccent` 改为 `US_JENNY`。
7. **语音包构建集成（1.4）**：在 `app/build.gradle.kts` 中添加 Gradle 任务，构建前从 `US-Jenny.zip` 解压到 `app/src/main/assets/US-Jenny/`。

> **PhonoArk 的做法参考**（`PhonoArk.csproj`）：
> - MSBuild 属性：`UsJennyZipPath` → `Data/US-Jenny.zip`；`UsJennyExtractRoot` → `Data/Exportfile/US-Jenny`
> - `EnsureVoicePackageTask`：计算 ZIP 内文件列表的 SHA256 指纹 → 与 `US-Jenny.manifest.json` 对比 → 不匹配时清空旧目录并重新解压 → 还处理了嵌套根目录自动拍平
> - `Target BeforeTargets="PrepareForBuild"`：解压 → `ItemGroup Include` 被解压的文件
> - `CopyUsJennyToOutput`、`CopyUsJennyToPublish`：构建/发布后复制到输出目录
>
> **pureopus 的 Gradle 等价方案**：
> ```kotlin
> // app/build.gradle.kts
> val voiceZip = rootProject.file("../PhonoArk/PhonoArk/Data/US-Jenny.zip")
> val voiceAssetsDir = file("src/main/assets/US-Jenny")
> 
> tasks.register<Copy>("extractVoicePack") {
>     from(zipTree(voiceZip))
>     into(voiceAssetsDir)
>     // 如果 zip 内有嵌套根目录，可用 eachFile { relativePath = RelativePath(true, *relativePath.segments.drop(1).toTypedArray()) }
>     onlyIf { voiceZip.exists() && (!voiceAssetsDir.exists() || voiceAssetsDir.listFiles()?.isEmpty() != false) }
> }
> tasks.named("preBuild") { dependsOn("extractVoicePack") }
> ```
> 如果 ZIP 文件不在本地（例如 CI），可降级为直接拷贝全部 WAV 到 `assets/US-Jenny/`。
>
> 同时需要在 pureopus 的 `Phoneme` 和 `ExampleWord` 数据模型中增加 `voiceAudioPaths: Map<String, String>` 字段，并在 JSON 加载逻辑中自动注入路径映射。

---

## 批次 2：IPA Chart 功能 + 收藏重构（功能 + 架构）

### 核心背景
**PhonoArk.Android 已移除独立的"收藏"标签页**，收藏功能完全内嵌于 IPA Chart 页面中：
- IPA Chart 顶部有批量收藏按钮行（★ 元音 / 双元音 / 辅音 / 清除全部）
- 每个音标按钮有 `IsFavorite` 边框高亮
- 详情面板右上角有单个收藏切换星标
- **不存在独立的 Favorites 页面和 Favorites 导航项**

pureopus 当前有独立的 `FavoritesScreen` + `FavoritesViewModel` + 底部导航第 3 项"收藏"。

### 差异项

| # | 差异 | PhonoArk 行为 | pureopus 现状 |
|---|------|-------------|------------|
| 2.1 | **独立 Favorites 页面应移除** | 无独立 Favorites 页面/标签；收藏管理完全在 IPA Chart 内 | 有独立 `FavoritesScreen` + `FavoritesViewModel`，底部导航含第 3 项 |
| 2.2 | **导航栏 5→4 项** | 底部导航 4 项：IPA / Exam / History / Settings | 底部导航 5 项：IPA / Exam / **Favorites** / History / Settings |
| 2.3 | **无批量收藏/取消收藏** | 每个分区(元音/双元音/辅音)有"一键全选收藏"切换按钮；另有"清空所有收藏"按钮 | 仅有单个音素的收藏切换 |
| 2.4 | **选中音素不自动播放** | `SelectPhonemeAsync` 末尾调用 `PlayPhonemeAsync(phoneme)` | `selectPhoneme` 仅更新 UI 状态，不发声 |

### 建议方案
- 2.1 + 2.2：
  - 删除 `FavoritesScreen.kt` 和 `FavoritesViewModel.kt`
  - 从 `AppNavigation.kt` 中移除 `Screen.Favorites` 和 `bottomNavItems` 中的 Favorites 项
  - 导航从 5 项变为 4 项：IPA / Exam / History / Settings
- 2.3：
  - 在 `IpaChartUiState` 增加 `allVowelsFavorited` / `allDiphthongsFavorited` / `allConsonantsFavorited` 布尔值和每个 phoneme 的 `isFavorite` 标记
  - ViewModel 添加 `toggleFavoriteBatch(type)` 和 `clearAllFavorites()` 方法
  - Screen 在音标列表上方添加批量收藏按钮行
  - 音标按钮根据 `isFavorite` 显示金色边框
- 2.4：在 `selectPhoneme()` 末尾调用播放（依赖批次 1 的 AudioRepository 更好，但不阻塞，可先用现有 TTS）

---

## 批次 3：IPA Chart 布局与交互对齐（UI）

### 核心背景
pureopus 和 PhonoArk.Android 在音标按钮的**大小、形状、排列方式、点击行为**上存在差异。

### 差异项

| # | 差异 | PhonoArk 行为 | pureopus 现状 |
|---|------|-------------|------------|
| 3.1 | **音标按钮尺寸与形状** | 移动端样式 `MinWidth=72, MinHeight=60, FontSize=26, Padding=14,10`，圆角矩形（`CornerRadius` 由 `flutter-tile` 样式控制） | `CircleShape`, `size=48.dp`, `fontSize=18.sp` — 更小的圆形按钮 |
| 3.2 | **音标排列容器** | `WrapPanel` + `Margin=5`，每个音标间距 5dp，在 Border(flutter-card) 内自然换行 | `FlowRow` + `spacedBy(8.dp)`，8dp 间距，无外层 Card 包裹 |
| 3.3 | **无 IsPlaying 状态指示** | 播放示例词时该词显示"播放中"视觉反馈 (`IsPlaying = true`)，切换时清除 | 无播放状态变化的视觉反馈 |
| 3.4 | **详情面板未叠加在音标区** | 移动端竖屏：`Grid RowDefinitions("*,*")`，音标列表占上半屏、详情面板占下半屏，**同屏可见**；横屏：`ColumnDefinitions("1.15*,0.85*")`，左右分屏。切换音标时列表保持可见，无需来回滚动 | 详情面板位于整个单列 Column 底部，需滚过所有元音+双元音+辅音才能看到单词详情，切换音标不方便 |

### 建议方案
- 3.1：将音标按钮改为圆角矩形（`RoundedCornerShape(12.dp)`），增大尺寸到 `minWidth=68.dp, minHeight=56.dp`，字号增到 `22.sp`，更接近 PhonoArk 安卓端的视觉效果。
- 3.2：在各分区（元音/双元音/辅音）外加 `Card` 包裹，与 PhonoArk 的 `Border.flutter-card` 一致；在 Card 内部添加分类标题。
- 3.3：在 `IpaChartUiState` 增加 `playingWordId` 或类似标记；Screen 中对正在播放的词加高亮/动画。
- 3.4：**将当前单列滚动布局改为分屏布局**。参考 PhonoArk 的 `IpaChartView.axaml.cs`：
  - 竖屏（`configuration.orientation == Portrait`）且有选中音素时：上半屏 = 音标列表（可滚动），下半屏 = 详情面板（含大音标 + 单词列表）。比例约 1:1。
  - 横屏且有选中音素时：左 `1.15fr` = 音标列表，右 `0.85fr` = 详情面板。
  - 无选中音素时：音标列表全屏。
  - Compose 实现方式：替换外层 `Column` 为 `BoxWithConstraints` 或根据 `LocalConfiguration.current.orientation` 动态切换 `Row` / `Column` 容器。详情面板用 `AnimatedVisibility` 展示/隐藏。

---

## 批次 4：Exam 行为与显示对齐（功能 + UI）

### 差异项

| # | 差异 | PhonoArk 行为 | pureopus 现状 |
|---|------|-------------|------------|
| 4.1 | **开始考试时不自动播放第一题** | `StartExamAsync` 末尾调用 `PlayCurrentQuestionAsync()` | `startExam()` 仅更新状态，用户需手动点"播放"才能听到第一题 |
| 4.2 | **选项交互禁用方式不同** | `AreOptionsInteractive = false` 答题后立刻锁死选项，防止 1.2 秒内重复点击 | 仅通过 `isAnswered` 过滤，但 Compose Button `enabled = !state.isAnswered` 已实现等价效果 |
| 4.3 | **考试选项泄露 IPA 音标** | 选项按钮只显示单词文本（`ExamDisplayWord` = `Word`），**不显示 IPA 转写**，因为本身就是考音标辨识 | 选项按钮同时显示 `option.word` + `option.ipaTranscription`（等同开卷考试，暴露了正确答案的音标线索） |
| 4.4 | **播放音素按钮按下效果不明显** | "播放音素" 按钮有明显的 `pressed` 样式（PhonoArk 的 `flutter-primary` 按钮类含 `:pressed` 视觉变化：背景加深 + 轻微缩放） | 标准 Material3 `Button`，按下反馈不够醒目，尤其在考试场景中用户频繁点击时难以确认是否已触发 |

### 说明
- 4.1 是体验差异：PhonoArk 开始考试 → 自动播放 → 用户直接选答案；pureopus 开始考试 → 用户需先点播放按钮。修复简单：`startExam()` 末尾加 `playCurrentPhoneme()`。
- 4.2 功能等价，仅记录，无需修改。
- 4.3 **是严重的功能 bug**：考试是考"听音标 → 选出含该音标的单词"，如果选项里直接显示 IPA 转写，用户可以直接比对音标字符找到答案，失去考试意义。修复：在 `ActiveExam` composable 的选项按钮中，**移除 `Text(text = option.ipaTranscription, ...)`**，只保留 `Text(text = option.word, ...)`。
- 4.4：给"播放音素"按钮增加按下态反馈。方案：
  - 方法一：使用 `Modifier.indication()` + `rememberRipple(bounded = true, radius = 28.dp)` 增大涟漪范围。
  - 方法二：用 `Modifier.pointerInput` 检测按下/抬起，配合 `animateColorAsState` 做背景色加深动画。
  - 方法三（最简单）：增大按钮尺寸（如 `fillMaxWidth(0.7f)`, `height(56.dp)`），让按钮更醒目，Material3 默认涟漪更容易看到。

---

## 批次 5：Settings 功能补齐（功能 + UI）

### 差异项

| # | 差异 | PhonoArk 行为 | pureopus 现状 |
|---|------|-------------|------------|
| 5.1 | **缺少 USJenny 口音选项** | Settings 下拉有 3 个选项：US-Jenny（默认）/ American / British | 仅 American / British 两个选项 |
| 5.2 | **缺少版本号与 GitHub 链接** | Settings 页面显示 `AppVersion` + 可点击 GitHub 链接 | 无版本信息，无 GitHub 入口 |
| 5.3 | **口音实时同步缺失** | Settings 切换口音 → `AudioService.CurrentAccent` 触发 `AccentChanged` 事件 → IPA Chart ViewModel 自动更新 `CurrentAccent` | pureopus Settings 保存口音到 DB，但 IpaChartViewModel 仅在 `init` 时读取，不会实时同步 |

### 建议方案
- 5.1：依赖批次 1（增加 USJenny 枚举后自然可选）。口音选择应标注"推荐"于 USJenny（因为 TTS 在中国手机可能不可用）。
- 5.2：在 Settings 卡片底部增加版本号展示 + GitHub 按钮（`Intent.ACTION_VIEW`）。
- 5.3：通过 `SettingsRepository` 暴露 `Flow<Accent>` 或用 SharedFlow 事件；IpaChartViewModel 和 ExamViewModel collect 该 Flow 以实时更新。

---

## 批次 6：History 页面增强（UI）

### 差异项

| # | 差异 | PhonoArk 行为 | pureopus 现状 |
|---|------|-------------|------------|
| 6.1 | **Session 卡片样式简化** | 每个 Session 下显示彩色小卡片列 (✓ 绿 / ✗ 红)，每卡片含题号 + 音标 + 结果符号；可点击展开单题详情 | 仅显示分数比例 (如 `7/10`)，点击弹出 Dialog 展示整个 session 的 attempts 列表 |
| 6.2 | **无分页/懒加载** | ExamHistoryViewModel 实现分页懒加载：首次加载 2 条 session，后续每次加 3 条；有 `HasMoreSessions` / `LoadMoreSessionsAsync` | pureopus 一次性加载全部历史记录 |

### 建议方案
- 6.1：可保留当前 Dialog 交互（安卓原生惯例），但丰富 SessionCard 内容：增加小彩点行显示各题对错一览。
- 6.2：对 `HistoryRepository.getAllResults()` 添加 `LIMIT/OFFSET` 分页参数；ViewModel 维护 `hasMore` 状态和 `loadMore()` 方法。

---

## 批次 7：资源与启动打磨（资源 + 体验）

### 核心背景
pureopus 缺少自定义应用图标和开屏图片，使用的是 Android 默认图标。PhonoArk.Android 有自定义的应用图标（`Icon.png`）和全屏开屏大图（`Assets/logo.jpg`），以及 `SplashActivity`（ImageView 全屏裁剪展示，停留 300ms 后跳转 MainActivity）。

### 差异项

| # | 差异 | PhonoArk 行为 | pureopus 现状 |
|---|------|-------------|------------|
| 7.1 | **无自定义应用图标** | `Icon.png`（项目根目录），`mipmap-anydpi-v26/ic_launcher.xml` 引用 `@drawable/icon` | 使用 Android 默认绿色图标（`ic_launcher_foreground.xml` 为 vector） |
| 7.2 | **无开屏大图** | `Assets/logo.jpg`，`SplashActivity` 用 `ImageView.ScaleType.Matrix` 做中心裁剪展示 | 无 SplashActivity，直接进入 MainActivity |
| 7.3 | **无 SplashActivity** | 独立 `SplashActivity`：加载 `logo.jpg`、等待 300ms、跳转 `MainActivity`；支持横竖屏裁剪 | 直接启动 `MainActivity` |

### 建议方案
- 7.1：将 PhonoArk.Android 的 `Icon.png` 拷贝至 pureopus 的 `app/src/main/res/` 下，生成各分辨率 mipmap（或直接用 `mipmap-anydpi-v26` + 自适应图标）。修改 `ic_launcher.xml` 引用新前景图。
- 7.2 + 7.3：将 `logo.jpg` 拷贝到 pureopus 的 `assets/` 或 `drawable` 下；创建 `SplashActivity.kt`，用 `ImageView` + `Matrix` 实现全屏裁剪展示，停留 300ms 后 `startActivity(MainActivity)` + `finish()`。AndroidManifest 中将 SplashActivity 设为 `<intent-filter>` 启动入口。

---

## 已确认无需修改的差异

| 差异 | 理由 |
|------|------|
| Room vs EF Core | 平台适配，等价 |
| Flow vs 事件驱动 | Kotlin 惯用法 |
| Volume `Int (0-100)` vs `double (0.0-1.0)` | 语义等价，TTS 端都转为 `0.0f~1.0f` |
| SettingsEntity 含 language 字段 | 统一管理更简洁 |
| 底部导航 vs 侧边栏 | 安卓用底部导航是原生惯例 |
| ExamQuestionAttempt 缺少 ExamDate | 可通过 JOIN 获取 |

---

## 建议实施顺序

```
批次 7 (图标/开屏)  ──→  批次 1 (音频+语音包)  ──→  批次 2 (IPA + 收藏重构)
       │                                                      │
       │                 批次 3 (IPA 布局+分屏)  ←────────────┘
       │                        │
       │                        └──→  批次 4 (Exam 修复)  ──→  批次 5 (Settings)
       │                                                            │
       └─── 批次 4.3 (去IPA, 独立可做) ←──── 可提前 ────────────────┘
                                                                    │
                                                                    └──→  批次 6 (History)
```

**依赖关系说明**：
- **批次 7（图标/开屏）** 无依赖，可最先做，立竿见影。
- **批次 1（音频+语音包）** 是批次 2/4/5 的基础：统一 AudioRepository + Gradle 解压语音包后，IPA 自动播放、Exam 首题播放、Settings 口音切换才能一致。
- **批次 2（收藏重构）** 必须在批次 3（布局）之前完成：因为要删掉 Favorites 标签页 + 在 IPA Chart 页添加批量收藏 UI。
- **批次 4.3（去掉选项 IPA 音标）** 是独立的 bug 修复，只需删一行代码，**可以立即做**，不依赖任何批次。
- **批次 4.4（播放按钮效果）** 和 **批次 3.4（详情面板分屏）** 是 UI 调整，独立于架构改动。
- 批次 4.1（Exam 首题播放）和批次 2.4（选中自动播放）可独立于批次 1 先做（直接用现有 TTS），但建议等 AudioRepository 就绪后一起做更干净。
- `ExampleWord` 需补 `voiceAudioPaths` 字段在批次 1 完成时一并处理。
- 批次 5.1（USJenny 选项）依赖批次 1（AudioRepository + Accent 枚举扩展）。
- 批次 5、6 独立，随时可做。

---

## 工时估算

| 批次 | 预估文件变更数 | 预估耗时 |
|:---:|:---:|:---:|
| 1（音频+语音包） | 10-12 | 1.5-2.5 小时 |
| 2（IPA+收藏重构） | 3-4 | 30-45 分钟 |
| 3（IPA布局+分屏） | 2-3 | 30-45 分钟 |
| 4（Exam修复） | 2-3 | 20-30 分钟 |
| 5（Settings） | 3-4 | 45-60 分钟 |
| 6（History） | 2-3 | 20-30 分钟 |
| 7（资源/图标） | 5-6 | 15-20 分钟 |
| **合计** | **~30** | **~5-6 小时** |
