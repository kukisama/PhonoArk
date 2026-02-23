using System.CommandLine;
using System.Text.Json;
using MsSpeechCmd.Helpers;
using MsSpeechCmd.Models;
using MsSpeechCmd.Services;

// ═══════════════════════════════════════════════════════════════════════════
//  msspeechcmd — Microsoft Azure Speech TTS 命令行工具
// ═══════════════════════════════════════════════════════════════════════════

var rootCommand = new RootCommand("Microsoft Azure Speech TTS 命令行工具 (msspeechcmd)");

// ── 全局选项：连接配置 ──────────────────────────────────────────────────────
var configFileOption = new Option<FileInfo?>(
    name: "--config",
    description: "speechconfig.json 文件路径（默认读取当前目录下的 speechconfig.json）");

var regionOption = new Option<string?>(
    name: "--region",
    description: "Azure 区域，例如 eastus、eastasia（优先级高于配置文件）");

var keyOption = new Option<string?>(
    name: "--key",
    description: "Azure Speech 订阅密钥（优先级高于配置文件）");

rootCommand.AddGlobalOption(configFileOption);
rootCommand.AddGlobalOption(regionOption);
rootCommand.AddGlobalOption(keyOption);

// ═══════════════════════════════════════════════════════════════════════════
//  子命令：synthesize（合成语音）
// ═══════════════════════════════════════════════════════════════════════════
var synthesizeCommand = new Command("synthesize", "将文本或 SSML 合成为语音文件");

var textArg = new Argument<string?>(
    name: "text",
    description: "要合成的文本内容（与 --ssml 互斥）",
    getDefaultValue: () => null);
synthesizeCommand.AddArgument(textArg);

var ssmlOption = new Option<FileInfo?>(
    name: "--ssml",
    description: "SSML 文件路径（与 text 参数互斥）");

var localeOption = new Option<string>(
    name: "--locale",
    description: "发音区域：us（美式英语，默认）或 uk（英式英语）",
    getDefaultValue: () => "us");
localeOption.AddAlias("-l");

var voiceOption = new Option<string?>(
    name: "--voice",
    description: "发音人名称，例如 Jenny、Aria、Sonia（可用 list-voices 查看完整列表）");
voiceOption.AddAlias("-v");

var outputFileOption = new Option<string?>(
    name: "--output",
    description: "输出文件名（含扩展名 .wav 或 .mp3），不指定则自动从文本前 3 个单词生成");
outputFileOption.AddAlias("-o");

var outputPathOption = new Option<string?>(
    name: "--output-path",
    description: "输出目录（不指定则输出到当前目录）");
outputPathOption.AddAlias("-p");

var rateOption = new Option<double>(
    name: "--rate",
    description: "语速，范围 0.5 ~ 2.0（默认 1.0）",
    getDefaultValue: () => 1.0);

var volumeOption = new Option<int>(
    name: "--volume",
    description: "音量，范围 0 ~ 100（默认 100）",
    getDefaultValue: () => 100);

var concurrencyOption = new Option<int>(
    name: "--concurrency",
    description: "最大并发合成数（默认 1，批量模式有效）",
    getDefaultValue: () => 1);

synthesizeCommand.AddOption(ssmlOption);
synthesizeCommand.AddOption(localeOption);
synthesizeCommand.AddOption(voiceOption);
synthesizeCommand.AddOption(outputFileOption);
synthesizeCommand.AddOption(outputPathOption);
synthesizeCommand.AddOption(rateOption);
synthesizeCommand.AddOption(volumeOption);
synthesizeCommand.AddOption(concurrencyOption);

synthesizeCommand.SetHandler(async (context) =>
{
    var text        = context.ParseResult.GetValueForArgument(textArg);
    var ssmlFile    = context.ParseResult.GetValueForOption(ssmlOption);
    var locale      = context.ParseResult.GetValueForOption(localeOption) ?? "us";
    var voice       = context.ParseResult.GetValueForOption(voiceOption);
    var outputFile  = context.ParseResult.GetValueForOption(outputFileOption);
    var outputPath  = context.ParseResult.GetValueForOption(outputPathOption);
    var rate        = context.ParseResult.GetValueForOption(rateOption);
    var volume      = context.ParseResult.GetValueForOption(volumeOption);
    var configFile  = context.ParseResult.GetValueForOption(configFileOption);
    var region      = context.ParseResult.GetValueForOption(regionOption);
    var key         = context.ParseResult.GetValueForOption(keyOption);

    if (string.IsNullOrWhiteSpace(text) && ssmlFile == null)
    {
        Console.Error.WriteLine("错误：必须提供 text 参数或 --ssml 选项。");
        context.ExitCode = 1;
        return;
    }

    var speechConfig = LoadSpeechConfig(configFile, region, key);
    if (speechConfig == null)
    {
        context.ExitCode = 1;
        return;
    }

    var request = new SynthesisRequest
    {
        Text       = text,
        SsmlFile   = ssmlFile?.FullName,
        Locale     = locale,
        Voice      = voice,
        OutputFile = outputFile,
        OutputPath = outputPath,
        Rate       = rate,
        Volume     = volume,
    };

    await using var service = new SpeechSynthesisService(speechConfig);

    Console.WriteLine($"正在合成：{(string.IsNullOrWhiteSpace(text) ? ssmlFile!.Name : text)}");
    var result = await service.SynthesizeAsync(request, context.GetCancellationToken());

    if (result.Success)
    {
        Console.WriteLine($"✔ 合成成功");
        Console.WriteLine($"  输出文件：{result.OutputFilePath}");
        Console.WriteLine($"  音频时长：{result.Duration.TotalSeconds:F2}s");
    }
    else
    {
        Console.Error.WriteLine($"✘ 合成失败：{result.ErrorMessage}");
        context.ExitCode = 1;
    }
});

rootCommand.AddCommand(synthesizeCommand);

// ═══════════════════════════════════════════════════════════════════════════
//  子命令：list-voices（列出可用语音）
// ═══════════════════════════════════════════════════════════════════════════
var listVoicesCommand = new Command("list-voices", "列出可用的 Azure Neural 语音");

var listLocaleOption = new Option<string?>(
    name: "--locale",
    description: "过滤区域：us 或 uk（不指定则列出全部英语语音）");
listLocaleOption.AddAlias("-l");

var listOnlineOption = new Option<bool>(
    name: "--online",
    description: "从 Azure API 实时获取最新语音列表（需要有效的配置文件或 --region/--key）",
    getDefaultValue: () => false);

listVoicesCommand.AddOption(listLocaleOption);
listVoicesCommand.AddOption(listOnlineOption);

listVoicesCommand.SetHandler(async (context) =>
{
    var listLocale  = context.ParseResult.GetValueForOption(listLocaleOption);
    var online      = context.ParseResult.GetValueForOption(listOnlineOption);
    var configFile  = context.ParseResult.GetValueForOption(configFileOption);
    var region      = context.ParseResult.GetValueForOption(regionOption);
    var key         = context.ParseResult.GetValueForOption(keyOption);

    if (online)
    {
        var speechConfig = LoadSpeechConfig(configFile, region, key);
        if (speechConfig == null)
        {
            context.ExitCode = 1;
            return;
        }

        await using var service = new SpeechSynthesisService(speechConfig);
        Console.WriteLine("正在从 Azure API 获取语音列表...");
        var voices = await service.GetAvailableVoicesAsync(context.GetCancellationToken());

        Console.WriteLine($"\n{"语音名称",-42} {"性别",-8} {"区域",-10}");
        Console.WriteLine(new string('-', 65));
        foreach (var v in voices)
            Console.WriteLine(v);

        Console.WriteLine($"\n共 {voices.Count} 个英语语音。");
    }
    else
    {
        // 显示内置已知语音列表
        var voices = VoiceHelper.AllVoices;
        if (!string.IsNullOrWhiteSpace(listLocale))
        {
            string normalizedLocale = VoiceHelper.NormalizeLocale(listLocale).ToLowerInvariant();
            voices = voices.Where(v =>
                VoiceHelper.NormalizeLocale(v.Locale).ToLowerInvariant() == normalizedLocale);
        }

        Console.WriteLine($"\n{"语音名称",-42} {"显示名称",-12} {"性别",-8} {"区域"}");
        Console.WriteLine(new string('-', 75));
        foreach (var v in voices)
            Console.WriteLine($"{v.ShortName,-42} {v.DisplayName,-12} {v.Gender,-8} {VoiceHelper.NormalizeLocale(v.Locale)}");

        Console.WriteLine("\n提示：使用 --online 选项可从 Azure API 获取最新完整语音列表。");
    }
});

rootCommand.AddCommand(listVoicesCommand);

// ═══════════════════════════════════════════════════════════════════════════
//  子命令：batch（批量合成，从 JSON 文件读取请求列表）
// ═══════════════════════════════════════════════════════════════════════════
var batchCommand = new Command("batch", "从 JSON 请求文件批量合成语音（支持并发）");

var batchFileArg = new Argument<FileInfo>(
    name: "requests-file",
    description: "包含 SynthesisRequest 数组的 JSON 文件路径");
batchCommand.AddArgument(batchFileArg);

var batchConcurrencyOption = new Option<int>(
    name: "--concurrency",
    description: "最大并发合成数（默认 4）",
    getDefaultValue: () => 4);
batchCommand.AddOption(batchConcurrencyOption);

batchCommand.SetHandler(async (context) =>
{
    var requestsFile = context.ParseResult.GetValueForArgument(batchFileArg);
    var concurrency  = context.ParseResult.GetValueForOption(batchConcurrencyOption);
    var configFile   = context.ParseResult.GetValueForOption(configFileOption);
    var region       = context.ParseResult.GetValueForOption(regionOption);
    var key          = context.ParseResult.GetValueForOption(keyOption);

    if (!requestsFile.Exists)
    {
        Console.Error.WriteLine($"错误：文件不存在：{requestsFile.FullName}");
        context.ExitCode = 1;
        return;
    }

    var speechConfig = LoadSpeechConfig(configFile, region, key);
    if (speechConfig == null)
    {
        context.ExitCode = 1;
        return;
    }

    List<SynthesisRequest>? requests;
    try
    {
        string json = await File.ReadAllTextAsync(requestsFile.FullName, context.GetCancellationToken());
        requests = JsonSerializer.Deserialize<List<SynthesisRequest>>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"错误：无法解析请求文件：{ex.Message}");
        context.ExitCode = 1;
        return;
    }

    if (requests == null || requests.Count == 0)
    {
        Console.Error.WriteLine("错误：请求文件为空。");
        context.ExitCode = 1;
        return;
    }

    Console.WriteLine($"批量合成 {requests.Count} 条请求（并发数：{concurrency}）...");
    await using var service = new SpeechSynthesisService(speechConfig);
    var results = await service.SynthesizeManyAsync(requests, concurrency, context.GetCancellationToken());

    int success = 0, failure = 0;
    for (int i = 0; i < results.Count; i++)
    {
        var r = results[i];
        if (r.Success)
        {
            Console.WriteLine($"  [{i + 1}] ✔ {r.OutputFilePath} ({r.Duration.TotalSeconds:F2}s)");
            success++;
        }
        else
        {
            Console.Error.WriteLine($"  [{i + 1}] ✘ {r.ErrorMessage}");
            failure++;
        }
    }

    Console.WriteLine($"\n完成：成功 {success}，失败 {failure}。");
    if (failure > 0)
        context.ExitCode = 1;
});

rootCommand.AddCommand(batchCommand);

// ═══════════════════════════════════════════════════════════════════════════
//  入口
// ═══════════════════════════════════════════════════════════════════════════
return await rootCommand.InvokeAsync(args);

// ── 加载语音配置（文件 → 参数覆盖）─────────────────────────────────────────
static SpeechServiceConfig? LoadSpeechConfig(FileInfo? configFile, string? region, string? key)
{
    var config = new SpeechServiceConfig();

    // 1. 尝试读取 speechconfig.json
    FileInfo target = configFile ?? new FileInfo(Path.Combine(Directory.GetCurrentDirectory(), "speechconfig.json"));
    if (target.Exists)
    {
        try
        {
            string json = File.ReadAllText(target.FullName);
            var loaded = JsonSerializer.Deserialize<SpeechServiceConfig>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (loaded != null)
            {
                config.Region = loaded.Region;
                config.Key    = loaded.Key;
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"警告：无法解析配置文件 {target.FullName}：{ex.Message}");
        }
    }

    // 2. 命令行参数覆盖
    if (!string.IsNullOrWhiteSpace(region))
        config.Region = region;
    if (!string.IsNullOrWhiteSpace(key))
        config.Key = key;

    // 3. 验证
    if (string.IsNullOrWhiteSpace(config.Region) || string.IsNullOrWhiteSpace(config.Key))
    {
        Console.Error.WriteLine(
            "错误：未找到有效的 Azure Speech 配置。\n" +
            "  请在当前目录创建 speechconfig.json，或使用 --region 和 --key 参数指定。\n" +
            "  speechconfig.json 格式：{\"region\":\"eastus\",\"key\":\"YOUR_KEY\"}");
        return null;
    }

    return config;
}
