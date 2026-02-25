## 2026-02-18 22:46:49

- 修复 `Views/IpaChartView.axaml` 中列表项按钮命令绑定：改为通过根节点 `#Root.DataContext` 访问 `SelectPhonemeCommand` 与 `PlayWordCommand`，避免模板内父级路径绑定失效。
- 新增 `Services/LocalizationService.cs`，提供基于 `resx` 的语言资源读取、文化设置与绑定刷新能力。

### 实现目标
### 验证结果（可选）
- 执行 `dotnet build PhonoArk/PhonoArk.sln`：构建成功（0 Error）。

## 2026-02-24 18:29:45

### 实现目标
- 修复“公共样式互相影响”导致 PC 与 Android 答题选项都出现小自适应格子的问题。
- 明确分离 PC 与 Android 答题区 4 个答案的宽度策略，并与各自反馈区边界对齐。

### 变更内容
- 更新 `PhonoArk/PhonoArk/Views/ExamView.axaml`：
	- `Button.exam-option` 恢复为 PC 默认固定宽度（`560`），作为桌面基线。
	- Android 样式改为根节点定向选择器 `#Root.mobile Button.exam-option` 与 `#Root.phone-landscape Button.exam-option`，仅在移动端覆盖为动态宽度（`Width=NaN` + `Stretch`）。
	- 移动端维持圆角并保留左右内边距（竖屏 `20`、横屏 `16`），避免“铺满过宽”观感。
- 保持 `ExamView.axaml.cs` 中移动端反馈区边距 `20,6,20,0`（桌面固定宽度 `560`），实现“选项区与反馈区同边界”。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk/PhonoArk.sln`：构建成功（0 Error）。

## 2026-02-24 18:03:26

### 实现目标
- 恢复到预期效果：Android 下备选答案与反馈答案区域同宽，并保持圆角卡片观感。
- 修复“样式已改但界面仍顶满”问题（样式优先级冲突）。

### 变更内容
- 更新 `PhonoArk/PhonoArk/Views/ExamView.axaml`：
	- 撤销安卓 `200` 固定宽试验，恢复为基于父容器的动态宽度（`Width=NaN` + `HorizontalAlignment=Stretch`）。
	- 安卓（含横屏）选项按钮统一左右边距为 `12`，圆角统一为 `18`。
	- 移除答题按钮模板中的本地 `HorizontalAlignment=Stretch`，避免覆盖平台样式导致“改了不生效”。
- 更新 `PhonoArk/PhonoArk/Views/ExamView.axaml.cs`：
	- 安卓横/竖屏下 `FeedbackBorder.Margin` 统一为 `12,6,12,0`，与选项按钮共享同一水平边界。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk/PhonoArk.sln`：构建成功（0 Error）。

## 2026-02-24 17:29:53

### 实现目标
- 按用户要求进行安卓答题按钮视觉试验：将备选答案固定宽度改为 `200`，先观察实际效果。

### 变更内容
- 更新 `PhonoArk/PhonoArk/Views/ExamView.axaml`：
	- `UserControl.mobile Button.exam-option` 改为固定 `Width/MinWidth/MaxWidth = 200`，`HorizontalAlignment=Center`。
	- `UserControl.phone-landscape Button.exam-option` 同步改为固定 `200` 宽并居中。
	- Windows 默认样式不变，仅影响安卓移动端样式。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk/PhonoArk.sln`：构建成功（0 Error）。

## 2026-02-24 17:23:10

### 实现目标
- 移除 Android 考试答题区固定 `560` 宽限制。
- 让备选答案格子和反馈框都按上一层容器动态计算宽度，保持一致边界与圆角观感。

### 变更内容
- 更新 `PhonoArk/PhonoArk/Views/ExamView.axaml.cs`：
	- Android 横屏/竖屏下 `ExamAnswerContentPanel.MaxWidth` 改为 `double.PositiveInfinity`。
	- Android 横屏/竖屏下 `ExamAnswerContentPanel.HorizontalAlignment` 改为 `Stretch`。
	- Android 横屏/竖屏下 `ExamAnswerScroll.HorizontalAlignment` 改为 `Stretch`。
	- 反馈框保持 `Width=NaN` 与 `Margin=0,6,0,0`，与备选答案共享同一父容器宽度逻辑。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk/PhonoArk.sln`：构建成功（0 Error）。

## 2026-02-24 17:15:46

### 实现目标
- 在 Android 下为考试页 4 个备选答案提供独立尺寸策略，避免过长导致“长方形感”过强。
- 让 Android 下备选答案格子与答案反馈格子保持一致宽度边界，并增强圆角观感。

### 变更内容
- 更新 `PhonoArk/PhonoArk/Views/ExamView.axaml`：
	- `UserControl.mobile Button.exam-option` 与 `UserControl.phone-landscape Button.exam-option`：
		- 间距改为 `Margin=0,4`（与反馈框同一左右边界）。
		- 圆角改为 `CornerRadius=16`，提升卡片感。
- 更新 `PhonoArk/PhonoArk/Views/ExamView.axaml.cs`：
	- Android 横屏/竖屏下将 `ExamAnswerContentPanel.MaxWidth` 统一为 `560`，限制选项区域不再拉得过长。
	- Android 横屏/竖屏下 `FeedbackBorder.Margin` 改为 `0,6,0,0`，与选项格子保持同宽对齐。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk/PhonoArk.sln`：构建成功（0 Error）。

## 2026-02-24 16:58:42

### 实现目标
- 修正考试页在 Windows 下 4 个备选答案间距过小的问题。
- 修正 Android 下“备选答案区域”和“答案反馈区域”宽度不一致的问题，使两者边界一致。

### 变更内容
- 更新 `PhonoArk/PhonoArk/Views/ExamView.axaml`：
	- `Button.exam-option` 默认增加纵向间距（Windows 桌面可读性提升）。
	- 移除选项按钮模板中的硬编码 `Margin=0`，改由平台样式控制。
	- 移动端 `exam-option` 间距统一为 `10,4`，保持左右留白一致。
	- 反馈区域 `FeedbackBorder` 默认改为自适应宽度（`Width=NaN`）。
- 更新 `PhonoArk/PhonoArk/Views/ExamView.axaml.cs`：
	- 桌面模式保持 `FeedbackBorder.Width=560` 且边距为 `0,6,0,0`。
	- Android 竖屏/横屏下统一设置 `FeedbackBorder.Margin=10,6,10,0`，与选项按钮左右边界一致。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk/PhonoArk.sln`：构建成功（0 Error）。

## 2026-02-24 16:45:51

### 实现目标
- 让历史记录标题语义与倒序展示一致，避免“第一次/第二次”阅读歧义。
- 修复 Android 下 IPA 详情区与考试“考核音标”显示字体不一致、部分音标缺字问题。

### 变更内容
- 更新 `PhonoArk/PhonoArk/Resources/Strings.zh-CN.resx` 与 `Strings.resx`：
	- `ExamSessionTitleTemplate` 改为强调“最近”语义（最新考试在最上）。
- 更新 `PhonoArk/PhonoArk/Views/IpaChartView.axaml`：
	- 详情区大音标移除局部字体覆盖，统一走全局 IPA 字体类，避免与音标列表字体链不一致。
	- 大音标字重改为 `Normal`，提升 IPA 符号字形命中率。
- 更新 `PhonoArk/PhonoArk/Views/ExamView.axaml`：
	- 竖屏/横屏“考核音标”文本显式指定 IPA 友好字体链，并将字重改为 `Normal`。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk/PhonoArk.sln`：构建成功（0 Error）。

## 2026-02-24 16:29:12

### 实现目标
- 让 IPA 音标区及已读取示例词在页面切换后保持常驻，减少重复加载等待。
- 保证历史考试结果倒序输出，最近一次考试固定展示在最上方。

### 变更内容
- 更新 `PhonoArk/PhonoArk/ViewModels/IpaChartViewModel.cs`：
	- 调整 `PrepareForDisplay()` 行为：切回 IPA 页时仅清理播放态，不再清空 `SelectedPhoneme` 与已加载词表。
	- 已加载过的单词集合继续挂在对应 `Phoneme.ExampleWords` 上，后续切换页签不重复拉取。
- 更新 `PhonoArk/PhonoArk/Services/ExamHistoryService.cs`：
	- `GetExamResultIdPageAsync` 改为按 `Id` 倒序主排，确保最新考试结果优先返回并显示在历史列表顶部。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk/PhonoArk.sln`：构建成功（0 Error）。

## 2026-02-23 15:07:56

### 实现目标
- 推进 Android 端追平桌面版设计风格，并按手机交互特性进行布局适配。

### 变更内容
- 更新 `PhonoArk/PhonoArk/App.axaml.cs`：
	- 在单视图平台（Android）初始化 `MainView` 时添加 `mobile` 类标记。
- 更新 `PhonoArk/PhonoArk/Views/MainView.axaml`：
	- 为主布局新增命名节点，支持移动端运行时布局重排。
	- 导航区改为可横向滚动容器，保留桌面视觉样式但适配手机触控与窄屏。
	- 内容区改为全宽显示，避免双栏在手机上挤压。
- 更新 `PhonoArk/PhonoArk/Views/MainView.axaml.cs`：
	- 新增移动端布局应用逻辑：切换为 `Auto,*` 两行布局、顶部导航 + 下方内容区。
	- 设置导航横向滚动与横向排布，提升 Android 小屏可用性。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk/PhonoArk.sln`：构建成功（0 Error）。

## 2026-02-23 15:16:56

### 实现目标
- 为 Android 端接入 TTS，追平桌面端“点击即发音”的能力。
- 保持核心音频服务共用，避免 Android/Desktop 各写一套业务逻辑。

### 变更内容
- 更新 `PhonoArk/PhonoArk/Services/AudioService.cs`：
	- 新增平台语音扩展点（播放/停止/诊断委托注册），Windows 继续使用既有 `System.Speech` 主路径。
	- 非 Windows 平台可通过注册处理器复用同一 `AudioService` 调用链。
- 新增 `PhonoArk/PhonoArk.Android/Services/AndroidTtsBridge.cs`：
	- 基于 Android `TextToSpeech` 实现播放、停止与语音诊断。
	- 支持按口音切换优先语言（RP=en-GB，GenAm=en-US），并回退到英语语音。
- 更新 `PhonoArk/PhonoArk.Android/MainActivity.cs`：
	- 启动时注册 Android TTS 桥接到共享 `AudioService`。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk/PhonoArk.sln`：构建成功（0 Error，含 `PhonoArk.Android`）。

## 2026-02-23 15:23:32

### 实现目标
- 优化设置页语音诊断展示：增加平台标识与关键语音信息，提升 Android/Desktop 可读性。

### 变更内容
- 更新 `PhonoArk/PhonoArk/ViewModels/SettingsViewModel.cs`：
	- 新增诊断字段：平台、目标语言、当前语音、语音条目数。
	- `RunVoiceDiagnosticsAsync` 填充结构化诊断信息。
- 更新 `PhonoArk/PhonoArk/Views/SettingsView.axaml`：
	- 语音诊断区域改为“摘要 + 关键字段 + 详情折叠列表”结构。
- 更新 `PhonoArk/PhonoArk/Resources/Strings.resx` 与 `Strings.zh-CN.resx`：
	- 新增诊断展示文案键（平台、目标语言、当前语音、条目数、语音详情）。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk/PhonoArk.sln`：构建成功（0 Error）。

## 2026-02-23 15:37:39

### 实现目标
- 修改一键脚本以适配当前 PhonoArk 安卓项目。
- 支持复用共享 AVD（包含原有 ReadStorm 虚拟机）进行统一测试。

### 变更内容
- 更新 `PhonoArk/scripts/android-dev-oneclick.ps1`：
	- 默认项目改为 `PhonoArk.Android/PhonoArk.Android.csproj`。
	- 默认包名改为 `com.CompanyName.PhonoArk`。
	- 新增共享 AVD 候选参数 `SharedAvdCandidates`，默认包含 `ReadStorm_API34`、`PhonoArk_API34` 等。
	- 新增 `Resolve-TargetAvdName`：优先使用显式 `-AvdName`，否则自动从共享候选中匹配，最后回退到本机第一个可用 AVD。
	- 桌面发布路径改为 `PhonoArk.Desktop/PhonoArk.Desktop.csproj`。
	- APK 输出目录改为 `PhonoArk.Android/bin/<Configuration>/net10.0-android`。
	- 交付 APK 命名改为 `PhonoArk-<configuration>.apk`。

### 验证结果（可选）
- 对脚本执行 PowerShell 语法解析：通过（无语法错误）。

## 2026-02-23 16:04:22

### 实现目标
- 修复 Android 端字体显示异常，改用真机常见字体策略。
- 通过一键脚本完成安装启动联调验证。

### 变更内容
- 更新 `PhonoArk/PhonoArk.Android/MainActivity.cs`：
	- 移除 Android 端 `WithInterFont()` 强制字体注入，改为优先使用系统字体（Roboto/Noto）。
- 更新 `PhonoArk/PhonoArk/Views/MainView.axaml`：
	- 导航图标从 Windows 私有字体码位（Segoe Fluent Icons）切换为跨平台可显示 Emoji 图标。
- 更新 `PhonoArk/PhonoArk/App.axaml`：
	- `flutter-icon` 字体族改为系统通用链：`sans-serif, Noto Sans CJK SC, Microsoft YaHei, Segoe UI Emoji`。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk/PhonoArk.sln`：构建成功（0 Error）。
- 执行 `PhonoArk/scripts/android-dev-oneclick.ps1`：构建、安装、启动均成功，日志未发现关键错误。

## 2026-02-23 16:12:41

### 实现目标
- 优化手机端 IPA 页面操作逻辑：点击音标后，详情与示例词在移动端可见区域稳定展示。
- 提升移动端触控尺寸与滚动体验，减少“点了没看到结果/不跟手”的感受。

### 变更内容
- 更新 `PhonoArk/PhonoArk/Views/IpaChartView.axaml`：
	- 新增移动端样式：放大 `flutter-tile` 与 `flutter-word` 的触控尺寸。
	- 为主内容区、列表区、详情区添加命名节点与类名，配合代码后置重排。
- 更新 `PhonoArk/PhonoArk/Views/IpaChartView.axaml.cs`：
	- 在单视图平台（Android）自动添加 `mobile` 类。
	- 在移动端将 IPA 主区重排为上下布局（上：音标列表；下：详情/占位），并设置列表滚动行为。
	- 解决 Avalonia 对 `ColumnDefinitions/RowDefinitions` 样式 setter 的限制，改为代码配置，避免运行时失效。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk/PhonoArk.sln`：构建成功（0 Error）。
- 执行 `PhonoArk/scripts/android-dev-oneclick.ps1`：构建、安装、启动成功，日志无关键错误。

## 2026-02-23 19:45:14

- 修复 Android 端 IPA 页面“无可点击音标、点击后无单词”问题。

- 更新 `PhonoArk/PhonoArk/Services/PhonemeDataService.cs`：
	- 词库加载改为跨平台双通道：
		- 优先读取磁盘路径 `AppContext.BaseDirectory/Data/phoneme-word-bank.json`
		- 若不存在则回退读取内嵌资源 `avares://PhonoArk/Data/phoneme-word-bank.json`
	- 解决 Android 打包后文件路径不可用导致音标列表为空的问题。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk/PhonoArk.sln`：构建成功（0 Error）。
- 执行 `PhonoArk/scripts/android-dev-oneclick.ps1`：构建、安装、启动成功，日志无关键错误。

## 2026-02-23 20:32:39

### 实现目标
- 完成移动端导航排版重构（优先手机操作逻辑）。
- 完成 IPA 字体覆盖专项修复，减少音标缺字/空白方块。

### 变更内容
- 更新 `PhonoArk/PhonoArk/Views/MainView.axaml`：
	- 移动端导航改为更紧凑的 Tab 风格参数（按钮宽度/间距/字号下调）。
	- 移动端隐藏品牌副标题与“单词学习（即将上线）”入口，减少顶部拥挤。
	- 移动端导航图标文本隐藏，仅保留标签，提升可读性与触达效率。
- 更新 `PhonoArk/PhonoArk/Views/MainView.axaml.cs`：
	- 移动端导航滚动条禁用，避免顶部出现不必要滚动条痕迹。
	- 同步收紧导航区内边距与间距。
- 更新 `PhonoArk/PhonoArk/App.axaml`：
	- 新增 IPA 专用字体样式（符号按钮、符号文本、转写文本）并配置跨平台回退字体链。
- 更新 `PhonoArk/PhonoArk/Views/IpaChartView.axaml`：
	- 音标按钮添加 `ipa-symbol-button` 类。
	- 详情音标与单词 IPA 转写文本添加专用类，统一使用 IPA 字体链。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk/PhonoArk.sln`：构建成功（0 Error）。
- 执行 `PhonoArk/scripts/android-dev-oneclick.ps1 -PackageOnly:$false`：构建、安装、启动成功，日志无关键错误。

## 2026-02-23 14:31:35

### 实现目标
- 修复考试中选错后由红色快速回退灰色的问题。

### 变更内容
- 更新 `PhonoArk/PhonoArk/Views/ExamView.axaml`：
	- 为考试选项恢复 `Classes.correct` / `Classes.wrong` 状态类绑定。
- 更新 `PhonoArk/PhonoArk/App.axaml`：
	- 调整 `examoption` 基础 hover/pressed 规则，仅对中性态生效。
	- 正确/错误状态继续使用更高优先级规则，确保 hover/pressed/disabled 下红绿不回灰。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk/PhonoArk.sln`：构建成功（0 Error）。

## 2026-02-23 14:26:33

### 实现目标
- 修复考试中“答对/答错后颜色很快从绿/红回退为灰色”的问题。
- 修复历史答题卡仍显示灰色的问题。

### 变更内容
- 更新 `PhonoArk/PhonoArk/Models/ExampleWord.cs`：
	- 新增显式颜色属性 `ExamBackground/ExamBorderBrush/ExamForeground`。
	- 状态变化时同步触发颜色属性通知，确保 UI 立即刷新。
- 更新 `PhonoArk/PhonoArk/Views/ExamView.axaml`：
	- 考试选项改为单按钮并直接绑定颜色属性，避免样式类优先级导致颜色回退。
- 更新 `PhonoArk/PhonoArk/ViewModels/ExamHistoryViewModel.cs`：
	- 为历史答题卡新增显式颜色属性 `CardBackground/CardBorderBrush/CardForeground`。
- 更新 `PhonoArk/PhonoArk/Views/ExamHistoryView.axaml`：
	- 历史答题卡改为单按钮并绑定显式颜色属性，保证正确/错误颜色稳定。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk/PhonoArk.sln`：构建成功（0 Error）。

## 2026-02-23 14:19:20

### 实现目标
- 移除内置词库，改为仅从外部 JSON 读取音标与单词数据。
- 当 JSON 读取失败时提供明确错误提示。

### 变更内容
- 重构 `PhonoArk/PhonoArk/Services/PhonemeDataService.cs`：
	- 删除原有内置大段词库代码。
	- 新实现仅从 `Data/phoneme-word-bank.json` 读取数据。
	- 增加 `LoadError` 属性，返回“文件缺失/格式错误/读取失败”等明确信息。
- 更新 `PhonoArk/PhonoArk/Data/phoneme-word-bank.json`：
	- 新增 `phonemes` 元数据（symbol/type/description），使程序可完全依赖外部词库。
- 更新 `PhonoArk/PhonoArk/Services/ExamService.cs`：
	- 若词库加载失败，抛出明确错误信息供上层展示。
- 更新 `PhonoArk/PhonoArk/ViewModels/ExamViewModel.cs`：
	- 开始考试时捕获词库错误并在界面反馈具体提示。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk/PhonoArk.sln`：构建成功（0 Error）。

## 2026-02-23 14:11:13

### 实现目标
- 将题库扩展为可通过 JSON 外部维护，缓解“撞库”问题。
- 增强考试随机策略，减少同一套题内的音标与单词重复。

### 变更内容
- 更新 `PhonoArk/PhonoArk/Services/PhonemeDataService.cs`：
	- 新增 JSON 词库扩展机制（`Data/phoneme-word-bank.json`）。
	- 维持内置词库兜底，同时从 JSON 加载全局词表并按音标匹配扩展。
	- 支持 `targetWordsPerPhoneme` 目标数量（默认 30），当候选超出目标时随机抽样。
- 新增 `PhonoArk/PhonoArk/Data/phoneme-word-bank.json`：
	- 提供面向初中阶段的较大公共词表（带 IPA），供各音标自动匹配扩充。
- 更新 `PhonoArk/PhonoArk/PhonoArk.csproj`：
	- 将 `Data/phoneme-word-bank.json` 配置为输出复制（`PreserveNewest`）。
- 更新 `PhonoArk/PhonoArk/Services/ExamService.cs`：
	- 出题前打散音标池，优先轮询不同音标。
	- 同一套题中优先避免重复单词（correct/wrong 选项均使用去重集合）。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk/PhonoArk.sln`：构建成功（0 Error）。

## 2026-02-23 13:58:11

### 实现目标
- 彻底修复考试过程和历史答题卡中的灰色覆盖问题。
- 修复历史记录详情浮层/气泡不显示问题。

### 变更内容
- 更新 `PhonoArk/PhonoArk/Models/ExampleWord.cs`：
	- 新增 `IsExamNeutral`，支持考试选项三态渲染（中性/正确/错误）。
- 更新 `PhonoArk/PhonoArk/Views/ExamView.axaml`：
	- 选项区改为三按钮可见性方案，按状态显式切换中性/正确/错误样式。
- 更新 `PhonoArk/PhonoArk/Views/ExamHistoryView.axaml`：
	- 根节点新增 `x:Name="Root"`，修复 `OpenAttemptDetailsCommand` 绑定失效。
	- 答题卡改为双按钮可见性方案（正确/错误），避免类动态切换不稳定导致全灰。
- 更新 `PhonoArk/PhonoArk/ViewModels/ExamHistoryViewModel.cs`：
	- 答题卡文本改为“题号 + 音标 + ✓/✗”，增强历史题目信息可见性。
- 更新 `PhonoArk/PhonoArk/App.axaml`：
	- 强化 `examoption` 与 `examattemptcard` 在 `pointerover/pressed/disabled` 全状态下的红绿显色优先级与不透明度。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk/PhonoArk.sln`：构建成功（0 Error）。

## 2026-02-23 13:46:21

### 实现目标
- 解决考试过程与历史答题卡中红绿反馈被灰色状态覆盖的问题。
- 让历史答题卡直接可见“题目线索”，不再只能看灰色块。

### 变更内容
- 更新 `PhonoArk/PhonoArk/App.axaml`：
	- 调整考试选项类名为 `examoption`，增加 `pointerover/pressed/disabled` 全状态覆盖规则。
	- 对 `examoption.correct/wrong` 在全部交互状态下强制保持红绿显色与不透明度。
	- 调整历史答题卡类名为 `examattemptcard`，并增加 `correct/wrong` 在全部交互状态下的显色覆盖。
- 更新 `PhonoArk/PhonoArk/Views/ExamView.axaml`：
	- 考试选项样式类切换为 `examoption`，匹配新的显色覆盖规则。
- 更新 `PhonoArk/PhonoArk/ViewModels/ExamHistoryViewModel.cs`：
	- 答题卡文本改为 `题号 + 音标 + ✓/✗`，增强题目可见性。
- 更新 `PhonoArk/PhonoArk/Views/ExamHistoryView.axaml`：
	- 答题卡尺寸扩大并改用 `examattemptcard` 样式类，保证红绿状态与题目文本清晰可见。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk/PhonoArk.sln`：构建成功（0 Error）。

## 2026-02-23 13:39:30

### 实现目标
- 修复历史答题卡“全灰”与显色不清晰问题（正确绿、错误红）。
- 提升答题卡信息可见性（可直接看题号并快速定位题目详情）。
- 确保红绿状态在鼠标悬停/按下时不被灰色交互态覆盖。

### 变更内容
- 更新 `PhonoArk/PhonoArk/ViewModels/ExamHistoryViewModel.cs`：
	- 为答题卡项新增 `IsWrong`、`CardText`、`CardToolTip`。
	- 答题卡内容由仅符号改为“题号 + 对错符号”（如 `3 ✗`）。
- 更新 `PhonoArk/PhonoArk/Views/ExamHistoryView.axaml`：
	- 答题卡 `Classes.wrong` 改为绑定显式 `IsWrong`，去除取反绑定不稳定因素。
	- 增加 `ToolTip` 展示题号、音标、用户答案、正确答案、结果。
- 更新 `PhonoArk/PhonoArk/App.axaml`：
	- 增加 `correct/wrong` 在 `:pointerover`、`:pressed` 下的样式覆盖，保证红绿显色稳定。
	- 同步增强历史答题卡 `exam-attempt-card` 的红绿状态覆盖规则。
	- 保持考试选项在反馈阶段的红绿状态优先级，避免被灰色交互态覆盖。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk/PhonoArk.sln`：构建成功（0 Error）。

## 2026-02-23 13:31:37

### 实现目标
- 修复语言切换仍不生效的问题。
- 修复考试题号计数异常（5 题只答到 4 次）。
- 移除考试选项按下灰色逻辑，仅保留作答后的红绿反馈。
- 将历史记录改为“分次答题卡 + 点击浮层详情”，并将错误音标统计改为可折叠。
- 让历史统计在每次考试结束后自动刷新。

### 变更内容
- 更新 `PhonoArk/PhonoArk/Services/LocalizationService.cs`：
	- 扩展语言切换通知（`Item` / `Item[]` / 全量通知），提高索引器绑定刷新可靠性。
- 更新 `PhonoArk/PhonoArk/ViewModels/SettingsViewModel.cs`：
	- 切换语言时刷新口音下拉文案，确保设置页可见内容即时变更。
- 更新 `PhonoArk/PhonoArk/Views/MainView.axaml` 与 `Views/SettingsView.axaml`：
	- 移除硬编码中文文案，改为 `resx` 绑定，避免“看似没切语言”。
- 更新 `PhonoArk/PhonoArk/Resources/Strings.resx` 与 `Strings.zh-CN.resx`：
	- 补充 `AppSubtitle`、`SettingsIntro`、`ExperienceSwitch`、`ExamSessionTitleTemplate`、`ResultCorrect/ResultWrong`、`Close` 等新资源键。
- 更新 `PhonoArk/PhonoArk/ViewModels/ExamViewModel.cs` 与 `Views/ExamView.axaml`：
	- 修复题号与切题索引逻辑（新增 `CurrentQuestionNumber`）。
	- 保持作答状态红绿反馈，避免按下态干扰。
- 更新 `PhonoArk/PhonoArk/App.axaml`：
	- 新增 `flutter-secondary.exam-option:pressed` 样式，屏蔽考试按钮按下灰色效果。
	- 新增答题卡样式 `exam-attempt-card` 及红绿状态样式。
- 更新 `PhonoArk/PhonoArk/Services/ExamHistoryService.cs`：
	- 新增 `HistoryChanged` 事件，在保存/删除/清空历史后触发。
- 更新 `PhonoArk/PhonoArk/ViewModels/ExamHistoryViewModel.cs`：
	- 重构为“按考试批次分组”的答题卡数据结构。
	- 支持点击答题卡显示浮层详情。
	- 支持错题筛选与可折叠统计联动。
	- 订阅 `HistoryChanged`，考试结束后自动刷新历史统计。
- 更新 `PhonoArk/PhonoArk/Views/ExamHistoryView.axaml`：
	- 历史页改为批次卡片 + WrapPanel 答题卡展示（红绿 + ✓/✗）。
	- 音标错误统计改为 `Expander` 折叠区。
	- 新增点击答题卡后的浮层详情窗体。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk/PhonoArk.sln`：构建成功（0 Error）。

## 2026-02-23 13:03:33

### 实现目标
- 修复考试页作答后整组选项发灰、红绿反馈不明显、反馈提示导致布局跳动的问题。
- 修复界面语言切换“看似切换但主界面不生效”的问题。

### 变更内容
- 更新 `PhonoArk/PhonoArk/ViewModels/ExamViewModel.cs`：
	- 将 `SelectAnswerCommand` 改为 `AllowConcurrentExecutions = true`，避免异步等待期间命令自动禁用导致整组按钮发灰。
- 更新 `PhonoArk/PhonoArk/Views/ExamView.axaml`：
	- 反馈区域改为固定占位（`MinHeight`）并仅切换文本可见性，消除答题后布局上下跳动。
- 更新 `PhonoArk/PhonoArk/App.axaml.cs`：
	- 统一使用 `App.axaml` 资源中的同一 `LocalizationService` 实例（`Loc`）注入 ViewModel。
	- 解决此前“UI 绑定读的是一个 Loc 实例，设置页修改的是另一个实例”导致的语言切换不生效。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk/PhonoArk.sln`：构建成功（0 Error）。

## 2026-02-23 12:57:32

### 实现目标
- 优化考试作答反馈：答对显示绿色对勾，答错显示红色叉号并同步标出正确答案，短暂停留后自动下一题。
- 增强历史记录：支持逐题对错回放、仅错题筛选、历史清空与错误音标统计。

### 变更内容
- 新增 `PhonoArk/PhonoArk/Models/ExamQuestionAttempt.cs`，用于持久化每道题的作答明细。
- 更新 `PhonoArk/PhonoArk/Data/AppDbContext.cs`：增加 `ExamQuestionAttempts` 表映射与索引配置。
- 更新 `PhonoArk/PhonoArk/Services/ExamHistoryService.cs`：
	- 新增逐题明细保存接口 `SaveResultWithAttemptsAsync`。
	- 新增逐题明细查询与音标错误统计查询。
	- 清空历史时同时清空逐题明细。
	- 对旧数据库增加 `CREATE TABLE IF NOT EXISTS` 兼容创建逻辑。
- 更新 `PhonoArk/PhonoArk/Models/ExampleWord.cs`：增加考试反馈状态属性（正确/错误）与展示文本。
- 更新 `PhonoArk/PhonoArk/Services/ExamService.cs`：出题时克隆选项词条，避免状态污染原始词库对象。
- 更新 `PhonoArk/PhonoArk/ViewModels/ExamViewModel.cs`：
	- 作答后标记当前选项状态并显示正确答案高亮。
	- 自动跳题等待时间缩短为 1.2 秒。
	- 结束考试时保存逐题明细。
- 更新 `PhonoArk/PhonoArk/Views/ExamView.axaml` 与 `PhonoArk/PhonoArk/App.axaml`：
	- 增加答题选项 `correct/wrong` 视觉样式（绿/红覆盖）。
- 更新 `PhonoArk/PhonoArk/ViewModels/ExamHistoryViewModel.cs` 与 `PhonoArk/PhonoArk/Views/ExamHistoryView.axaml`：
	- 历史页新增逐题列表、错题筛选、清空按钮、错误音标统计与总体错误率展示。
- 更新 `PhonoArk/PhonoArk/Resources/Strings.resx` 与 `Strings.zh-CN.resx`：补充历史增强相关中英文文案键。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk/PhonoArk.sln`：构建成功（0 Error）。
- 仍保留 `PhonoArk.Browser` 的既有 WASM NativeFileReference 警告（不影响 Desktop 体验测试）。

## 2026-02-18 23:08:04

### 实现目标
- 完成 `Exam/Favorites/ExamHistory` 三页 i18n 迁移。
- 在设置页增加语言切换并实现持久化。

### 变更内容
- 扩展 `LocalizationService`：新增 `GetString/Format`，并将语言设置持久化到 `LocalApplicationData/PhonoArk/language.txt`。
- `SettingsViewModel` 增加语言选项与当前语言绑定，选择语言时即时生效并持久化。
- `SettingsView` 增加语言选择 UI。
- `ExamView`、`FavoritesView`、`ExamHistoryView` 全面改为资源键绑定（`Loc`），移除大部分硬编码英文文案。
- `ExamViewModel` 的运行时反馈文案（正确/错误/无题可用/考试完成）改为 i18n 资源格式化输出。
- 补全 `Resources/Strings.resx` 与 `Resources/Strings.zh-CN.resx` 的新键值翻译。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk/PhonoArk.sln`：构建成功（0 Error）。
- Browser 项目仍有既有 WASM 警告，未新增与本次改动相关的编译错误。

## 2026-02-18 23:21:20

### 实现目标
- 修复设置页“默认口音”无法选择（`InvalidCastException`）问题。
- 让音标/单词按钮在 Desktop 上至少产生可听反馈音。

### 变更内容
- `SettingsViewModel` 新增强类型 `AccentOption` 与 `AccentOptions/SelectedAccentOption`，避免 `ComboBoxItem -> Accent` 的类型转换错误。
- `SettingsView` 的默认口音下拉改为 `ItemsSource` 绑定 `AccentOptions`，通过 `DisplayName` 展示。
- `AudioService` 改造为可听反馈：在未接入真实音频文件播放时，按音标/单词生成回退蜂鸣音（Windows）。
- 为回退蜂鸣音增加跨平台保护与分析器处理，避免构建失败。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk/PhonoArk.sln`：构建成功（0 Error）。
- 当前仅保留既有 Browser WASM 警告，与本次修复无关。

## 2026-02-18 23:35:25

### 实现目标
- 为 Windows Desktop 接入可用的系统 TTS，实现音标/单词点击后正常语音输出。

### 变更内容
- `AudioService` 增加 Windows TTS 播放链路：通过反射调用 `System.Speech.Synthesis.SpeechSynthesizer`，避免对非 Windows 目标产生编译耦合。
- 增加按口音选择语音文化（`GenAm -> en-US`，`RP -> en-GB`）的逻辑。
- 音标播放优先使用示例单词进行 TTS（可听性更高），单词播放直接朗读单词文本。
- 当 TTS 不可用时自动回退到原有提示音兜底，保证交互有反馈。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk/PhonoArk.sln`：构建成功（0 Error）。
- Browser 项目仍有既有 WASM 警告，未新增与本次改动相关错误。

## 2026-02-18 23:43:31

### 实现目标
- 修复 Windows Debug 桌面环境下语音仍回退蜂鸣的问题，提升 TTS 命中率。

### 变更内容
- 在 `AudioService` 中新增 Windows 二级兜底：当直接反射 `System.Speech` 失败时，自动调用 `pwsh/powershell` 执行系统 TTS 脚本朗读。
- 脚本中同步应用当前口音（`en-US/en-GB`）与音量百分比。
- 保留最终蜂鸣兜底，仅在两级 TTS 均失败时触发。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk/PhonoArk.sln`：构建成功（0 Error）。
- Browser 项目仍有既有 WASM 警告，与本次修复无关。

## 2026-02-18 23:53:50

### 实现目标
- 按根因修复 Windows Debug 下 TTS 回退蜂鸣问题，不再使用 PowerShell 兜底。

### 变更内容
- 移除 `AudioService` 中的 PowerShell TTS 兜底实现。
- 根因修复：将 `System.Speech` 调用从线程池线程改为专用 STA 线程执行（`RunOnStaThreadAsync`），避免线程模型不匹配导致语音合成失败。
- 保留最后一级蜂鸣兜底，仅在系统 TTS 仍失败时触发。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk/PhonoArk.sln`：构建成功（0 Error）。
- 仅保留既有 Browser WASM 警告，未引入新错误。

## 2026-02-23 11:05:29

### 实现目标
- 发布目标收敛为 Windows + Android，移除 iOS/Browser 的发布与解决方案入口。
- 修复 Windows 下 IPA 页面点击单词无发音的问题。

### 变更内容
- 更新 `.github/workflows/release.yml`：
	- Windows 构建仅保留 `win-x64` / `win-arm64`。
	- 删除 Linux/macOS 产物流程。
	- 新增 Android 构建作业，安装 Android workload 后发布并上传 `.apk` 产物。
	- Release 阶段改为同时收集 `.zip`（Windows）与 `.apk`（Android）。
- 更新 `PhonoArk/PhonoArk.sln`：移除 `PhonoArk.Browser` 与 `PhonoArk.iOS` 项目引用，仅保留 `PhonoArk`、`PhonoArk.Desktop`、`PhonoArk.Android`。
- 更新 `PhonoArk/PhonoArk/Services/AudioService.cs`：
	- 在现有 `System.Speech` 朗读失败时新增 Windows SAPI COM 兜底（`SAPI.SpVoice`）。
	- 按口音优先选择语音（GenAm=`0x409`，RP=`0x809`），并应用音量设置。
	- 两级 TTS 失败后才回退蜂鸣音，提升“点击单词可听到发音”的成功率。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk/PhonoArk.Desktop/PhonoArk.Desktop.csproj -c Debug`：构建成功（0 Error）。

## 2026-02-24 19:18:00

### 实现目标
- 在 `tools/msspeechcmd/bin/Release/net10.0/win-x64` 下新增一版可执行的 `test.ps1`，用于批量调用 `msspeechcmd` 生成音标与单词发音文件。

### 变更内容
- 新增 `tools/msspeechcmd/bin/Release/net10.0/win-x64/test.ps1`：
	- 读取 `phoneme-word-bank.json` 的 `phonemes` 与 `globalWords`。
	- 音标按顺序输出为 `phonemes01.wav`、`phonemes02.wav` ...（自动补零）。
	- 单词输出为与单词同名的音频文件（如 `apple.wav`）。
	- 输出目录分层为：`output/phonemes` 与 `output/words`。
	- 支持 `-Overwrite`、`-SkipPhonemes`、`-SkipWords`、`-Locale us|uk`、`-Voice`。
	- 默认使用 SSML `phoneme` 标签生成音标发音，也支持 `-UsePlainTextPhoneme` 回退纯文本发音。

### 验证结果（可选）
- PowerShell 语法解析通过（Parser 无错误）。
- 执行 `test.ps1 -SkipPhonemes -SkipWords` 空跑通过，脚本可正常启动并输出汇总。

## 2026-02-24 19:31:00

### 实现目标
- 修复 `test.ps1` 在音标模式下调用 Azure SSML 时报错：`Ssml should contain at least one [VOICE] tag`。

### 变更内容
- 更新 `tools/msspeechcmd/bin/Release/net10.0/win-x64/test.ps1`：
	- 新增 `Resolve-SsmlVoiceName`，根据 `-Locale` 与 `-Voice` 解析可用 Azure 语音名。
	- 音标 SSML 由仅 `<phoneme>` 调整为 `<voice><phoneme/></voice>` 结构，满足 Azure SSML 要求。
	- 支持 `-Voice` 传短名（如 `Jenny`）或完整名（如 `en-US-JennyNeural`）。

### 验证结果（可选）
- `test.ps1` PowerShell 语法解析通过（Parser 无错误）。
- 根因已确认：缺少 `<voice>` 标签导致服务端 1007 关闭连接。

## 2026-02-24 19:42:00

### 实现目标
- 更新 `test.ps1` 的脚本用法说明，支持在 PowerShell 中直接通过 `Get-Help` 查看完整文档。

### 变更内容
- 更新 `tools/msspeechcmd/bin/Release/net10.0/win-x64/test.ps1`：
	- 新增 Comment-Based Help（`SYNOPSIS` / `DESCRIPTION` / `PARAMETER` / `EXAMPLE`）。
	- 补充常用示例：全量、仅音标、仅单词（英式 Sonia）和 `Get-Help` 查看方式。
	- 整理音标 SSML 片段的缩进，提升可读性。

### 验证结果（可选）
- PowerShell 语法解析通过（Parser 无错误）。
- `Get-Help .\test.ps1 -Detailed` 可正常展示中文说明与示例。
- `test.ps1 -SkipPhonemes -SkipWords` 空跑通过。

## 2026-02-24 19:58:00

### 实现目标
- 为 `test.ps1` 增加“失败自动重试”能力，避免网络抖动导致批量任务立即中断。

### 变更内容
- 更新 `tools/msspeechcmd/bin/Release/net10.0/win-x64/test.ps1`：
	- 在 `Invoke-MsSpeechSynthesize` 中加入重试循环。
	- 合成失败后等待 `2` 秒重试，最多尝试 `3` 次。
	- 若连续 `3` 次失败，抛出异常并停止脚本。
	- 在脚本帮助 `NOTES` 中补充重试行为说明。

### 验证结果（可选）
- PowerShell 语法解析通过（Parser 无错误）。
- 执行 `test.ps1 -SkipPhonemes -SkipWords` 空跑通过。

## 2026-02-24 20:16:00

### 实现目标
- 为 `test.ps1` 增加 PS7 并发处理能力，可控制并发数（默认 10），提升批量合成吞吐。

### 变更内容
- 更新 `tools/msspeechcmd/bin/Release/net10.0/win-x64/test.ps1`：
	- 新增参数 `-ThrottleLimit`（`1~64`，默认 `10`）。
	- 音标与单词批量任务改为 `ForEach-Object -Parallel` 并发执行（PS7 runspace 并行模型）。
	- 保留并适配重试策略：每条任务失败后等待 2 秒，最多重试 3 次，连续失败则记为失败。
	- 批次结束后若存在失败任务，抛出汇总异常并停止。
	- 脚本帮助新增 `ThrottleLimit` 参数说明与并发行为说明。

### 验证结果（可选）
- PowerShell 语法解析通过（Parser 无错误）。
- 执行 `test.ps1 -SkipPhonemes -SkipWords -ThrottleLimit 10` 空跑通过。

## 2026-02-24 19:05:00

### 实现目标
- 为 `tools/msspeechcmd/bin/Release/net10.0/win-x64/msspeechcmd.exe` 生成运行所需的 Azure Speech 配置 JSON（Key + Region）。

### 变更内容
- 新增 `tools/msspeechcmd/bin/Release/net10.0/win-x64/speechconfig.json`：
	- `region`：`eastasia`
	- `key`：`YOUR_AZURE_SPEECH_KEY`（占位符，待替换为真实密钥）
- 配置格式与程序内 `LoadSpeechConfig` 逻辑一致：`{"region":"...","key":"..."}`。

### 验证结果（可选）
- 已确认配置文件位于 `msspeechcmd.exe` 同目录，满足默认读取路径（当前目录下 `speechconfig.json`）。

## 2026-02-24 18:13:47

### 实现目标
- 继续收敛 Android 练习测试选项“看起来过宽”的问题。
- 修复移动端样式可能未命中导致仍沿用桌面固定宽度的风险。

### 变更内容
- 更新 `PhonoArk/PhonoArk/Views/ExamView.axaml`：
	- 将 `exam-option` 的固定宽度从通用样式中移除，仅在 `UserControl.desktop` 下启用 `560` 固定宽。
	- Android 移动端选项内边距由 `12` 调整为 `20`（横屏为 `16`），降低“铺满过宽”观感。
	- 保持移动端动态宽度（`Width=NaN` + `HorizontalAlignment=Stretch`）与圆角卡片效果。
- 更新 `PhonoArk/PhonoArk/Views/ExamView.axaml.cs`：
	- 构造函数中显式为非移动端添加 `desktop` 类，确保桌面/移动样式边界明确。
	- Android 横/竖屏 `FeedbackBorder.Margin` 调整为 `20,6,20,0`，与选项按钮水平边界一致。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk/PhonoArk.sln`：构建成功（0 Error）。

## 2026-02-24 16:13:21

### 实现目标
- 深化历史记录页面性能优化，降低 Home/History 切换时的卡顿感。
- 将历史数据加载从“全量拉取后分组”改为“分页按需加载 + 详情点击再取”。

### 变更内容
- 更新 `PhonoArk/PhonoArk/Services/ExamHistoryService.cs`：
	- 新增按考试结果分页取 ID 的接口 `GetExamResultIdPageAsync(skip, take)`。
	- 新增按结果 ID 批量取摘要接口 `GetQuestionAttemptsSummaryByResultIdsAsync(...)`。
	- 新增答题总量统计接口 `GetAttemptMetricsAsync()`，减少 VM 侧二次计算压力。
- 更新 `PhonoArk/PhonoArk/ViewModels/ExamHistoryViewModel.cs`：
	- 重构为“首屏小页 + 滚动增量加载”模型，首屏优先渲染。
	- 新增可取消加载机制（`CancellationTokenSource`），离开页面可中断后台加载。
	- 汇总指标改为后台并行刷新，避免阻塞首屏显示。
	- 保留点击答题卡后再加载详情的按需策略。
- 更新 `PhonoArk/PhonoArk/Views/ExamHistoryView.axaml.cs`：
	- 页面加载改为 `EnsureHistoryLoadedAsync()`，避免重复全量刷新。
	- 视图离开可触发取消，配合 VM 降低无效计算。
- 更新 `PhonoArk/PhonoArk/ViewModels/MainViewModel.cs`：
	- 切换离开历史页时主动通知 `OnViewDeactivated()`，减少后台任务占用。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk/PhonoArk.sln`：构建成功（0 Error）。

## 2026-02-24 15:22:34

### 实现目标
- 优化 Android 练习测试选项区域边界：选项按钮应以内层圆角卡片为容器边界，左右贴近但不贴死边。
- 首页（IPA）在 Android 引入“点音标再加载词表”的懒加载体验：先显示音标，单词区在点击后出现。
- 修复首页单词区 IPA 字体缺字：与首页音标区统一字体策略。
- 历史记录进一步减轻切换卡顿，并将弹窗内容改为点击后按需加载。

### 变更内容
- 更新 `PhonoArk/PhonoArk/Views/ExamView.axaml`：
	- Android 端 `exam-option` 保持自适应撑满（`Stretch`）但增加左右内边距效果（`Margin=10,3`），保留圆角矩形边界可见。
	- 项模板默认 `Margin` 归零，避免桌面与移动端叠加边距冲突。
- 更新 `PhonoArk/PhonoArk/Services/PhonemeDataService.cs`：
	- 新增 `GetExampleWordsBySymbol`，按音标按需返回词表副本（支持懒加载）。
- 更新 `PhonoArk/PhonoArk/ViewModels/IpaChartViewModel.cs`：
	- 新增移动端判定，移动端初始化仅加载音标元信息，词表先不灌入。
	- 点击音标时才按 `Symbol` 拉取词表并显示。
	- 新增 `PrepareForDisplay()`：移动端切回 IPA 页时重置选中态，确保“单词永远在点击音标后才出现”。
- 更新 `PhonoArk/PhonoArk/ViewModels/MainViewModel.cs`：
	- `NavigateToIpaChart` 前调用 `IpaChartViewModel.PrepareForDisplay()`。
- 更新 `PhonoArk/PhonoArk/Views/IpaChartView.axaml`：
	- 单词区 IPA 文本增加 `ipa-symbol` 类，与首页音标区统一字体链，减少 Android 缺字。
- 更新 `PhonoArk/PhonoArk/Services/ExamHistoryService.cs`：
	- 新增 `GetAllQuestionAttemptsSummaryAsync`（摘要查询）与 `GetQuestionAttemptDetailByIdAsync`（详情按需查询）。
- 更新 `PhonoArk/PhonoArk/ViewModels/ExamHistoryViewModel.cs`：
	- 历史首屏使用摘要数据构建，减少首次切页加载负担。
	- 新增 `LoadAttemptDetailCommand`，点击答题卡时再加载该题用户答案/正确答案。
- 更新 `PhonoArk/PhonoArk/Views/ExamHistoryView.axaml`：
	- 答题卡按钮绑定 `LoadAttemptDetailCommand`，实现“不点击不加载弹窗详情”。

### 验证结果（可选）
- 相关 XAML/C# 文件错误检查通过。
- 执行 `dotnet build PhonoArk/PhonoArk.Desktop/PhonoArk.Desktop.csproj -c Debug`：构建成功（0 Error, 0 Warning）。

## 2026-02-24 14:28:46

### 实现目标
- 区分 Android 与 PC 的练习测试宽度策略：PC 保持固定宽度，Android 根据单词选择区域容器自适应撑满且不溢出。
- 优化历史记录切换卡顿：先显示首屏，再增量加载剩余历史答案。

### 变更内容
- 更新 `PhonoArk/PhonoArk/Views/ExamView.axaml`：
	- `UserControl.mobile Button.exam-option` 与 `UserControl.phone-landscape Button.exam-option` 改为自适应宽度：`Width=NaN`、`HorizontalAlignment=Stretch`，移除固定 200 宽限制。
	- 选项按钮模板 `HorizontalAlignment` 改为 `Stretch`，按单词选择容器两侧顶满展示，不超出容器。
- 更新 `PhonoArk/PhonoArk/ViewModels/ExamHistoryViewModel.cs`：
	- 新增“首屏优先 + 分批加载”机制：初次仅渲染前 2 个考试分组，其余进入待加载队列。
	- 新增 `LoadMoreSessionsCommand`、`HasMoreSessions`、`IsLoadingMoreSessions`，按批次（每次 3 组）增量追加。
	- 进入历史页后短延时预加载一批，兼顾首屏速度与后续连续浏览体验。
- 更新 `PhonoArk/PhonoArk/Views/ExamHistoryView.axaml.cs`：
	- 监听历史列表滚动，接近底部时自动触发 `LoadMoreSessionsCommand`，实现滚动懒加载。

### 验证结果（可选）
- `ExamView.axaml`、`ExamHistoryViewModel.cs`、`ExamHistoryView.axaml.cs` 错误检查通过。
- 执行 `dotnet build .\\PhonoArk.Desktop\\PhonoArk.Desktop.csproj`：构建成功（0 Error, 0 Warning）。

## 2026-02-24 13:20:55

### 实现目标
- 按最新反馈收敛考试界面信息密度：删除“请听音素并发音”提示行，仅保留播放按钮与当前考核音标。
- 将“选择单词”按钮恢复为固定宽度，避免动态伸缩。
- 修复历史记录页面无法打开的问题，恢复可正常进入与展示。

### 变更内容
- 更新 `PhonoArk/PhonoArk/Views/ExamView.axaml`：
	- 删除竖屏/横屏两处“ListenAndSelectWord”提示文案控件。
	- 选词按钮改为固定宽度：`Width/MinWidth/MaxWidth = 500`。
- 更新 `PhonoArk/PhonoArk/Views/ExamView.axaml.cs`：
	- 删除已移除提示控件的字体设置引用，避免控件引用不一致。
- 更新 `PhonoArk/PhonoArk/Views/ExamHistoryView.axaml.cs`：
	- 将横屏布局回退到更稳的行映射（`Auto,Auto,Auto,*`）并重排右侧统计/列表位置，修复页面进入异常。
	- 保留左侧三卡结构（概览/错题筛选/清空历史），避免再次引入打不开问题。

### 验证结果（可选）
- 相关 XAML/C# 文件错误检查通过。
- 执行 `dotnet build .\PhonoArk\PhonoArk.Desktop\PhonoArk.Desktop.csproj`：构建成功（0 Error, 0 Warning）。

## 2026-02-24 13:38:07

### 实现目标
- 定位并修复“历史记录页在 Android 下无法打开”的平台分支问题。

### 变更内容
- 检查 `ExamHistoryView` 移动端布局分支后确认：
	- `ExamHistoryView.axaml.cs` 在移动端竖屏分支中将 `ErrorStatsExpander.MaxHeight` 设为 `double.NaN`。
	- 该值对 `MaxHeight` 是非法值，会在运行时触发异常（桌面端因不走 mobile 分支不易复现，Android 会直接命中）。
- 修复：
	- 将 `ErrorStatsExpander.MaxHeight = double.NaN` 改为 `double.PositiveInfinity`。

### 验证结果（可选）
- `ExamHistoryView.axaml.cs` 错误检查通过。
- 在当前目录执行 `dotnet build .\PhonoArk.Desktop\PhonoArk.Desktop.csproj`：构建成功（0 Error）。

## 2026-02-24 13:46:30

### 实现目标
- 修正桌面端考试页布局被“移动端改动连带影响”的问题：恢复上下区块间距、主容器宽度改为 600、单词按钮更宽。
- 解决选中答案后反馈区域高度跳变问题。
- 优化桌面端历史记录卡片排布：一行可显示更多答案，不再固定 2 列。

### 变更内容
- 更新 `PhonoArk/PhonoArk/Views/ExamView.axaml`：
	- 新增 `UserControl.mobile Button.exam-option` 样式，将 200 固定宽仅限制在移动端，避免影响桌面端。
	- `ActiveExamGrid` 增加 `RowSpacing=12`，恢复上下框体间距。
	- 桌面默认答题主容器改为 `Width/MaxWidth=600`。
	- 桌面默认选项按钮改为固定 `280` 宽（移动端仍保持 `200`）。
	- 反馈区改为固定高度 `52`，并设置 `TextWrapping=NoWrap + TextTrimming=CharacterEllipsis`，避免答案出现时框体被撑高。
- 更新 `PhonoArk/PhonoArk/Views/ExamHistoryView.axaml`：
	- 将答案卡片面板由 `UniformGrid Columns=2` 改为 `WrapPanel`（`ItemWidth=180`），并将单卡固定宽度约 `176`，支持桌面端单行展示更多答案。

### 验证结果（可选）
- `ExamView.axaml` 与 `ExamHistoryView.axaml` 错误检查通过。
- 执行 `dotnet build .\PhonoArk.Desktop\PhonoArk.Desktop.csproj`：构建成功（0 Error, 0 Warning）。

## 2026-02-24 13:56:17

### 实现目标
- 继续优化桌面端历史答案区密度：条目宽度约减半、增大左右间距，并修复两行视觉叠加问题。

### 变更内容
- 更新 `PhonoArk/PhonoArk/Views/ExamHistoryView.axaml`：
	- `WrapPanel` 由 `ItemWidth=180, ItemHeight=38` 调整为 `ItemWidth=104, ItemHeight=50`。
	- 答案按钮由 `176` 宽调整为 `92` 宽（约为原先一半）。
	- 条目外边距由 `Margin=4` 调整为 `Margin=6`（左右间距约提升 1.5 倍）。
	- 按钮内容改为居中，提升紧凑卡片可读性。
	- 通过让 `ItemHeight=按钮高度(38)+上下Margin(6*2)=50`，消除两行重叠。

### 验证结果（可选）
- `ExamHistoryView.axaml` 错误检查通过。
- 执行 `dotnet build .\PhonoArk.Desktop\PhonoArk.Desktop.csproj`：构建成功（0 Error, 0 Warning）。

## 2026-02-24 13:31:42

### 实现目标
- 将考试选词按钮宽度从 500 调整为 200（固定宽度）。
- 修复历史记录页仍无法查看的问题，回退到已验证可用的稳定布局版本。

### 变更内容
- 更新 `PhonoArk/PhonoArk/Views/ExamView.axaml`：
	- `exam-option` 固定宽度由 `500` 改为 `200`（`Width/MinWidth/MaxWidth` 同步）。
- 更新 `PhonoArk/PhonoArk/Views/ExamHistoryView.axaml` 与 `ExamHistoryView.axaml.cs`：
	- 回退到此前稳定的单筛选卡结构（`FilterBorder`），撤销分拆后可能导致不可用的布局映射。
	- 横/竖屏行列与控件映射同步恢复，确保历史页可正常加载与浏览。

### 验证结果（可选）
- 相关 XAML/C# 文件错误检查通过。
- 执行 `dotnet build .\PhonoArk\PhonoArk.Desktop\PhonoArk.Desktop.csproj`：构建成功（0 Error, 0 Warning）。

## 2026-02-24 13:06:50

### 实现目标
- 继续优化移动端考试与历史页体验：解决“练习测试区域偏小、播放按钮文字视觉偏移、考试音标字体不一致、横屏选项格偏窄、历史页横屏信息区不够均衡”等问题。

### 变更内容
- 更新 `PhonoArk/PhonoArk/Views/ExamView.axaml`：
	- 横屏答案按钮改为固定更宽尺寸（`Width/MinWidth/MaxWidth = 500`），避免状态变化时宽度抖动。
	- “播放音素”按钮补充显式内容居中与对称内边距，提升文字视觉居中性。
	- 目标音标展示改为“标签小字 + 音标大字”两行结构，并给音标应用 `ipa-symbol` 字体类，复用 IPA 页面可正常显示的字体链。
	- 选项按钮上限宽度提升到 `620`，横屏下可容纳更长单词。
- 更新 `PhonoArk/PhonoArk/Views/ExamView.axaml.cs`：
	- 移动端（横/竖）考试设置区改为 `Stretch` 并按屏幕高度动态提高 `MinHeight`，减少仅占半屏的空荡感。
	- 横屏主答题区 `MaxWidth` 提升到 `620`，并同步更新新拆分音标文本的字号控制。
	- 竖屏答题区 `MaxWidth` 提升到 `760`，保证内容更饱满。
- 更新 `PhonoArk/PhonoArk/Views/ExamHistoryView.axaml.cs`：
	- 横屏行定义改为 `*,Auto,Auto,Auto,*`，左侧三卡片（概览/错题筛选/清空历史）视觉上下居中。
	- 压缩错误统计占位高度（`MaxHeight = 148`）并调整成绩列表上边距，提升下方答案区域可视空间。

### 验证结果（可选）
- 相关文件错误检查通过（XAML/C# 无错误）。
- 执行 `dotnet build .\PhonoArk\PhonoArk.Desktop\PhonoArk.Desktop.csproj`：构建成功（0 Error, 0 Warning）。

## 2026-02-24 12:35:04

### 实现目标
- 完成“1234都做”中剩余移动端 UI 调整：收藏页/历史页横屏结构优化，底部导航按下态优化，并修复考试页新增布局后的结构问题。

### 变更内容
- 更新 `PhonoArk/PhonoArk/Views/MainView.axaml`：
	- 为 `mobile-nav` 增加按下态样式与短时 `Background/BorderBrush` 过渡，避免点击时灰色闪烁。
	- 对 `mobile-nav.active:pressed` 固定高亮色，减少“按下后退色”观感。
- 更新 `PhonoArk/PhonoArk/Views/FavoritesView.axaml` 与 `FavoritesView.axaml.cs`：
	- 将原“分组 + 清空”单卡片拆成“分组筛选卡 + 清空按钮卡”两段，减少横屏拥挤。
	- 横屏下左列比例调整为 `0.95*:1.55*`，并压缩左侧卡片 padding/spacing；右侧列表增加左边距，视觉更平衡。
	- 竖屏时恢复原有间距与尺寸，保持常规阅读体验。
- 更新 `PhonoArk/PhonoArk/Views/ExamHistoryView.axaml` 与 `ExamHistoryView.axaml.cs`：
	- 将“仅看错题/清空历史”拆分为两个独立卡片，横屏左列形成“概览 + 筛选 + 清空”竖向流。
	- 横屏比例调整为 `0.95*:1.55*`，右侧会话列表区可用宽度提升；ErrorStats 与 Sessions 重排，减少遮挡。
	- 同步修正网格行定义与控件行列映射，竖屏完整恢复到单列顺序布局。
- 修复 `PhonoArk/PhonoArk/Views/ExamView.axaml`：
	- 补齐答案卡片内部容器层级，修复 `Border` 未闭合导致的 XAML 解析错误。

### 验证结果（可选）
- XAML/C# 错误检查通过（相关文件无错误）。
- 执行 `dotnet build .\PhonoArk\PhonoArk.Desktop\PhonoArk.Desktop.csproj`：构建成功（0 Error, 0 Warning）。

## 2026-02-23 12:32:01

### 实现目标
- 恢复被误删的 `AudioService`，修复项目不可编译状态。
- 打通设置页“语音诊断”能力，便于定位 Windows 口音切换与语音包缺失问题。

### 变更内容
- 新建 `PhonoArk/PhonoArk/Services/AudioService.cs`：
	- 恢复 Windows 主流单路径 `System.Speech` 发音实现（STA 线程 + `SpeakAsync`）。
	- 保留“新点击打断上一次发音”的停止/释放逻辑（`SpeakAsyncCancelAll` + `Dispose`）。
	- 保留按口音选择语音（优先按已安装语音文化 `en-US/en-GB`，失败再回退 `SelectVoiceByHints`）。
	- 新增 `GetVoiceDiagnosticsAsync()` 诊断接口，返回摘要与已安装语音明细。
- 复核 `SettingsViewModel` 与 `SettingsView`：诊断命令、摘要文本与明细列表绑定正确。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk/PhonoArk.sln`：构建成功（0 Error）。

## 2026-02-23 11:11:37

### 实现目标
- 按需求彻底删除非目标平台项目目录（Browser / iOS），仅保留 Windows + Android。

### 变更内容
- 物理删除目录：`PhonoArk/PhonoArk.Browser`。
- 物理删除目录：`PhonoArk/PhonoArk.iOS`。

### 验证结果（可选）
- 目录存在性检查结果：`BrowserExists=False; iOSExists=False`。

## 2026-02-23 11:22:19

### 实现目标
- 按需求将 Windows 发音收敛为单一主流方案：点击即走系统 TTS。
- 移除蜂鸣音与多重兜底分支，简化逻辑。

### 变更内容
- 更新 `PhonoArk/Directory.Packages.props`：新增 `System.Speech` 统一包版本。
- 更新 `PhonoArk/PhonoArk.Desktop/PhonoArk.Desktop.csproj`：引入 `System.Speech` 包引用，确保 Windows 运行时可解析语音合成程序集。
- 更新 `PhonoArk/PhonoArk/Services/AudioService.cs`：
	- 保留单一路径 `System.Speech.SpeechSynthesizer`（STA 线程）作为 Windows 发音实现。
	- 删除 SAPI COM 兜底。
	- 删除蜂鸣音回退逻辑。
	- 失败时仅记录日志，不再用提示音替代。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk/PhonoArk.Desktop/PhonoArk.Desktop.csproj -c Debug`：构建成功（0 Error）。

## 2026-02-23 11:32:26

### 实现目标
- 修复点击发音后界面长时间不可继续操作的问题。
- 实现“新点击立即打断上一次发音并播放当前发音”的交互。
- 对主界面与设置页进行现代化（Flutter 风格）视觉升级。

### 变更内容
- 更新 `PhonoArk/PhonoArk/Services/AudioService.cs`：
	- 将朗读方式从阻塞式 `Speak` 改为非阻塞 `SpeakAsync`。
	- 增加当前语音实例管理：新播放前执行 `SpeakAsyncCancelAll` 并释放上一次实例，确保可即时打断。
	- `StopPlayback()` 改为真实停止逻辑（STA 线程内执行取消与释放）。
	- 保持单一路径 `System.Speech`（Windows）不变，不引入蜂鸣/多兜底。
- 更新 `PhonoArk/PhonoArk/App.axaml`：新增全局现代化样式（卡片、胶囊导航按钮、主按钮、输入控件高度、标题与说明文案风格）。
- 更新 `PhonoArk/PhonoArk/Views/MainView.axaml`：
	- 改为卡片化侧栏 + 卡片化内容区布局。
	- 背景与间距优化，导航按钮统一为现代化胶囊样式。
- 更新 `PhonoArk/PhonoArk/Views/SettingsView.axaml`：
	- 重构为分区卡片布局（口音、语言、音量、题量、体验开关）。
	- `CheckBox` 升级为 `ToggleSwitch`，增强现代感。
	- 保存按钮升级为强调色主按钮。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk/PhonoArk.Desktop/PhonoArk.Desktop.csproj -c Debug`：构建成功（0 Error）。

## 2026-02-23 11:41:21

### 实现目标
- 将剩余页面（IPA 图表 / 测试 / 收藏 / 历史）统一到同一套现代化 Flutter 风格视觉体系。

### 变更内容
- 更新 `PhonoArk/PhonoArk/App.axaml`：
	- 扩展全局风格系统，新增 `flutter-secondary`、`flutter-tile` 样式，统一按钮层级与交互反馈。
- 更新 `PhonoArk/PhonoArk/Views/IpaChartView.axaml`：
	- 页面头部、分组区域、详情区域全部卡片化。
	- 音标按钮统一为 `flutter-tile`，播放/收藏/例词按钮统一主次按钮风格。
- 更新 `PhonoArk/PhonoArk/Views/ExamView.axaml`：
	- 测试配置区改为卡片布局。
	- 题目区、反馈区、控制按钮统一风格与间距。
- 更新 `PhonoArk/PhonoArk/Views/FavoritesView.axaml`：
	- 顶部筛选区卡片化。
	- 收藏项列表卡片化，操作按钮统一次级按钮风格。
- 更新 `PhonoArk/PhonoArk/Views/ExamHistoryView.axaml`：
	- 统计摘要与历史条目统一卡片风格，整体视觉与其他页面一致。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk/PhonoArk.Desktop/PhonoArk.Desktop.csproj -c Debug`：构建成功（0 Error）。

## 2026-02-23 11:46:33

### 实现目标
- 在统一风格基础上增加导航激活态、轻量交互动效与深色主题一致性。

### 变更内容
- 更新 `PhonoArk/PhonoArk/ViewModels/MainViewModel.cs`：
	- 新增 `IsIpaChartSelected / IsExamSelected / IsFavoritesSelected / IsHistorySelected / IsSettingsSelected` 状态。
	- 在 `CurrentView` 变更时自动刷新导航选中态。
- 更新 `PhonoArk/PhonoArk/Views/MainView.axaml`：
	- 导航按钮新增 `Classes.active` 绑定，实现当前页面高亮。
	- 主背景改为主题资源驱动，支持深浅色一致视觉。
- 更新 `PhonoArk/PhonoArk/App.axaml`：
	- 新增 Light/Dark 主题色板资源（页面背景、卡片、主色、Tile、导航激活色）。
	- 全局样式改为动态主题资源驱动。
	- 为主/次/导航/Tile 按钮新增平滑颜色过渡动效（BrushTransition）。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk/PhonoArk.Desktop/PhonoArk.Desktop.csproj -c Debug`：构建成功（0 Error）。

## 2026-02-23 11:51:15

### 实现目标
- 继续提升 Flutter 风格一致性：导航图标化、页面切换动画、卡片微抬升质感。

### 变更内容
- 更新 `PhonoArk/PhonoArk/Views/MainView.axaml`：
	- 导航按钮由 emoji 文本改为“图标 + 文本”结构（Segoe Fluent Icons 字形）。
	- 内容容器由 `ContentControl` 升级为 `TransitioningContentControl`，新增 `CrossFade` 页面切换动画。
- 更新 `PhonoArk/PhonoArk/App.axaml`：
	- 卡片 `flutter-card` 增加悬浮边框高亮与阴影增强，形成微抬升效果。
	- 新增图标文本样式 `flutter-icon`，统一图标字号与字体来源。
- 更新 `PhonoArk/PhonoArk/Resources/Strings.resx` 与 `Strings.zh-CN.resx`：
	- 导航文案移除 emoji 前缀，交由图标样式统一呈现。

### 验证结果（可选）
- 重新执行 `dotnet build PhonoArk/PhonoArk.Desktop/PhonoArk.Desktop.csproj -c Debug`：构建成功（0 Error）。
- 期间出现一次输出文件被运行中进程占用导致的构建失败，释放进程后复构建通过。

## 2026-02-23 12:01:15

### 实现目标
- 修复 IPA 顶部口音切换按钮显示异常。
- 修复点击单词时整块区域闪烁问题，仅保留当前点击按钮的按下反馈。
- 修复深色模式切换不生效问题。

### 变更内容
- 更新 `PhonoArk/PhonoArk/Views/IpaChartView.axaml`：
	- 为口音切换按钮补充 `Grid.Column="1"`，避免与标题重叠导致“看起来消失”。
	- 示例单词按钮样式从通用次级按钮改为 `flutter-word`，用于独立按下反馈。
- 更新 `PhonoArk/PhonoArk/App.axaml`：
	- 将卡片悬浮增强从全局 `flutter-card:pointerover` 收敛为 `flutter-card-lift:pointerover`，避免详情区整体闪烁。
	- 新增 `flutter-word:pressed` 样式，实现仅当前点击单词按钮的即时视觉反馈。
- 更新 `PhonoArk/PhonoArk/ViewModels/SettingsViewModel.cs`：
	- 在加载设置与 `DarkMode` 变更时同步设置 `Application.Current.RequestedThemeVariant`。
	- 深色/浅色模式现可即时生效。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk/PhonoArk.Desktop/PhonoArk.Desktop.csproj -c Debug`：构建成功（0 Error）。

## 2026-02-23 12:06:58

### 实现目标
- 将“卡片微抬升”效果限定在主导航与设置卡片，避免学习内容区产生不必要动态干扰。

### 变更内容
- 更新 `PhonoArk/PhonoArk/Views/MainView.axaml`：
	- 主导航卡片增加 `flutter-card-lift` 类。
- 更新 `PhonoArk/PhonoArk/Views/SettingsView.axaml`：
	- 设置页各分区卡片增加 `flutter-card-lift` 类。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk/PhonoArk.Desktop/PhonoArk.Desktop.csproj -c Debug`：构建成功（0 Error）。

## 2026-02-23 12:11:30

### 实现目标
- 修复点击单词后整组单词按钮一起变灰的问题。

### 变更内容
- 更新 `PhonoArk/PhonoArk/ViewModels/IpaChartViewModel.cs`：
	- 将 `PlayPhonemeAsync` 与 `PlayWordAsync` 命令改为 `[RelayCommand(AllowConcurrentExecutions = true)]`。
	- 避免异步命令执行时自动禁用同命令绑定的全部按钮。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk/PhonoArk.Desktop/PhonoArk.Desktop.csproj -c Debug`：构建成功（0 Error）。
- 首次构建因运行中的 `PhonoArk.Desktop` 锁定输出文件失败，结束进程后复构建通过。

## 2026-02-23 12:15:38

### 实现目标
- 点击单词时仅高亮当前单词按钮，不影响其他单词。
- 修复英式/美式切换后听感变化不明显的问题，提升口音切换可感知性。

### 变更内容
- 更新 `PhonoArk/PhonoArk/Models/ExampleWord.cs`：
	- 增加可观察属性 `IsPlaying`，用于单词项播放状态展示。
- 更新 `PhonoArk/PhonoArk/ViewModels/IpaChartViewModel.cs`：
	- 播放单词前清除其他单词状态，仅标记当前单词 `IsPlaying=true`。
	- 切换音标时清理高亮状态，避免旧高亮残留。
- 更新 `PhonoArk/PhonoArk/Views/IpaChartView.axaml`：
	- 单词按钮新增 `Classes.playing="{Binding IsPlaying}"` 绑定。
- 更新 `PhonoArk/PhonoArk/App.axaml`：
	- 新增 `Button.flutter-word.playing` 样式，当前单词显示为主色高亮。
- 更新 `PhonoArk/PhonoArk/Services/AudioService.cs`：
	- 口音切换优先按已安装语音列表精确匹配文化（`en-GB` / `en-US`）并按语音名选择。
	- 若精确匹配失败再回退到 `SelectVoiceByHints`，增强切换稳定性。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk/PhonoArk.Desktop/PhonoArk.Desktop.csproj -c Debug`：构建成功（0 Error）。

## 2026-02-23 14:40:33

### 实现目标
- 彻底消除考试选项颜色被灰色状态覆盖的问题，避免继续叠加样式补丁。

### 变更内容
- 更新 `PhonoArk/PhonoArk/ViewModels/ExamViewModel.cs`：
	- 新增 `AreOptionsInteractive` 交互锁定状态。
	- 开始考试/切换到下一题时开启交互，作答后立即锁定，杜绝反馈阶段再次触发交互态覆盖。
	- `SelectAnswerAsync` 增加交互锁判断，形成单入口控制。
- 更新 `PhonoArk/PhonoArk/Views/ExamView.axaml`：
	- 考试选项按钮从 `flutter-secondary examoption` 改为独立 `exam-option` 类。
	- 绑定 `IsHitTestVisible` 到 `AreOptionsInteractive`，作答反馈期间不再进入 hover/pressed 状态。
- 更新 `PhonoArk/PhonoArk/App.axaml`：
	- 删除 `flutter-secondary.examoption` 相关 correct/wrong/pointerover 多分支规则。
	- 新增独立 `Button.exam-option` 基础样式，仅保留形态与过渡，不再写入状态颜色，颜色统一由绑定属性提供。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk/PhonoArk.sln`：构建成功（0 Error）。

## 2026-02-23 14:56:13

### 实现目标
- 在考试页显示当前考核音标（纯文字）。
- 保证错误备选项不包含当前要考核的音标。

### 变更内容
- 更新 `PhonoArk/PhonoArk/Views/ExamView.axaml`：
	- 在播放按钮下新增“当前考核音标”文本，绑定 `CurrentQuestion.Phoneme.Symbol`。
- 更新 `PhonoArk/PhonoArk/Resources/Strings.resx` 与 `Strings.zh-CN.resx`：
	- 新增文案键 `TargetPhonemeLabel`（英/中文）。
- 更新 `PhonoArk/PhonoArk/Services/ExamService.cs`：
	- 新增 IPA 归一化与目标音标匹配方法（`NormalizePhonemeToken`、`ContainsTargetPhonemeToken`）。
	- 正确答案优先从“确实包含目标音标”的候选中抽取。
	- 错误选项先从严格池抽取：仅来自其他音标且 IPA 不含目标音标，再去重随机填充。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk/PhonoArk.sln`：构建成功（0 Error）。

## 2026-02-23 21:06:18

### 实现目标
- 按 Android 真实使用场景重构主导航：手机采用底部导航，平板采用左侧 rail，避免“顶部只能点一个”的交互问题。

### 变更内容
- 更新 `PhonoArk/PhonoArk/Views/MainView.axaml`：
	- 导航容器拆分为两套形态：
		- `MobileBottomNavGrid`（手机底部 5 项等宽导航）
		- `DesktopNavScroll`（平板/桌面侧栏导航）
	- 为平板 rail 相关节点增加命名（品牌标题、副标题、即将上线入口），便于运行时按断点控制显隐。
	- 新增 `mobile-nav` 样式与 `active` 激活态，优化底部导航触控体验与状态可见性。
- 更新 `PhonoArk/PhonoArk/Views/MainView.axaml.cs`：
	- 新增断点逻辑（`TabletBreakpoint = 900`）并监听窗口尺寸变化。
	- 在移动平台下按宽度自动切换：
		- 手机（<900）：内容在上、底部导航在下。
		- 平板（>=900）：左侧 rail + 右侧内容。
	- 平板模式下隐藏品牌副标题与“即将上线”入口，提升导航密度。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk.Desktop/PhonoArk.Desktop.csproj`：构建成功（0 Error）。

## 2026-02-23 21:18:50

### 实现目标
- 优化 IPA 详情区交互，减少占位控件：收藏改为右上角星标、音标本体点击即发音。
- 优化示例词展示密度：按词长自适应并支持一行多个单词。
- 保证 Android 与 Desktop 使用同一套交互与布局处理逻辑。

### 变更内容
- 更新 `PhonoArk/PhonoArk/ViewModels/IpaChartViewModel.cs`：
	- `SelectPhonemeCommand` 改为 `AllowConcurrentExecutions = true`。
	- 选择音标后除选中与收藏状态同步外，立即调用播放逻辑，实现“点击音标即发音”。
- 更新 `PhonoArk/PhonoArk/Views/IpaChartView.axaml`：
	- 移除详情区独立“播放发音”按钮与大块收藏按钮。
	- 新增右上角 `☆/★` 双态收藏图标按钮（收藏后即时切换为实心星）。
	- 将详情区主音标改为可点击按钮，直接触发 `PlayPhonemeCommand`。
	- 示例词列表改为 `WrapPanel` 流式布局，单词项改为紧凑 `word-chip` 样式（单词与 IPA 横向排布），支持一行多个单词并按内容长度自然换行。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk.Desktop/PhonoArk.Desktop.csproj`：构建成功（0 Error）。

## 2026-02-23 21:25:30

### 实现目标
- 为 IPA 音标按钮增加“当前选中”高亮，提升定位感与操作反馈一致性。

### 变更内容
- 更新 `PhonoArk/PhonoArk/Models/Phoneme.cs`：
	- 新增可观察状态 `IsSelected`，用于驱动音标按钮激活态样式。
- 更新 `PhonoArk/PhonoArk/ViewModels/IpaChartViewModel.cs`：
	- 在选择音标时统一更新全部音标项的 `IsSelected` 状态，仅当前音标为高亮。
	- 保持“选中即播放”逻辑不变，确保反馈连贯。
- 更新 `PhonoArk/PhonoArk/Views/IpaChartView.axaml`：
	- 为音标按钮绑定 `Classes.active="{Binding IsSelected}"`。
	- 新增 `flutter-tile.active` 高亮样式，采用导航主色进行明显区分。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk.Desktop/PhonoArk.Desktop.csproj`：构建成功（0 Error）。

## 2026-02-23 21:36:40

### 实现目标
- 修复 IPA 详情区已选音标显示过小问题，恢复大字符主视觉。
- 修复收藏星标在部分环境下显示异常及“收藏后无高亮反馈”问题。

### 变更内容
- 更新 `PhonoArk/PhonoArk/Views/IpaChartView.axaml`：
	- 详情区主音标按钮改为固定最小尺寸并将字体提升到 `56`，确保视觉上明显大于下方示例词。
	- 收藏按钮由“双按钮显隐切换”重构为“单按钮内容切换”，通过 `FavoriteButtonConverter` 输出 `☆/★`，并统一字体链，避免符号回退成异常字符。
	- 收藏按钮保留 `active` 样式绑定，收藏后立即呈现实心星与高亮背景。
	- 音标格子新增 `favorite` 状态样式（描边高亮），收藏后的音标在列表中可直接识别。
- 更新 `PhonoArk/PhonoArk/ViewModels/IpaChartViewModel.cs`：
	- 增加收藏状态初始化逻辑，进入页面后为所有音标同步收藏标记。
	- 收藏/取消收藏时同步回写 `SelectedPhoneme.IsFavorite`，保证详情区与音标列表状态一致。
- 更新 `PhonoArk/PhonoArk/Models/Phoneme.cs`：
	- 新增可观察属性 `IsFavorite`，用于 UI 收藏态绑定。
- 更新 `PhonoArk/PhonoArk/Converters/FavoriteButtonConverter.cs`：
	- 输出精简为 `☆/★`，适配顶部图标化交互。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk.Desktop/PhonoArk.Desktop.csproj`：构建成功（0 Error）。

## 2026-02-23 21:43:20

### 实现目标
- 调整发音触发时机：切换音标仅切换内容，不自动发音；发音仅在用户明确点击后触发。

### 变更内容
- 更新 `PhonoArk/PhonoArk/ViewModels/IpaChartViewModel.cs`：
	- 移除 `SelectPhonemeCommand` 中的自动发音调用。
	- 当前行为改为：点击音标只更新选中态/收藏态与详情内容；只有点击详情区大音标或示例单词时才发音。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk.Desktop/PhonoArk.Desktop.csproj`：构建成功（0 Error）。

## 2026-02-23 21:55:10

### 实现目标
- 提升练习模式配置区占位与可读性，避免在大内容区中显得过小。
- 测试范围默认选中“全部音标”。
- 历史详情支持更自然关闭：Windows 支持 `Esc` 和点空白关闭，Android 支持点击空白区域关闭。

### 变更内容
- 更新 `PhonoArk/PhonoArk/Views/ExamView.axaml`：
	- 练习配置卡片宽度上限由 `460` 提升至 `760`，并设置最小高度，整体视觉更饱满。
	- 测试范围下拉从 `SelectedItem` 改为 `SelectedIndex` 绑定，避免默认项空白。
- 更新 `PhonoArk/PhonoArk/ViewModels/ExamViewModel.cs`：
	- 新增 `SelectedExamScopeIndex`（默认 `0`），确保首次进入即选中“全部音标”。
	- 建立 `SelectedExamScopeIndex <-> ExamScope` 双向映射（`0=All`, `1=Favorites`）。
- 更新 `PhonoArk/PhonoArk/Views/ExamHistoryView.axaml`：
	- 新增 `Escape` 快捷键绑定到 `CloseAttemptDetailsCommand`。
	- 新增全屏半透明遮罩按钮，点击遮罩空白区域关闭详情弹层（桌面与 Android 一致生效）。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk.Desktop/PhonoArk.Desktop.csproj`：构建成功（0 Error）。

## 2026-02-23 22:06:30

### 实现目标
- 优化 Android IPA 首屏体验：首次进入仅展示音标区，点击音标后再展开单词详情区。
- 统一音标区与详情区字体表现，避免“上方正常、下方缺字/变形”。
- 优化考试反馈区文案长度，避免底部反馈框被双行撑高。
- 修复底部 5 项导航首项中文标签拥挤重叠。

### 变更内容
- 更新 `PhonoArk/PhonoArk/Views/IpaChartView.axaml.cs`：
	- 在移动端按 `SelectedPhoneme` 动态调整布局：
		- 未选择音标：仅保留音标区（详情/单词区隐藏）。
		- 选择后：展开详情/单词区。
	- 移动端隐藏占位提示卡，避免占用首屏空间。
- 更新 `PhonoArk/PhonoArk/Views/IpaChartView.axaml`：
	- 详情区主音标显式指定与音标按钮一致的字体链，确保显示一致。
- 更新 `PhonoArk/PhonoArk/Resources/Strings.resx` 与 `Strings.zh-CN.resx`：
	- 将考试反馈模板压缩为 `✓/✗ + 正确单词 + IPA` 单行格式。
	- 中文导航首项 `NavIpaChart` 调整为“国际音标”，减少底部栏拥挤。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk.Desktop/PhonoArk.Desktop.csproj`：构建成功（0 Error）。

## 2026-02-23 22:18:20

### 实现目标
- 查看历史答案时取消全屏下拉遮罩与顶部固定弹窗，改为靠近被点击答题卡的就地浮动详情。

### 变更内容
- 更新 `PhonoArk/PhonoArk/Views/ExamHistoryView.axaml`：
	- 将答题卡点击详情从“全局遮罩 + 固定顶部详情面板”重构为“每个答题卡自身的 Flyout 浮窗”。
	- 详情浮窗位置设置为 `Right`，内容与原详情一致（题号、音标、结果、你的答案、正确答案）。
	- 移除全屏半透明遮罩层与顶部固定详情卡，避免整屏刷新/下压观感。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk.Desktop/PhonoArk.Desktop.csproj`：构建成功（0 Error）。

## 2026-02-24 00:07:30

### 实现目标
- 修复 Android 端 IPA 首次进入仅显示提示、音标区被遮挡的问题。
- 保持目标交互：首屏显示大音标区，点击音标后才展开单词详情区。

### 变更内容
- 更新 `PhonoArk/PhonoArk/Views/IpaChartView.axaml`：
	- 移除详情侧占位提示面板（原 `ipa-placeholder-panel`），避免在移动端首屏覆盖音标区。
- 更新 `PhonoArk/PhonoArk/Views/IpaChartView.axaml.cs`：
	- 删除对占位面板的布局与显隐控制。
	- 保留并强化移动端行为：未选中时仅显示音标区；选中后显示详情区。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk.Desktop/PhonoArk.Desktop.csproj`：构建成功（0 Error）。

## 2026-02-24 00:24:10

### 实现目标
- 修复移动端选中音标后详情区过大导致无法继续便捷选择其余音标的问题。
- 音标描述词按中英文系统显示本地化文案（如 Short vowel/短元音）。

### 变更内容
- 更新 `PhonoArk/PhonoArk/Views/IpaChartView.axaml.cs`：
	- 移动端选中音标后布局改为上下各半（`*,*`），保证上半区音标列表始终可继续操作。
- 更新 `PhonoArk/PhonoArk/Services/PhonemeDataService.cs`：
	- 新增描述词本地化映射逻辑（Short/Long vowel、Diphthong、Consonant、Schwa 等）。
	- 加载音标数据时按当前语言环境写入本地化描述。
- 更新 `PhonoArk/PhonoArk/App.axaml.cs`：
	- 调整服务初始化顺序，先初始化 `LocalizationService`，再创建 `PhonemeDataService`，确保描述文案按当前语言生效。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk.Desktop/PhonoArk.Desktop.csproj`：构建成功（0 Error）。

## 2026-02-24 00:33:40

### 实现目标
- 补全语言切换后的实时刷新：IPA 音标描述词在运行中切换中英文后立即更新，无需重启页面。

### 变更内容
- 更新 `PhonoArk/PhonoArk/Models/Phoneme.cs`：
	- 将 `Description` 改为可观察属性，支持运行时变更后 UI 即时刷新。
- 更新 `PhonoArk/PhonoArk/Services/PhonemeDataService.cs`：
	- 新增原始描述缓存（按音标符号保存）。
	- 新增 `RefreshLocalizedDescriptions()`，在当前语言环境下重算并回写所有音标描述。
- 更新 `PhonoArk/PhonoArk/App.axaml.cs`：
	- 订阅 `LocalizationService.PropertyChanged`，在语言切换时触发 `PhonemeDataService.RefreshLocalizedDescriptions()`。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk.Desktop/PhonoArk.Desktop.csproj`：构建成功（0 Error）。

## 2026-02-24 00:47:30

### 实现目标
- 加固“应用首次进入直接打开历史记录页”场景，降低 Android 端潜在闪退风险。

### 变更内容
- 更新 `PhonoArk/PhonoArk/ViewModels/ExamHistoryViewModel.cs`：
	- 为 `LoadHistoryAsync` 增加 `SemaphoreSlim` 互斥保护，避免并发加载触发 EF Core 上下文重入异常。
- 更新 `PhonoArk/PhonoArk/Views/ExamHistoryView.axaml`：
	- 历史答题卡详情 `Flyout` 的 `ShowMode` 由 `TransientWithDismissOnPointerMoveAway` 调整为兼容性更稳的 `Transient`。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk.Desktop/PhonoArk.Desktop.csproj`：构建成功（0 Error）。

## 2026-02-24 01:12:20

### 实现目标
- 继续加固“冷启动后直接进入历史记录页”稳定性，降低未观察异常导致的闪退风险。

### 变更内容
- 更新 `PhonoArk/PhonoArk/App.axaml.cs`：
	- 调整服务构造方式，`FavoriteService`、`ExamHistoryService`、`SettingsService` 改为各自独立 `AppDbContext` 实例，避免共享上下文引发跨服务并发访问问题。
- 更新 `PhonoArk/PhonoArk/ViewModels/ExamHistoryViewModel.cs`：
	- `OnHistoryChanged` 改为调用安全加载入口 `LoadHistorySafeAsync()`。
	- 新增加载异常捕获与日志输出，避免事件回调中的未处理异常向 UI 线程扩散。
	- 补充 `using System;`，修复 `Exception` 类型解析错误。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk.Desktop/PhonoArk.Desktop.csproj`：构建成功（0 Error）。

## 2026-02-24 01:34:30

### 实现目标
- 优化手机横屏（非平板）可用空间：在 IPA/考试/收藏/历史页面引入分栏布局，减少横屏时内容拥挤与滚动受限。

### 变更内容
- 更新 `PhonoArk/PhonoArk/Views/IpaChartView.axaml.cs`：
	- 新增手机横屏判定（`mobile && 宽>高 && 宽<900`）。
	- 横屏且已选音标时切换为左右分栏（左音标区，右详情区）；竖屏保持上下布局。
	- 增加尺寸变化监听，旋转后自动重排。
- 更新 `PhonoArk/PhonoArk/Views/ExamView.axaml` 与 `ExamView.axaml.cs`：
	- 重构考试配置区为可重排网格，手机横屏改为“左两项配置 + 右侧开始按钮”双栏结构。
	- 竖屏保持原单列流程，避免学习路径改变。
- 更新 `PhonoArk/PhonoArk/Views/FavoritesView.axaml` 与 `FavoritesView.axaml.cs`：
	- 新增根布局命名节点并加入手机横屏分栏：左侧筛选面板（较窄），右侧收藏列表（较宽）。
	- 竖屏恢复为“上筛选、下列表”。
- 更新 `PhonoArk/PhonoArk/Views/ExamHistoryView.axaml` 与 `ExamHistoryView.axaml.cs`：
	- 新增手机横屏分栏：左侧统计/筛选/错误统计，右侧历史记录滚动列表。
	- 竖屏保持原纵向堆叠结构。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk.Desktop/PhonoArk.Desktop.csproj`：构建成功（0 Error）。

## 2026-02-24 01:49:10

### 实现目标
- 修正手机横屏下考试页按钮比例失衡问题。
- 修复历史页错误统计下拉在横屏手机上难以展开/阅读的问题。
- 优化历史答题卡可读性：减少同一行卡片数量，缓解字符挤压与串行感。

### 变更内容
- 更新 `PhonoArk/PhonoArk/Views/ExamView.axaml.cs`：
	- 手机横屏下 `Start Exam` 按钮取消跨两行拉伸，改为标准高度（约 64）且顶部对齐，保持与左侧配置卡片比例协调。
- 更新 `PhonoArk/PhonoArk/Views/ExamHistoryView.axaml`：
	- 统计摘要第二行由水平 `StackPanel` 改为 `WrapPanel`，避免长文案在窄宽度下硬挤。
	- 错误统计 `Expander` 内容改为可滚动区域（`ScrollViewer MaxHeight=200`），确保音标错误信息可完整查看。
	- 历史答题卡面板从 `WrapPanel ItemWidth=120` 调整为 `UniformGrid Columns=2`，将横屏场景的一行多卡收敛为两卡。
	- 答题卡按钮改为横向拉伸并保留紧凑高度，提升文本稳定显示。
- 更新 `PhonoArk/PhonoArk/Views/ExamHistoryView.axaml.cs`：
	- 手机横屏添加 `phone-landscape` 类并压缩左侧间距。
	- 横屏下默认折叠错误统计卡，避免初始布局挤压导致不可操作。
	- 横屏隐藏页标题，释放垂直空间给统计筛选与错误信息。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk.Desktop/PhonoArk.Desktop.csproj`：构建成功（0 Error）。
- 执行 `dotnet build PhonoArk.Android/PhonoArk.Android.csproj`：构建成功（0 Error）。

## 2026-02-24 02:08:20

### 实现目标
- 修复 Android 一键联调启动即崩溃（`No assemblies found ... __override__/x86_64` / `F monodroid`）问题。

### 变更内容
- 更新 `PhonoArk/scripts/android-dev-oneclick.ps1`：
	- 联调模式不再仅执行普通 `dotnet build`，改为显式生成可安装 APK（`SignAndroidPackage + apk`）。
	- 强制联调 APK 内嵌程序集：`EmbedAssembliesIntoApk=true`，并禁用共享运行时：`AndroidUseSharedRuntime=false`。
	- APK 选择逻辑改为“按最新时间优先”，联调时优先本次构建产物，避免误装历史残留包。
	- 保留 `PackageOnly` 交付策略不变，联调和交付路径分离，减少互相污染。

### 验证结果（可选）
- 执行 `scripts/android-dev-oneclick.ps1`：构建、安装、启动成功。
- 日志检查未再出现 `No assemblies found ... __override__/x86_64`、`F monodroid`、`Abort at monodroid`。

## 2026-02-24 02:45:00

### 实现目标
- 修复手机横屏考试页面三处布局问题：开始按钮位置、进行中考试单列溢出、历史页错误统计不可展开。

### 变更内容
- `ExamView.axaml` + `ExamView.axaml.cs`：
	- 考试设置横屏：Start 按钮改为 `VerticalAlignment=Center`，`RowSpan=2` 居中显示；横屏时移除 `MinHeight` 以适配矮屏。
	- 考试进行中：重构为左右双区域布局（`ActiveExamGrid`）。左侧（`ExamLeftScroll`）放进度、题目信息、控制按钮；右侧（`ExamAnswerScroll`）放答案选项和反馈。横屏比例 `0.85*:1.15*`，两侧均可滚动。
- `ExamHistoryView.axaml.cs`：
	- 横屏时将错误统计 Expander 移至右栏顶部（`Col=1, Row=0`），下方紧接 Sessions 列表（`Col=1, Row=1, RowSpan=2`），保证 Expander 可正常展开。
	- 左栏仅保留 Summary + Filter，布局更紧凑。

### 验证结果（可选）
- Desktop + Android 构建均通过（0 Error, 0 Warning）。

## 2026-02-24 03:15:00

### 实现目标
- 优化手机横屏各页面的空间比例、字体大小和容器紧凑度，使内容在横屏有限高度内完整显示。

### 变更内容
- `ExamView.axaml` + `ExamView.axaml.cs`：
	- 考试设置横屏：隐藏 "Practice Exam" 标题节省纵向空间；容器改为 `HorizontalAlignment=Center` 自适应宽度包裹；减小 padding 和 RowSpacing；Start 按钮缩至 56px 高度。
	- 考试进行中横屏：左右比例调整为 `0.7*:1.3*`，右侧空间更充裕可显示 5-6 个选项。左侧 card 内间距压缩（Spacing 10→6, Padding 紧凑化）。增加 `phone-landscape` CSS 类，答案按钮字体从 18→15，MinHeight 缩至 32，Margin 从 5→3。反馈区 MinHeight 从 56→44。
	- 为进度/题目/反馈等 8 个 TextBlock 增加 `x:Name`，横屏时动态缩小字体（18→15, 16→14, 20→17 等），竖屏时恢复默认值。
- `ExamHistoryView.axaml` + `ExamHistoryView.axaml.cs`：
	- 横屏左右比例调整为 `0.75*:1.75*`，右侧给更多空间；Margin 从 20→10，RowSpacing 从 6→4。
	- Summary/Filter card 减 Padding（8,6 / 8,4），StackPanel Spacing 从 8→4。
	- Average Score 字体横屏从 18→15；`examattemptcard` 按钮 MinHeight 从 38→34、字体从 13→12、Padding 从 8,4→6,3。
	- 竖屏恢复所有字体和 Padding 默认值。

### 验证结果（可选）
- Desktop + Android 构建均通过（0 Error, 0 Warning）。

## 2026-02-24 10:20:00

### 实现目标
- 评估并吸收 `Get‑HybridCoreMap.ps1` 的 P/E 核识别思路，提升一键联调脚本在 Intel 混合架构上的识别准确性。

### 变更内容
- 更新 `PhonoArk/scripts/android-dev-oneclick.ps1`：
	- 新增 `Ensure-CpuSetNativeType` + `Get-CpuSetCoreClassification`，优先调用 Windows `GetSystemCpuSetInformation` 获取 `EfficiencyClass` / `SchedulingClass`。
	- 修正 P 核判定方向：按最高 `EfficiencyClass` 识别 P 核（原逻辑为最低值）。
	- `Get-PerformanceCoreLogicalProcessorIds` 改为“API 优先，注册表回退”。
	- `Get-CpuTopologyInfo` 改为优先基于 CPU Set 实际分类统计 P/E 核；无法分类时再回退基础混合架构检测。

### 验证结果（可选）
- PowerShell 语法检查通过（解析器 + 编辑器错误检查均无报错）。

## 2026-02-24 21:10:00

### 实现目标
- 新增真人语音选项 `US-Jenny`，并作为默认预选语音。
- 基于 `phoneme-word-bank.json` 与 `Data/US-Jenny` 目录实现“点击音标/单词即播放对应 wav”。
- 抽象可复用的语音映射结构，为后续新增更多真人音色预留扩展点。

### 变更内容
- 更新 `PhonoArk/PhonoArk/Models/Accent.cs` 与 `AppSettings.cs`：
	- 新增 `USJenny` 语音枚举（保留旧枚举值顺序兼容已存储设置）。
	- 新安装默认语音改为 `USJenny`。
- 更新 `PhonoArk/PhonoArk/Models/Phoneme.cs` 与 `ExampleWord.cs`：
	- 新增 `VoiceAudioPaths` 字典，支持按语音键映射音频路径（可扩展到更多真人音色）。
- 更新 `PhonoArk/PhonoArk/Services/PhonemeDataService.cs`：
	- 按 JSON 音标顺序将 `phonemes01.wav` 到 `phonemes44.wav` 映射到各音标。
	- 按单词名将 `Data/US-Jenny/words/<word>.wav` 映射到示例单词。
	- 在数据克隆链路中保留 `VoiceAudioPaths` 映射。
- 更新 `PhonoArk/PhonoArk/Services/AudioService.cs`：
	- 播放时优先尝试当前语音对应的真人 wav；失败再回退到原有 TTS。
	- 新增跨平台文件播放扩展点（共享层 -> 平台层）。
	- 支持从磁盘与 `avares://` 资源双通道解析音频，并缓存提取后的临时文件路径。
	- Windows 下新增 wav 播放与停止能力，并与原 TTS 停止逻辑统一。
- 更新 `PhonoArk/PhonoArk.Android/Services/AndroidTtsBridge.cs`：
	- 新增 Android `MediaPlayer` 文件播放实现。
	- 停止播放时统一停止 TTS 与文件音频。
- 更新 `PhonoArk/PhonoArk/ViewModels/SettingsViewModel.cs` 与 `IpaChartViewModel.cs`：
	- 设置页语音选项新增并默认展示 `US-Jenny`（放在首位）。
	- 顶部语音切换改为三态循环：`US-Jenny -> GenAm -> RP`。
- 更新 `PhonoArk/PhonoArk/Resources/Strings.resx` 与 `Strings.zh-CN.resx`：
	- 新增 `AccentUsJenny` 文案并更新语音切换提示。
- 更新 `PhonoArk/PhonoArk/PhonoArk.csproj`：
	- 将 `Data/US-Jenny/**` 配置为输出复制，保证运行时可直接读取 wav。

### 验证结果（可选）
- 待执行构建与基础播放链路验证（Desktop/Android）。

## 2026-02-24 21:28:00

### 实现目标
- 修复 Windows Debug 下点击音标/单词未听到 US-Jenny 对应 wav 发音的问题。

### 变更内容
- 定位根因：`AudioService` 的 Windows wav 播放依赖 `System.Media.SoundPlayer`，运行时需要 `System.Windows.Extensions.dll`；此前 Desktop 输出目录缺少该依赖，导致真人 wav 分支不可用。
- 更新 `PhonoArk/Directory.Packages.props`：新增 `System.Windows.Extensions` 中央包版本。
- 更新 `PhonoArk/PhonoArk.Desktop/PhonoArk.Desktop.csproj`：新增 `PackageReference Include="System.Windows.Extensions"`。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk/PhonoArk.Desktop/PhonoArk.Desktop.csproj -c Debug`：构建成功（0 Error）。
- 检查输出目录：
	- `System.Windows.Extensions.dll` = `True`
	- `System.Speech.dll` = `True`

## 2026-02-24 21:41:00

### 实现目标
- 将 `US-Jenny` 设为真正默认语音（覆盖已有本地历史设置一次）。
- 加固 Android 本地 wav 播放路径，确保 `US-Jenny` 在 Android 也能稳定发声。

### 变更内容
- 更新 `PhonoArk/PhonoArk/Services/SettingsService.cs`：
	- 新增“一次性默认语音迁移”逻辑：首次命中新版本时，将 `DefaultAccent` 迁移为 `USJenny` 并写入本地 marker 文件（仅执行一次，不会每次启动强制覆盖用户后续选择）。
- 更新 `PhonoArk/PhonoArk.Android/Services/AndroidTtsBridge.cs`：
	- 文件播放前先停止 TTS，避免双通道抢占。
	- `MediaPlayer` 改为基于 `Uri` 的本地文件数据源，增强路径兼容性。
	- 补充空 `Uri` 防御与播放失败回退处理，降低机型差异导致的失败概率。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk/PhonoArk.sln`：构建成功（Desktop + Android）。

## 2026-02-24 21:52:00

### 实现目标
- 修复首页右上角语音名称与实际当前语音不一致的问题，确保与默认值/当前值一一对应。

### 变更内容
- 更新 `PhonoArk/PhonoArk/Services/AudioService.cs`：
	- 新增 `AccentChanged` 事件，在 `CurrentAccent` 变更时通知订阅方。
- 更新 `PhonoArk/PhonoArk/ViewModels/IpaChartViewModel.cs`：
	- 新增 `CurrentAccentDisplay`，显示本地化语音名称（US-Jenny / 美式 / 英式）。
	- 订阅 `AudioService.AccentChanged`，当设置页加载或切换语音后首页标题即时同步。
	- 订阅本地化变更，语言切换时语音显示文案同步刷新。
- 更新 `PhonoArk/PhonoArk/Views/IpaChartView.axaml`：
	- 右上角按钮显示由 `CurrentAccent` 改为 `CurrentAccentDisplay`。
- 更新 `PhonoArk/PhonoArk/App.axaml.cs`：
	- `IpaChartViewModel` 构造注入 `LocalizationService`，支持语音名本地化。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk/PhonoArk.sln`：构建成功（0 Error）。

## 2026-02-24 10:36:00

### 实现目标
- 在混合架构识别相关函数中补充来源致谢注释，明确参考项目并便于后续维护追溯。

### 变更内容
- 更新 `PhonoArk/scripts/android-dev-oneclick.ps1`：
	- 在 `Get-CpuSetCoreClassification` 函数内新增致谢注释，感谢 `Retrieve-IntelCPUCoreEfficiencyClass` 项目并附仓库链接。

### 验证结果（可选）
- PowerShell 脚本编辑器错误检查通过（无报错）。

## 2026-02-24 10:55:00

### 实现目标
- 排查并修复 `FastDebug` 模式偶发“未重新打包却复用旧 APK”的问题。
- 收敛 Android 配置选择逻辑，避免构建目录与取包目录不一致。

### 变更内容
- 更新 `PhonoArk/scripts/android-dev-oneclick.ps1`：
	- 新增参数 `ForceRepackIfStale`（默认 `true`）：FastDebug 下若未检测到本次新 `Signed.apk`，自动执行 `Clean + 重打包`。
	- 修正 `FastDebug` 注释文案，明确当前流程是“生成签名调试包”，非“跳过签名”。
	- 新增 `Invoke-FastDebugPackageBuild`，统一 FastDebug 打包参数，减少重复代码。
	- 新增 `effectiveAndroidConfiguration`：FastDebug + PackageOnly 时强制使用 `Debug`，并在用户传入其他配置时给出警告，防止误读旧目录 APK。
	- `PackageOnly` 取包改为优先选择“本次构建时间窗口内”的 `Signed.apk`；若没有则触发兜底重打包，再次匹配。

### 验证结果（可选）
- `android-dev-oneclick.ps1` 静态检查通过（无脚本错误）。

## 2026-02-24 12:05:00

### 实现目标
- 为一键脚本增加“无参数名称”的场景化快捷入口，支持 `xxx.ps1 1/2/3/4/5/6` 快速启动常用流程。

### 变更内容
- 更新 `PhonoArk/scripts/android-dev-oneclick.ps1`：
	- 新增位置参数 `QuickScenario`（`1~6`）并新增 `Apply-QuickScenarioPreset`。
	- 预设场景映射：
		- `1`：安卓+桌面 Release 交付包
		- `2`：安卓 Debug + 模拟器联调（完整日志）
		- `3`：安卓 Debug + 真机联调（完整日志）
		- `4`：安卓 FastDebug 快速打包（构建冒烟）
		- `5`：安卓 Debug 交付包（仅打包）
		- `6`：仅设备连通性自检（不构建）
	- 新增 `Invoke-DeviceConnectivityCheck`：执行 adb 启动、设备枚举与机型读取，便于快速确认真机/模拟器通信状态。

### 验证结果（可选）
- PowerShell 语法解析通过（无语法错误）。

## 2026-02-24 12:22:00

### 实现目标
- 修复快捷场景参数生效范围问题，确保场景 3（真机联调）不会误启动模拟器。

### 变更内容
- 更新 `PhonoArk/scripts/android-dev-oneclick.ps1`：
	- 在 `Apply-QuickScenarioPreset` 中将场景预设赋值由函数局部变量改为脚本作用域变量（`$script:*`）。
	- 场景 1~6 的 `Mode/Configuration/PackageOnly/FastDebug/NoEmulator/ShowFullLogcat/SkipBuild` 现可正确覆盖主流程参数。

### 验证结果（可选）
- 脚本语法解析通过（OK）。

## 2026-02-24 13:10:00

### 实现目标
- 优化移动端考试页布局体验：减少割裂框体、调整按钮位置与视觉对齐。
- 优化横屏考试页可读性：增加区块间距并收敛选项宽度。
- 提升移动端导航切换跟手性：关闭移动端页面切换动画。

### 变更内容
- 更新 `PhonoArk/PhonoArk/Views/ExamView.axaml`：
	- 将考试中“提示/播放/目标音标/选项/反馈/结束测试”整合到同一主卡片区域。
	- 将“结束测试”按钮移至主内容区底部，符合移动端单路径操作。
	- 选项按钮最大宽度收敛，减少横屏场景下单词按钮过长。
- 更新 `PhonoArk/PhonoArk/Views/ExamView.axaml.cs`：
	- 横屏布局增加左右区块间距与边距，提升观感。
	- 调整设置区卡片行列间距与卡片边距，避免框体贴连。
	- 横竖屏分别设置主内容区 `MaxWidth`，控制选项展示长度。
- 更新 `PhonoArk/PhonoArk/Views/MainView.axaml` 与 `MainView.axaml.cs`：
	- 为内容宿主控件增加命名并在移动端设置 `PageTransition = null`。
	- 桌面端保留较短 `CrossFade`（120ms），移动端关闭切页动画，提升“跟手”感。
- 更新 `PhonoArk/PhonoArk/App.axaml`：
	- `flutter-primary` 增加水平/垂直内容居中，修复“播放音素”文本视觉偏移。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk/PhonoArk.Desktop/PhonoArk.Desktop.csproj -c Debug`：构建成功（0 Error）。

## 2026-02-24 22:10:00

### 实现目标
- 按最新约束完成 Android 与 Windows 图标配置，并接入 Android 大图开屏链路。
- Android 安装后应用名显示为中文“音标方舟”。
- Android 12 采用“纯色系统阶段 -> 应用内大图 SplashActivity”方案，不展示小图标过渡。

### 变更内容
- 更新 `PhonoArk/PhonoArk.Android/PhonoArk.Android.csproj`：
	- Android 图标改为从共享资源 `PhonoArk/Assets/app.png` 统一映射到 `drawable/icon`、`mipmap/ic_launcher`、`mipmap/ic_launcher_round`。
	- 开屏大图资源改为优先使用 `Assets/logo.jpg`，若不存在则自动回退 `Assets/logo.png`。
- 更新 `PhonoArk/PhonoArk.Android/Properties/AndroidManifest.xml`：
	- 应用名改为 `音标方舟`。
	- 图标改为 `@mipmap/ic_launcher`，并补充 `@mipmap/ic_launcher_round`。
- 更新 `PhonoArk/PhonoArk.Android/MainActivity.cs`：
	- 主 Activity 取消 `MainLauncher`，图标改为 `@mipmap/ic_launcher`，与 Manifest 统一。
- 新增 `PhonoArk/PhonoArk.Android/SplashActivity.cs`：
	- 作为启动入口（`MainLauncher=true`）展示全屏 `logo` 大图，再跳转到 `MainActivity`。
	- 先实现单图逻辑，不区分横竖屏资源。
- 更新 `PhonoArk/PhonoArk.Android/Resources/values/styles.xml` 与 `values-v31/styles.xml`：
	- 新增 `SplashTheme`。
	- Android 12+ 下系统阶段仅保留纯色背景并使用透明占位图标，随后直接进入应用内大图。
- 新增资源：
	- `Resources/drawable/splash_icon_blank.xml`
	- `Resources/mipmap-anydpi-v26/ic_launcher.xml`
	- `Resources/mipmap-anydpi-v26/ic_launcher_round.xml`
- 更新 `PhonoArk/PhonoArk.Desktop/PhonoArk.Desktop.csproj`：
	- 设置 `ApplicationIcon=..\PhonoArk\Assets\app.ico`（Windows 可执行文件图标）。
- 更新 `PhonoArk/PhonoArk/Views/MainWindow.axaml`：
	- 窗口图标改为 `/Assets/app.ico`。

### 后续计划（可选）
- 若补齐 `PhonoArk.Android/Assets/logo.jpg`，将自动优先启用 JPG 开屏图，无需再改代码。

## 2026-02-24 22:26:00

### 实现目标
- 修复 Android 开屏顶部黑条问题，确保登录/开屏图为完整全屏大图展示。

### 变更内容
- 更新 `PhonoArk/PhonoArk.Android/Resources/values/styles.xml` 与 `values-v31/styles.xml`：
	- `SplashTheme` 与 `SplashTheme.Post` 增加 `android:windowFullscreen=true`。
- 更新 `PhonoArk/PhonoArk.Android/SplashActivity.cs`：
	- 启动时启用全屏窗口标志。
	- Android 11+ 使用 `WindowInsetsController` 隐藏状态栏与导航栏，避免顶部黑条。
	- 低版本使用沉浸式系统 UI 标志兜底。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk/PhonoArk.sln`：构建成功（0 Error）。

## 2026-02-24 22:40:00

### 实现目标
- 修复 Android 启动闪退（SplashActivity）并保持开屏全屏展示。

### 变更内容
- 定位根因：`SplashActivity` 在部分机型访问 `Window.InsetsController` 时触发空指针，导致 `Unable to start activity`。
- 更新 `PhonoArk/PhonoArk.Android/SplashActivity.cs`：
	- 移除不稳定的 `Window.InsetsController` 调用路径，改为兼容性更稳的沉浸式 UI 标志实现全屏。
	- 保留 `windowFullscreen` + `CenterCrop`，继续保证开屏图全屏铺满。
	- 新增开屏图资源兜底：若 `logo` 资源异常，回退纯白背景，避免启动崩溃。

### 验证结果（可选）
- 执行 `dotnet build PhonoArk/PhonoArk.sln`：构建成功（0 Error）。

## 2026-02-24 22:55:00

### 实现目标
- 完全按 `scripts/参考/ReadStorm.Android` 的成功方案重做 Android 启动链路，消除 SplashActivity 启动闪退风险。

### 变更内容
- 更新 `PhonoArk/PhonoArk.Android/SplashActivity.cs`：
	- 改为参考项目同款实现：`MyTheme.Launch` 启动主题 + `ImageView(Matrix)` 全屏裁切。
	- 增加 `IsTaskRoot` 启动防重入判断与延时跳转主页机制（`ClearTop | SingleTop`）。
	- 移除此前不稳定的 `InsetsController` 访问路径。
- 更新 `PhonoArk/PhonoArk.Android/Resources/values/styles.xml`：
	- 对齐为 `MyTheme.NoActionBar` 与 `MyTheme.Launch` 双主题结构。
	- 使用 `@drawable/launch_background` 作为启动背景来源。
- 更新 `PhonoArk/PhonoArk.Android/Resources/values-v31/styles.xml`：
	- 对齐 Android 12+ 启动主题与 `windowSplashScreen*` 配置（透明占位图标）。
- 新增 `PhonoArk/PhonoArk.Android/Resources/values-v28/styles.xml`：
	- 对齐参考项目在 API 28+ 的刘海与系统栏样式策略。
- 新增 `PhonoArk/PhonoArk.Android/Resources/drawable/launch_background.xml`：
	- 统一系统启动阶段纯色背景。
- 更新 `PhonoArk/PhonoArk.Android/Resources/values/colors.xml`：
	- 增加 `abc_decor_view_status_guard*` 透明覆盖，减少顶部细线/黑条机型差异。
- 更新 `PhonoArk/PhonoArk.Android/Properties/AndroidManifest.xml`：
	- `application` 显式指定 `@style/MyTheme.NoActionBar`，与参考方案一致。

### 验证结果（可选）
- `SplashActivity.cs` 与相关样式/清单文件错误检查通过（无错误）。

## 2026-02-25 10:20:00

### 实现目标
- 实现构建期全自动语音包解压：基于 `Data/US-Jenny.zip` 自动解压到 `Data/Exportfile/US-Jenny`。
- 加入一致性校验，命中一致时跳过解压，避免重复 IO 与构建时间浪费。
- 将运行时语音路径切换到 `Data/Exportfile`，匹配新的资源工作流。

### 变更内容
- 更新 `PhonoArk/PhonoArk/PhonoArk.csproj`：
	- 新增 `EnsureVoicePackageTask`（MSBuild 内联任务），在构建前执行：
		- 检查 `Data/US-Jenny.zip` 是否存在；
		- 计算 zip 指纹（zip 文件元信息 + 条目数 + 条目哈希）；
		- 对比 `Data/Exportfile/US-Jenny.marker` 与目标目录文件数；
		- 一致则跳过，不一致则自动清理并重新解压；
		- 解压后做文件数校验，失败则中断构建。
	- 构建期动态将 `Data/Exportfile/US-Jenny/**` 注入 `AvaloniaResource` 与输出复制，确保桌面/安卓可用。
	- `Data/**` 资源扫描排除 `Data/Exportfile/**`，避免与动态注入重复。
	- 移除旧的 `Data/US-Jenny/**` 直接复制策略，改为保留并复制 `Data/US-Jenny.zip`。
- 更新 `PhonoArk/PhonoArk/Services/PhonemeDataService.cs`：
	- `US-Jenny` 音标与单词映射路径由 `Data/US-Jenny/...` 调整为 `Data/Exportfile/US-Jenny/...`。
- 更新 `PhonoArk/.gitignore`：
	- 明确忽略 `PhonoArk/PhonoArk/Data/Exportfile/` 及 marker 文件，避免大量解压小文件进入仓库。

### 验证结果（可选）
- 已完成代码级接入，待执行一次 `dotnet build` 验证构建期解压与跳过逻辑。

### 后续计划（可选）
- 将当前单包流程抽象为“多语音包”通用模板（一个 zip 对应一个目标目录），支持后续批量扩展。

## 2026-02-25 11:05:00

### 实现目标
- 将 `US-Jenny` 解压判定从纯 marker 字符串升级为可读 JSON 清单（含文件列表），便于长期维护与多包扩展。

### 变更内容
- 更新 `PhonoArk/PhonoArk/PhonoArk.csproj`：
	- 构建期清单文件由 `US-Jenny.marker` 升级为 `US-Jenny.manifest.json`。
	- 清单内容改为确定性 JSON：包含包名、zip 文件名、zip 签名、条目总数、逐条目路径/大小/时间戳。
	- 跳过解压条件改为“清单完全一致 + 文件总数一致 + 目录结构完整（phonemes/words）”。
	- 不一致时自动重解压并重写清单。
- 更新 `PhonoArk/.gitignore`：
	- 增加 `Data/Exportfile/**/*.manifest.json` 忽略规则（与 Exportfile 整体忽略策略一致）。

### 验证结果（可选）
- 进入打包测试阶段（桌面/安卓）。

## 2026-02-25 11:28:00

### 实现目标
- 完成清单升级后的桌面/安卓打包验证，并修复发布过程中的重复输出冲突。

### 变更内容
- 更新 `PhonoArk/PhonoArk/PhonoArk.csproj`：
	- 去除 `EnsureUsJennyVoicePackage` 中动态注入资源的重复 `None` 输出项，仅保留 `AvaloniaResource`，修复 `NETSDK1152` 重复发布文件错误。

### 验证结果（可选）
- 桌面打包：
	- `dotnet publish PhonoArk/PhonoArk.Desktop/PhonoArk.Desktop.csproj -c Release -r win-x64 --self-contained false` 成功。
	- 产物目录：`PhonoArk/PhonoArk.Desktop/bin/Release/net10.0/win-x64/publish/`。
- 安卓打包：
	- `dotnet publish PhonoArk/PhonoArk.Android/PhonoArk.Android.csproj -c Debug -f net10.0-android /p:AndroidPackageFormat=apk` 成功（含既有 2 条 Android 警告）。
	- `dotnet build PhonoArk/PhonoArk.Android/PhonoArk.Android.csproj -c Debug -f net10.0-android /p:AndroidPackageFormat=apk /t:SignAndroidPackage` 成功。
	- APK 产物：`PhonoArk/PhonoArk.Android/bin/Debug/net10.0-android/com.CompanyName.PhonoArk-Signed.apk`。

	## 2026-02-25 12:05:00

	### 实现目标
	- 修正 Windows 打包行为：发布产物应包含解压后的真人语音目录，而不是仅携带 zip。

	### 变更内容
	- 更新 `PhonoArk/PhonoArk/PhonoArk.csproj`：
		- 保持构建前自动解压与清单校验逻辑。
		- `Data/US-Jenny.zip` 继续从打包资源中排除（不进发布产物）。
		- 保留共享工程构建输出复制逻辑，不再通过发布项收集注入，避免冲突与重复路径问题。
	- 更新 `PhonoArk/PhonoArk.Desktop/PhonoArk.Desktop.csproj`：
		- 新增桌面头项目的输出/发布后复制目标：
			- `CopyUsJennyToDesktopOutput`
			- `CopyUsJennyToDesktopPublish`
		- 将 `../PhonoArk/Data/Exportfile/US-Jenny/**` 复制到桌面产物目录 `Data/Exportfile/US-Jenny/**`。

	### 验证结果（可选）
	- 执行 `dotnet publish PhonoArk.Desktop/PhonoArk.Desktop.csproj -c Release -r win-x64 --self-contained false`：成功。
	- 发布目录确认：
		- 存在 `Data/Exportfile/US-Jenny/phonemes` 与 `Data/Exportfile/US-Jenny/words`。
		- 不存在 `US-Jenny.zip`。

## 2026-02-25 16:08:47

### 实现目标
- 简化 AvaloniaResource 排除规则，使 Android 端可通过 `avares://` 访问解压后的真人语音文件。
- 为未来多语音包扩展提供通配符支持。

### 变更内容
- 更新 `PhonoArk/PhonoArk/PhonoArk.csproj`：
	- AvaloniaResource 排除规则由 `Exclude="Data\Exportfile\**;Data\US-Jenny.zip"` 简化为 `Exclude="Data\*.zip"`。
	- 移除对 `Data\Exportfile\**` 的排除后，解压后的 wav 文件被嵌入为 AvaloniaResource，Android 端可通过 `avares://PhonoArk/Data/Exportfile/US-Jenny/...` 访问。
	- 使用 `Data\*.zip` 通配符排除所有 zip 包，为后续新增语音包预留扩展性。
- 平台音频访问策略明确为：
	- Windows：通过 MSBuild Copy 目标将文件复制到输出目录，运行时走磁盘文件路径。
	- Android：通过 AvaloniaResource 嵌入 DLL，运行时走 `avares://` 资源路径。

### 验证结果（可选）
- Desktop Release 构建：0 errors, 0 warnings。
- Desktop Release 发布：phonemes/ ✅、words/ ✅、291 文件 ✅、无 zip ✅。
- Android Debug APK 构建：0 errors, 2 pre-existing warnings。
## 2026-02-25 16:17:04

### 实现目标
- 修复首次 clean build（`Data/Exportfile/` 不存在时）Android APK 静默缺少嵌入音频资源的问题。
- 确保新开发者 clone 后第一次构建即可得到完整产物。

### 变更内容
- 更新 `PhonoArk/PhonoArk/PhonoArk.csproj`：
	- AvaloniaResource 静态 glob 恢复排除 Exportfile 目录：`Exclude="Data\*.zip;Data\Exportfile\**"`，避免 glob 在解压前求值时匹配到空目录。
	- 在 `EnsureUsJennyVoicePackage` 解压目标执行后，动态注入 `<AvaloniaResource Include="Data\Exportfile\US-Jenny\**" />`，确保无论是否首次构建，资源列表都包含解压后的 wav 文件。

### 验证结果（可选）
- 删除 `Data/Exportfile/` 后 Desktop clean build：成功，291 文件正常解压。
- 删除 `Data/Exportfile/` 后 Android clean build：成功，0 errors。
- 增量 build：与此前一致，无额外耗时。

## 2026-02-25 16:32:53

### 实现目标
- 消除 Android 构建的 2 个编译警告，实现 0 warnings 构建。

### 变更内容
- 更新 `PhonoArk/PhonoArk.Android/Services/AndroidTtsBridge.cs`：
	- 将 `SetAudioStreamType(Stream.Music)` 替换为 `SetAudioAttributes(AudioAttributes)`，消除 CA1422 警告（`SetAudioStreamType` 在 API 26+ 过时）。
	- 改用 `AudioUsageKind.Media` + `AudioContentType.Music` 的 Builder 模式，适配 Android 12-15。
- 更新 `PhonoArk/PhonoArk.Android/SplashActivity.cs`：
	- 移除重复的 `using Android.Content.PM;`，消除 CS0105 警告。
- 清理 `publish/desktop/release/Data/US-Jenny` 空残留目录（旧发布产物，非构建产出）。

### 验证结果（可选）
- Android Release 构建：0 errors, 0 warnings。

## 2026-02-24 23:23:27

### 实现目标
- 在根目录新增 purecodex，搭建安卓原生重构基础框架，并对齐现有 App 的页面入口结构。

### 变更内容
- 新增 `purecodex/` 目录，使用 Gradle Kotlin DSL 搭建原生 Android 应用骨架。
- 新增基于 Jetpack Compose + Navigation 的主界面与底部导航，提供音标图表/收藏/测验/历史/设置五个入口页面。
- 新增 `purecodex/README.md`，说明与原 Avalonia 版本页面对应关系及后续迁移方向。

### 验证结果（可选）
- 变更前基线验证：`dotnet test newapp/tests/PhonoArk.Mobile.Tests/PhonoArk.Mobile.Tests.csproj` 通过（89/89）。
- 变更前基线构建：`dotnet build PhonoArk/PhonoArk.Desktop/PhonoArk.Desktop.csproj` 失败（现存 MainView.axaml AVLN3000，非本次变更引入）。
- 变更后回归验证：`dotnet test newapp/tests/PhonoArk.Mobile.Tests/PhonoArk.Mobile.Tests.csproj` 通过（89/89）。
- 原生工程验证：`gradle -p purecodex help` 在当前环境无法解析 Android Gradle Plugin（仓库依赖不可达），已确认目录结构与关键源码文件创建完成。

## 2026-02-25 00:02:43

### 实现目标
- 将 purecodex 从占位骨架推进到接近 1:1 的可运行安卓原生实现，覆盖 IPA/收藏/测验/历史/设置核心流程。

### 变更内容
- 新增原生数据与状态层：`model/Models.kt`、`data/AppRepository.kt`、`ui/AppViewModel.kt`。
- 将 `phoneme-word-bank.json` 复制到 `purecodex/app/src/main/assets/` 并在原生端加载。
- 重写 `MainActivity.kt`，集成 `AppViewModel` 与 Android `TextToSpeech`。
- 新增 `ui/AppUi.kt`，实现五大页面的 Compose 界面与交互：
  - IPA：分类筛选、详情、收藏、发音
  - 收藏：列表、取消、清空
  - 测验：出题、答题、反馈、完成入历史
  - 历史：成绩与高频错误统计、清空
  - 设置：口音/音量/题量/深色模式/语言持久化
- 更新 `purecodex/app/build.gradle.kts` 依赖（lifecycle compose 相关）。
- 更新 `purecodex/README.md`，补充当前实现能力说明。

### 验证结果（可选）
- 回归：`dotnet test newapp/tests/PhonoArk.Mobile.Tests/PhonoArk.Mobile.Tests.csproj` 通过（89/89）。
- 说明：当前环境无法解析 Android Gradle Plugin 依赖，未能完成 `purecodex` 的本地 Gradle 构建与 UI 运行截图。

### 后续计划（可选）
- 在可用 Android SDK/仓库网络环境中执行 `gradle -p purecodex assembleDebug`，补充运行截图与交互验收。
- 继续细化 UI 视觉（间距/色板/动效）及音频映射策略，进一步贴近原 Avalonia 版本。

## 2025-02-25 17:02:34

### 实现目标
- 修复 Android 历史记录页频繁崩溃问题（EF Core DbContext 并发访问 + 跨线程 UI 更新 + 滚动加载竞态）

### 变更内容
- **ExamHistoryService 改为短生命周期 DbContext 模式**（P0）
  - 存储 `DbContextOptions<AppDbContext>` 替代单一 `AppDbContext` 实例
  - 每个公共方法内部使用 `using var context = CreateContext()` 创建独立上下文
  - `EnsureQuestionAttemptsTable` 改为 `static` 方法，接受 `AppDbContext` 参数
  - 根除 `Task.WhenAll` 三并发查询、写入后事件触发读取、滚动加载、详情懒加载等路径的 DbContext 并发冲突
- **App.axaml.cs 传递 `optionsBuilder.Options` 而非 `new AppDbContext(...)`**（P0）
- **ExamHistoryViewModel 回调分派到 UI 线程**（P1）
  - `OnHistoryChanged` 用 `Dispatcher.UIThread.Post` 包裹
  - 语言切换 `PropertyChanged` 回调同样用 `Dispatcher.UIThread.Post` 包裹
  - 防止非 UI 线程修改 `ObservableCollection` 导致 Android 崩溃
- **LoadMoreSessionsAsync 加 `_loadGate` 保护**（P1）
  - 使用 `_loadGate.WaitAsync(0)` 非阻塞尝试获取信号量
  - 若 `ReloadHistoryAsync` 正在执行则直接跳过，消除滚动加载与全量刷新间的竞态

### 验证结果
- Desktop Release 构建：0 错误 0 警告 ✅
- Android Release 构建：0 错误 0 警告 ✅

## 2026-02-25 18:30:00

### 实现目标
- 统一 DbContext 生命周期管理，消除 FavoriteService / SettingsService 的长生命周期单例 DbContext 脏跟踪风险。
- 消除 fire-and-forget 异步（`_ = SomeAsync()`）静默吞异常问题，统一使用 SafeFireAndForget 扩展方法。
- 在 IPA 首页新增批量收藏快捷按钮（收藏/取消元音、双元音、辅音、全部清空），支持切换状态且 PC / 手机复用。

### 变更内容
- **DbContext 生命周期修复**
  - `FavoriteService`：构造参数由 `AppDbContext` 改为 `DbContextOptions<AppDbContext>`，每个公共方法内部 `using var context = CreateContext()` 创建独立上下文。
  - `SettingsService`：同上，每次操作创建新 Context，迁移方法内也使用独立 Context。
  - `App.axaml.cs`：传递 `optionsBuilder.Options` 而非 `new AppDbContext(...)`，与 ExamHistoryService 保持一致。
- **Fire-and-forget 安全包裹**
  - 新增 `TaskExtensions.cs`，提供 `SafeFireAndForget(callerHint)` 扩展方法，捕获并 Debug 输出异常（忽略 OperationCanceledException）。
  - 替换 9 处 `_ = SomeAsync()` 为 `.SafeFireAndForget("hint")`：App.axaml.cs、IpaChartViewModel、FavoritesViewModel、ExamHistoryViewModel（4 处）、AudioService（2 处）。
- **IPA 首页批量收藏按钮**
  - `IpaChartViewModel` 新增：`AllVowelsFavorited` / `AllDiphthongsFavorited` / `AllConsonantsFavorited` 状态属性，`ToggleFavoriteVowelsCommand` / `ToggleFavoriteDiphthongsCommand` / `ToggleFavoriteConsonantsCommand` / `ClearAllFavoritesCommand` 四个命令，`RefreshBatchFavoriteStates()` 辅助方法。
  - `IpaChartView.axaml`：在元音区上方新增 `flutter-card` 容器，内含 4 个右对齐按钮，使用 `flutter-secondary` 样式 + `.active` 变体高亮已全选状态。
  - `App.axaml`：新增 `Button.flutter-secondary.active` 样式（金色高亮，与收藏主题一致）。
  - `Strings.resx` / `Strings.zh-CN.resx`：新增 FavToggleVowels / FavToggleDiphthongs / FavToggleConsonants / FavClearAll 四组国际化文案。

### 验证结果
- Desktop Release 构建：0 错误 0 警告 ✅
- Android Release 构建：0 错误 0 警告 ✅
- IDE 静态分析：0 错误 ✅

## 2026-02-25 23:50:00

### 实现目标
- 将应用显示名称从英文 "PhonoArk" 统一改为中文 "音标方舟"
- 点击 IPA 音标时自动播放对应发音

### 变更内容
- **应用名称统一为"音标方舟"**
  - `MainWindow.axaml`：桌面窗口标题 `Title` 改为 "音标方舟"（影响 Windows 任务栏与窗口顶部）。
  - `Strings.resx`：英文资源 `AppName` 改为 "音标方舟"。
  - `Strings.zh-CN.resx`：中文资源 `AppName` 改为 "音标方舟"。
  - Android 端（`AndroidManifest.xml` / `MainActivity.cs`）已是 "音标方舟"，无需变更。
- **IPA 首页点击音标自动发声**
  - `IpaChartViewModel.SelectPhonemeAsync`：在选中音标并加载收藏状态后，追加 `await _audioService.PlayPhonemeAsync(phoneme)` 调用，复用已有的真人录音 → TTS 回退链路，无需新增任何服务或接口。

### 验证结果
- Desktop Release 构建：0 错误 0 警告 ✅
- Android Release 构建：0 错误 0 警告 ✅

## 2026-02-25 24:10:00

### 实现目标
- 移除独立的"收藏"选项卡（导航入口 + 视图 + ViewModel），收藏功能仍保留在 IPA 首页中
- 恢复上一轮误删的"历史记录"选项卡

### 变更内容
- **移除收藏选项卡**
  - `MainView.axaml`：桌面侧栏和移动端底栏删除收藏按钮，移动端底栏列数 5→4。
  - `MainView.axaml.cs`：删除 `OnFavoritesClick` 事件处理。
  - `MainViewModel.cs`：删除 `IsFavoritesSelected`、`FavoritesViewModel` 属性、`NavigateToFavorites()` 方法及相关状态更新。
  - `App.axaml.cs`：删除 `FavoritesViewModel` 实例化及传参。
  - 删除文件：`FavoritesView.axaml`、`FavoritesView.axaml.cs`、`FavoritesViewModel.cs`。
  - `FavoriteService` 保留不动（`IpaChartViewModel` 和 `ExamService` 仍依赖它）。
- **恢复历史记录选项卡**（上一轮误删）
  - `MainViewModel.cs`：恢复 `IsHistorySelected`、`ExamHistoryViewModel` 属性、`NavigateToHistory()`、`SwitchTo` 离开时 `OnViewDeactivated()` 调用。
  - `MainView.axaml`：恢复桌面侧栏历史记录按钮、移动端底栏历史记录按钮、DataTemplate 映射。
  - `MainView.axaml.cs`：恢复 `OnHistoryClick` 事件处理。
  - `App.axaml.cs`：恢复 `ExamHistoryViewModel` 实例化及传参。

### 验证结果
- Desktop Release 构建：0 错误 0 警告 ✅
- Android Release 构建：0 错误 0 警告 ✅

## 2026-02-25 22:00:00

### 实现目标
- 生成 PhonoArk 桌面端项目介绍 PPT 演示文稿

### 变更内容
- 新增 `docs/generate_ppt.py`：基于 python-pptx 的 PPT 生成脚本
- 生成 `docs/PhonoArk桌面端介绍.pptx`：12 页完整项目介绍演示文稿
  - 封面页：项目名称与技术栈标识
  - 目录页：8 个章节概览
  - 项目概览：产品定位、核心价值、亮点数据
  - 功能特性（IPA 学习）：交互式图表、双口音、收藏系统
  - 功能特性（练习考试）：随机测试、进度追踪、错题统计
  - 技术架构：技术栈 6 项 + MVVM 分层说明
  - 项目结构：代码组织与模块统计
  - 数据模型：4 个核心实体说明
  - 国际化与主题：双语支持 + 深色/浅色主题
  - 构建与运行：前置条件 + 4 步构建流程
  - 路线图：近期计划与远期愿景
  - 感谢页

## 2026-02-25 19:30:00

### 实现目标
- 修复 Android 端点击"历史记录"页 100% 闪退的致命崩溃问题

### 变更内容

#### 根因定位

该崩溃是 **Avalonia 11.3.x 的已知框架 Bug**（对应 GitHub Issue [#20453](https://github.com/AvaloniaUI/Avalonia/issues/20453)，由 PR [#19985](https://github.com/AvaloniaUI/Avalonia/pull/19985) 引入，PR [#19991](https://github.com/AvaloniaUI/Avalonia/pull/19991) 在 master 修复但尚未包含在 11.3.12 正式 release 中）。

**崩溃堆栈**：
```
FATAL EXCEPTION: main
android.runtime.JavaProxyThrowable:
  [System.Reflection.TargetException]:
  Object type AndroidX.Core.View.Accessibility.AccessibilityNodeInfoCompat
  does not match target type
  Avalonia.Android.Automation.ToggleNodeInfoProvider.

  at Avalonia.Android.Automation.ToggleNodeInfoProvider.PopulateNodeInfo
  at Avalonia.Android.AvaloniaAccessHelper.OnPopulateNodeForVirtualView
  at ExploreByTouchHelper.createNodeForChild
  at ExploreByTouchHelper.obtainAccessibilityNodeInfo
  at AccessibilityInteractionController$AccessibilityNodePrefetcher
      .prefetchDescendantsOfVirtualNode
```

**Bug 本质**：`ToggleNodeInfoProvider.PopulateNodeInfo` 内部使用反射设置 `AccessibilityNodeInfoCompat.Checked` 属性时，错误地将 `this`（`ToggleNodeInfoProvider` 实例）作为 `PropertyInfo.SetValue` 的第一个参数传入，而正确的 target 应该是 `nodeInfo`（`AccessibilityNodeInfoCompat` 实例）。

**触发条件**（缺一不可）：
1. 页面包含 `CheckBox` 或 `ToggleSwitch` 控件（本项目中历史记录页的"仅显示错题"CheckBox、设置页的"暗色模式"和"学习提醒"ToggleSwitch）
2. Android 系统无障碍服务（Accessibility Service）正在运行并枚举 View 树的虚拟节点
3. 使用 Avalonia 11.3.9 ~ 11.3.12（包含该反射 Bug 的版本范围）

**该问题难以发现的原因**：
- 仅在 **Android 真机** 上触发，桌面端完全正常
- 需要设备上有 **活跃的无障碍服务**（如 MIUI 的 FindDevice、屏幕阅读器等，许多国产 ROM 默认开启）
- 崩溃堆栈指向 Avalonia 框架内部（`ToggleNodeInfoProvider`），与业务代码无直接关系，开发者容易误判为自身代码问题
- 纯 IPA 图表页或考试页（无 Toggle 类控件）不会触发

#### 修复方案

修改 `PhonoArk.Android/MainActivity.cs`，在 `OnCreate` 和 `OnAttachedToWindow` 两个时机遍历 Android View 树，移除 Avalonia 注册的无障碍委托：

```csharp
private void SuppressAvaloniaAccessibility()
{
    if (Window?.DecorView is ViewGroup decor)
        WalkAndClearAccessibilityDelegate(decor);
}

private static void WalkAndClearAccessibilityDelegate(ViewGroup parent)
{
    for (var i = 0; i < parent.ChildCount; i++)
    {
        var child = parent.GetChildAt(i);
        if (child == null) continue;
        child.SetAccessibilityDelegate(null);
        child.ImportantForAccessibility = ImportantForAccessibility.NoHideDescendants;
        if (child is ViewGroup group)
            WalkAndClearAccessibilityDelegate(group);
    }
}
```

**为什么必须遍历 View 树而非只设 DecorView**：
Avalonia 的 `AvaloniaAccessHelper`（继承 `ExploreByTouchHelper` → `AccessibilityDelegateCompat`）通过 `ViewCompat.SetAccessibilityDelegate(avaloniaView, helper)` 直接挂载到渲染视图 `AvaloniaView` 上。Android 无障碍框架会通过该 delegate 的 `AccessibilityNodeProvider` 直接查询虚拟节点，**完全绕过**父视图（DecorView）的 `ImportantForAccessibility` 标志。因此必须找到 `AvaloniaView` 本身并移除其 delegate。

#### 排查过程记录

| 步骤 | 操作 | 结果 |
|------|------|------|
| 1 | 真机部署 Debug APK，用户复现闪退 3 次 | 确认 100% 可复现 |
| 2 | `adb logcat` 抓取崩溃日志 | 定位到 `ToggleNodeInfoProvider.PopulateNodeInfo` TargetException |
| 3 | 查 Avalonia GitHub Issue 确认为已知 Bug #20453 | master 已修复，11.3.12 未包含 |
| 4 | 尝试方案 A：`DecorView.ImportantForAccessibility = NoHideDescendants` | ❌ 无效（delegate 绕过） |
| 5 | 尝试方案 B：`FindViewById(Android.Resource.Id.Content)` | ❌ .NET Android 绑定中 `ResourceConstant.Id` 不含 `Content` |
| 6 | 尝试方案 C：遍历 View 树 + `SetAccessibilityDelegate(null)` | ✅ 成功，闪退彻底消失 |

#### 副作用与后续

- **副作用**：应用内 Android TalkBack 等屏幕阅读器将无法朗读 Avalonia UI 元素。对于当前产品定位（音标学习工具）影响极小。
- **TODO**：待 Avalonia 发布包含 PR #19991 修复的正式版本后，移除此变通方案，恢复无障碍支持。

### 验证结果
- Debug APK 构建成功（0 错误 0 警告）
- 真机（Xiaomi duchamp, MIUI, Mediatek）部署后反复测试历史记录页、设置页，均稳定无闪退
- 所有包含 CheckBox/ToggleSwitch 的页面功能正常

## 2026-02-25 20:30:00

### 实现目标
- 为 android-dev-oneclick.ps1 新增 `PhysicalCoresOnly` 开关，支持禁用超线程（HT）以减少编译时缓存争抢

### 变更内容
- 新增参数 `[bool]$PhysicalCoresOnly = $true`，默认启用
- 新增函数 `Get-OnePerPhysicalCore`：通过 CpuSet CoreIndex 分组，每个物理核只保留 1 个逻辑线程
- 修改 `Initialize-PCorePreference`：当 PhysicalCoresOnly 开启时，对 P 核逻辑线程列表执行去 HT 过滤
- 改进构建日志输出：显示绑定线程数与 HT 状态，便于诊断调优
- 日志中 `策略=Floor(物理核*0.85)` 更新为 `策略=Floor(物理核*1)` 以匹配已有代码

## 2026-02-25 21:15:00

### 实现目标
- 新增 `Arm64Only` 参数，控制是否跳过 x86_64 架构打包以加速 Release 构建

### 变更内容
- 新增参数 `[bool]$Arm64Only = $true`，默认仅打包 arm64（真机），跳过 x86_64（模拟器）
- 在 `Get-BuildTuningArgs` 中注入 `-p:RuntimeIdentifiers=android-arm64`，三个构建入口统一生效
- 构建日志显示 ABI 打包范围信息，便于确认当前策略

## 2026-02-26 02:30:00

### 实现目标
- 对 `android-dev-oneclick.ps1`（v1，1107 行）进行结构化重构，生成 `android-dev-oneclick-v2.ps1`（1010 行），提升可读性与可维护性

### 变更内容
- 新增 `PhonoArk/scripts/android-dev-oneclick-v2.ps1`，保留 v1 全部功能，结构优化如下：
  - 添加 PowerShell 注释帮助头（`.SYNOPSIS` / `.DESCRIPTION` / `.PARAMETER` / `.EXAMPLE`）
  - 按职责划分为 9 个 `#region` 段：日志工具、快捷场景预设、CPU 拓扑与 P 核亲和性、构建与发布、Android SDK/ADB/模拟器、APK 查找、构建阶段、部署联调、模拟器启动
  - 新增 `$script:CachedCpuTopology` 缓存，避免 `Get-CpuTopologyInfo` 在同一次运行中重复查询 WMI 与 CpuSet API
  - 提取 `Find-BuiltApk` 函数：统一 APK 查找逻辑（含 FastDebug Clean+重打包兜底）
  - 提取 `Invoke-BuildStage` 函数：封装 CPU 信息输出 + P 核初始化 + 构建执行
  - 提取 `Invoke-DeployAndDebug` 函数：封装安装 + 启动 + 日志过滤
  - 提取 `Start-EmulatorIfNeeded` 函数：封装模拟器启动判断与等待
  - 主流程精简为 10 个编号步骤，清晰可读
- v1 原文件保持不动，可随时对比回退

## 2026-02-25 23:50:00

### 实现目标
- 将 v2 脚本的参数系统从 6 个固定场景预设改为 3 位置参数自由组合 + 无参数交互式菜单

### 变更内容
- 移除 `$QuickScenario`、`$Mode`、`$PackageOnly`（switch）、`$SkipBuild`（switch）、`$NoEmulator`（switch）、`$ShowFullLogcat`（switch）、`$FastDebug`（bool param）、`$Configuration`（string param）等作为脚本参数
- 新增 3 个位置参数：`$Platform`（1=Android/2=Desktop/3=Both）、`$Config`（1=Debug/2=Release）、`$Action`（1=模拟器联调/2=真机联调/3=仅打包/4=FastDebug/5=设备自检）
- 新增交互式菜单系统：`Show-InteractiveMenu` + `Read-Choice`，无参数运行时逐步引导用户选择
- 新增 `Resolve-Choices` 函数：将位置参数/交互选择统一映射到 `$script:` 运行时变量
- 主流程从 10 步精简到 9 步，移除场景预设步骤
- 用法示例：`.\script.ps1 1 1 2`（Android+Debug+真机）或直接 `.\script.ps1` 进入交互模式

## 2026-02-25 14:30:00

### 实现目标
- 消除 v2 脚本在 VS Code 中的 196 条 PowerShell 扩展误报红字

### 变更内容
- 将 C# P/Invoke 互操作代码从内联 here-string 提取到外部文件 `scripts/CpuSetNative.cs`，`Ensure-CpuSetNativeType` 改用 `Add-Type -Path`
- 含 `(exit=...)` 括号的双引号字符串改为 `-f` 格式化操作符（`Run-Dotnet`、模拟器启动失败）
- 含 `$()` 子表达式的双引号字符串改为字符串拼接（CPU 标签、APK 路径输出、P 核绑定日志等）
- `Read-Choice` 中的 `$input`（PowerShell 自动变量）重命名为 `$choice`，消除语义冲突
- 确认行为模式下的双引号字符串改为 `-f` 格式化操作符
- 不需要变量插值的字符串从双引号改为单引号（函数默认值、空字符串等）

### 验证结果
- PowerShell 原生解析器 `[Parser]::ParseFile` 确认零语法错误
- VS Code 诊断由 196 条降至 0 条（重启扩展后生效）
- `.\android-dev-oneclick-v2.ps1 1 1 3` 实际运行成功：构建 4.2s，APK 13.8 MB

## 2026-02-25 23:00:00

### 实现目标
- 将 `README.md` 和 `RELEASE_NOTES.md` 从 ReadStorm（阅读风暴）模板内容更新为 PhonoArk（音标方舟）实际项目信息

### 变更内容
- 重写 `README.md`：项目名更正为 PhonoArk（音标方舟），功能模块表对齐实际代码（IPA 音标图表、多口音发音、听音辨词测试、历史记录与错题统计、收藏管理、语音诊断、设置），架构目录反映真实项目结构（含 purecodex / pureopus / tools 子项目），补充语音包构建机制说明
- 重写 `RELEASE_NOTES.md`：版本更新日志替换为 PhonoArk 实际开发里程碑（i18n 迁移、TTS 引擎接入、US-Jenny 真人发音、Android 无障碍修复、批量收藏、考试页适配、构建脚本 v2 等），平台包名与可执行文件名统一为 PhonoArk

## 2026-02-25 23:30:00

### 实现目标
- 在设置页最顶部增加 GitHub 项目地址与版本号显示
- 版本号从 `RELEASE_NOTES.md` 第一行动态提取，构建期注入程序集

### 变更内容
- `PhonoArk.csproj`：新增 MSBuild Target `SetVersionFromReleaseNotes`，构建前解析 `RELEASE_NOTES.md` 第一行版本号（正则匹配 `v?X.Y.Z`），设为 `InformationalVersion`
- `ViewModels/SettingsViewModel.cs`：新增 `AppVersion` 属性（从 `AssemblyInformationalVersionAttribute` 读取，截掉 `+` 后缀）、`GitHubUrl` 常量属性、`OpenGitHubCommand` 命令（跨平台打开浏览器）
- `Views/SettingsView.axaml`：设置页标题下方、第一个配置卡片前新增"版本 + GitHub 链接"卡片
- `Resources/Strings.resx` 与 `Strings.zh-CN.resx`：新增 `VersionLabel` 资源键（English: "Version" / 中文: "版本"）

### 验证结果
- 构建成功（0 Error, 0 Warning）
- 程序集 `ProductVersion` 确认为 `1.1.0`（从 RELEASE_NOTES.md 动态读取）

## 2026-02-26 00:00:00

### 实现目标
- 修复 PhonoArk.Android 构建失败（`XAFLT7000: System.BadImageFormatException: Missing data directory`）

### 变更内容
- 诊断发现 `PhonoArk\bin\Debug\net10.0\PhonoArk.dll` 残留了损坏的构建产物，Android `FilterAssemblies` 任务尝试读取其 PE 元数据时因缺少数据目录而失败
- 执行 `dotnet clean` 清除 PhonoArk 共享项目及 PhonoArk.Android 项目的构建缓存，重新构建后 DLL 恢复为有效 .NET 程序集
- 无代码变更，属于构建缓存污染问题

### 验证结果
- `dotnet build PhonoArk.csproj -c Debug`：成功（0 Error）
- `dotnet build PhonoArk.Android.csproj -c Debug /t:SignAndroidPackage`：成功（0 Error）
- `[AssemblyName]::GetAssemblyName()` 验证 PhonoArk.dll 为有效程序集（Version 1.0.0.0）

## 2026-02-26 01:00:00

### 实现目标
- 修复安卓端点击设置页 GitHub 链接闪退问题

### 变更内容
- `ViewModels/SettingsViewModel.cs`：新增静态属性 `PlatformOpenUri`（`Action<Uri>?`），`OpenGitHubCoreAsync` 优先使用该平台回调打开 URI，桌面端仍回退到 Avalonia `TopLevel.Launcher`
- `PhonoArk.Android/MainActivity.cs`：新增 `ConfigureUriLauncher()` 方法，在 `CustomizeAppBuilder` 中注册原生 `Intent.ActionView` 实现，使用 Android Intent 启动外部浏览器
- 参考 `docs/参考/ReadStorm.Android` 的 `AndroidSystemUiBridge` 平台桥接模式，与项目已有的 `AndroidTtsBridge` 静态回调模式保持一致

### 验证结果
- `dotnet build PhonoArk.Android.csproj -c Debug /t:SignAndroidPackage`：成功（0 Error）

## 2026-02-26 14:30:00

### 实现目标
- 解决安卓增量构建残留导致 APK 闪退的问题，为一键脚本增加"强制清理"选项

### 变更内容
- `scripts/android-dev-oneclick-v2.ps1`：新增 `-ForceClean` 参数（`$null`=交互询问 / `$true` / `$false`），支持命令行和交互模式
- 交互菜单新增第 4 步"增量 / 全量构建"选择，仅构建动作（非设备自检/仅灌装）生效
- 强制清理逻辑：删除 Android 项目、共享项目的 `bin`/`obj` 目录 + `dotnet clean`，确保全量重构建
- 确认行新增 `| 强制清理` 标识，方便确认当前构建策略
