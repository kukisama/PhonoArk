# msspeechcmd

基于 **Microsoft Azure Cognitive Services Speech SDK** 的命令行文字转语音（TTS）工具。  
使用 C# (.NET 10) 编写，支持并发合成、WAV/MP3 输出、SSML 输入，以及美式/英式英语多发音人。

---

## 目录

- [开发思路](#开发思路)
- [项目结构](#项目结构)
- [前置要求](#前置要求)
- [编译与发布](#编译与发布)
- [配置说明](#配置说明)
- [使用说明](#使用说明)
  - [synthesize — 合成语音](#synthesize--合成语音)
  - [list-voices — 列出语音](#list-voices--列出语音)
  - [batch — 批量合成](#batch--批量合成)
- [SSML 支持](#ssml-支持)
- [文件名自动生成规则](#文件名自动生成规则)
- [内置语音列表](#内置语音列表)
- [错误处理](#错误处理)

---

## 开发思路

1. **分层清晰**：`Models`（数据结构）→ `Helpers`（纯函数工具）→ `Services`（业务逻辑）→ `Program.cs`（CLI 入口），各层单向依赖，不跨层引用。
2. **接口驱动**：核心逻辑通过 `ISpeechSynthesisService` 接口暴露，方便测试与未来替换底层 SDK。
3. **并发控制**：`SynthesizeManyAsync` 通过 `SemaphoreSlim` 限制最大并发数，避免 Azure API 限流。
4. **SSML 优先**：当 `--ssml` 选项指定 SSML 文件时，直接将文件内容提交给 Azure API，绕过文本包装逻辑；当需要调整语速/音量时，自动将纯文本包装为 SSML `<prosody>` 元素。
5. **格式选择**：WAV 使用 `Riff24Khz16BitMonoPcm`（无损、通用），MP3 使用 `Audio16Khz32KBitRateMonoMp3`（有损、小体积）。
6. **配置优先级**：`speechconfig.json` → 命令行 `--region`/`--key` 覆盖，支持 CI/CD 场景下使用环境变量包装器传入密钥。

---

## 项目结构

```
tools/msspeechcmd/
├── MsSpeechCmd.csproj          # 项目文件（.NET 10, 可单文件发布）
├── Program.cs                  # CLI 入口（System.CommandLine）
├── Models/
│   ├── SpeechConfig.cs         # speechconfig.json 映射模型
│   ├── SynthesisRequest.cs     # 单次合成请求参数
│   └── SynthesisResult.cs      # 合成结果
├── Services/
│   ├── ISpeechSynthesisService.cs  # 合成服务接口
│   └── SpeechSynthesisService.cs   # Azure SDK 实现
├── Helpers/
│   ├── FileNameHelper.cs       # 输出路径/文件名解析
│   └── VoiceHelper.cs          # 语音/区域映射
└── README.md                   # 本文档
```

---

## 前置要求

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- 有效的 [Azure Cognitive Services Speech 订阅](https://azure.microsoft.com/zh-cn/products/ai-services/text-to-speech)（免费层每月 50 万字符）

---

## 编译与发布

### 调试编译

```bash
dotnet build tools/msspeechcmd/MsSpeechCmd.csproj
```

### 发布为独立单文件可执行程序

```bash
# Windows x64
dotnet publish tools/msspeechcmd/MsSpeechCmd.csproj -r win-x64 -c Release

# Linux x64
dotnet publish tools/msspeechcmd/MsSpeechCmd.csproj -r linux-x64 -c Release

# macOS Apple Silicon
dotnet publish tools/msspeechcmd/MsSpeechCmd.csproj -r osx-arm64 -c Release
```

发布产物位于 `bin/Release/net10.0/<RID>/publish/msspeechcmd`（Linux/macOS）或 `msspeechcmd.exe`（Windows）。

---

## 配置说明

工具启动时按如下优先级加载 Azure Speech 配置：

1. 当前目录下的 `speechconfig.json`
2. `--config` 选项指定的文件
3. `--region` / `--key` 命令行参数（最高优先级，覆盖文件中的值）

### speechconfig.json 格式

```json
{
  "region": "eastus",
  "key": "your-azure-speech-subscription-key"
}
```

> ⚠️ **安全提示**：请勿将包含真实密钥的 `speechconfig.json` 提交到版本控制系统。

---

## 使用说明

### synthesize — 合成语音

将文本合成为音频文件：

```bash
msspeechcmd synthesize "Hello, world!"
```

**完整参数说明：**

| 参数 | 简写 | 说明 | 默认值 |
|------|------|------|--------|
| `<text>` | — | 要合成的文本（与 `--ssml` 互斥） | — |
| `--ssml <file>` | — | SSML 文件路径（与文本参数互斥） | — |
| `--locale` | `-l` | 发音区域：`us`（美式）或 `uk`（英式） | `us` |
| `--voice` | `-v` | 发音人（简短名或完整名，见下方列表） | 区域默认值 |
| `--output` | `-o` | 输出文件名（`.wav` 或 `.mp3`） | 自动生成 |
| `--output-path` | `-p` | 输出目录 | 当前目录 |
| `--rate` | — | 语速 `0.5 ~ 2.0` | `1.0` |
| `--volume` | — | 音量 `0 ~ 100` | `100` |
| `--region` | — | Azure 区域（覆盖配置文件） | — |
| `--key` | — | Azure 订阅密钥（覆盖配置文件） | — |
| `--config` | — | speechconfig.json 路径 | 当前目录 |

**示例：**

```bash
# 美式英语，默认语音，自动生成文件名
msspeechcmd synthesize "The quick brown fox"

# 英式英语，Ryan 发音人，输出 MP3
msspeechcmd synthesize "Good morning" -l uk -v Ryan -o morning.mp3

# 指定输出目录和慢速朗读
msspeechcmd synthesize "Hello there" -p /tmp/audio --rate 0.75

# 使用命令行密钥（不依赖配置文件）
msspeechcmd --region eastus --key YOUR_KEY synthesize "Hello"

# 输出 MP3，指定 Monica 语音
msspeechcmd synthesize "Welcome to PhonoArk" -v Monica -o welcome.mp3
```

---

### list-voices — 列出语音

列出内置已知语音：

```bash
msspeechcmd list-voices
```

过滤区域：

```bash
msspeechcmd list-voices -l uk
```

从 Azure API 实时获取最新完整列表（需要有效配置）：

```bash
msspeechcmd list-voices --online
```

---

### batch — 批量合成

从 JSON 文件批量合成多条请求，支持并发：

```bash
msspeechcmd batch requests.json --concurrency 4
```

**requests.json 格式（SynthesisRequest 数组）：**

```json
[
  {
    "text": "Hello, world!",
    "locale": "us",
    "voice": "Jenny",
    "outputFile": "hello.wav"
  },
  {
    "text": "Good morning",
    "locale": "uk",
    "voice": "Sonia",
    "outputFile": "morning.mp3",
    "outputPath": "/tmp/audio"
  },
  {
    "text": "The quick brown fox jumps over the lazy dog",
    "locale": "us",
    "rate": 0.8,
    "volume": 90
  }
]
```

---

## SSML 支持

通过 `--ssml` 选项传入 SSML 文件，可实现精细的发音控制：

```bash
msspeechcmd synthesize --ssml my_speech.ssml -o output.wav
```

**SSML 文件示例（my_speech.ssml）：**

```xml
<speak version="1.0" xmlns="http://www.w3.org/2001/10/synthesis" xml:lang="en-US">
  <voice name="en-US-JennyNeural">
    <prosody rate="0.9" pitch="+2st">
      Welcome to <emphasis level="strong">PhonoArk</emphasis>.
    </prosody>
    <break time="500ms"/>
    <phoneme alphabet="ipa" ph="həˈloʊ">Hello</phoneme>
  </voice>
</speak>
```

> 当使用纯文本且指定了 `--rate`/`--volume` 非默认值时，工具会自动将文本包装为带 `<prosody>` 的 SSML。

---

## 文件名自动生成规则

若不指定 `--output` 选项，工具将自动根据文本生成文件名：

1. 按空格拆分文本，取前 **最多 3 个单词**
2. 每个单词去除标点符号（保留字母、数字、撇号）
3. 单词间保留空格
4. 附加扩展名（`.wav` 或根据需要指定 `.mp3`）

**示例：**

| 输入文本 | 生成文件名 |
|---------|-----------|
| `Hello, world!` | `Hello world.wav` |
| `The quick brown fox jumps` | `The quick brown.wav` |
| `Good morning` | `Good morning.wav` |
| `It's a beautiful day today` | `It's a beautiful.wav` |

---

## 内置语音列表

### 美式英语 (en-US)

| 语音名称 | 显示名称 | 性别 |
|---------|---------|------|
| en-US-JennyNeural | Jenny | Female（**默认**） |
| en-US-AriaNeural | Aria | Female |
| en-US-AnaNeural | Ana | Female |
| en-US-MichelleNeural | Michelle | Female |
| en-US-MonicaNeural | Monica | Female |
| en-US-GuyNeural | Guy | Male |
| en-US-DavisNeural | Davis | Male |
| en-US-TonyNeural | Tony | Male |
| en-US-JasonNeural | Jason | Male |
| en-US-BrandonNeural | Brandon | Male |

### 英式英语 (en-GB)

| 语音名称 | 显示名称 | 性别 |
|---------|---------|------|
| en-GB-SoniaNeural | Sonia | Female（**默认**） |
| en-GB-LibbyNeural | Libby | Female |
| en-GB-MaisieNeural | Maisie | Female |
| en-GB-RyanNeural | Ryan | Male |
| en-GB-ThomasNeural | Thomas | Male |
| en-GB-OliverNeural | Oliver | Male |
| en-GB-NoahNeural | Noah | Male |

> 使用 `msspeechcmd list-voices --online` 可获取 Azure 上所有可用的英语神经网络语音（包含新发布的语音）。

---

## 错误处理

| 情况 | 退出码 | 说明 |
|------|--------|------|
| 成功 | `0` | 所有请求均已成功合成 |
| 部分失败 | `1` | 批量模式下存在失败请求 |
| 配置缺失 | `1` | 未找到有效的 region/key |
| 文件不存在 | `1` | 指定的 SSML 文件或批量请求文件不存在 |
| Azure API 错误 | `1` | 鉴权失败、配额超限、网络错误等 |

详细错误信息输出到 `stderr`。
