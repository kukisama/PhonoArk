namespace MsSpeechCmd.Helpers;

/// <summary>语音与区域映射帮助类，提供已知的 Azure Neural TTS 语音列表。</summary>
public static class VoiceHelper
{
    // -----------------------------------------------------------------------
    // 美式英语 (en-US) Neural 语音
    // -----------------------------------------------------------------------
    private static readonly IReadOnlyList<VoiceInfo> UsVoices = new[]
    {
        new VoiceInfo("en-US-JennyNeural",      "Jenny",      "Female", "us"),
        new VoiceInfo("en-US-AriaNeural",        "Aria",       "Female", "us"),
        new VoiceInfo("en-US-AnaNeural",         "Ana",        "Female", "us"),
        new VoiceInfo("en-US-MichelleNeural",    "Michelle",   "Female", "us"),
        new VoiceInfo("en-US-MonicaNeural",      "Monica",     "Female", "us"),
        new VoiceInfo("en-US-GuyNeural",         "Guy",        "Male",   "us"),
        new VoiceInfo("en-US-DavisNeural",       "Davis",      "Male",   "us"),
        new VoiceInfo("en-US-TonyNeural",        "Tony",       "Male",   "us"),
        new VoiceInfo("en-US-JasonNeural",       "Jason",      "Male",   "us"),
        new VoiceInfo("en-US-BrandonNeural",     "Brandon",    "Male",   "us"),
    };

    // -----------------------------------------------------------------------
    // 英式英语 (en-GB) Neural 语音
    // -----------------------------------------------------------------------
    private static readonly IReadOnlyList<VoiceInfo> UkVoices = new[]
    {
        new VoiceInfo("en-GB-SoniaNeural",  "Sonia",  "Female", "uk"),
        new VoiceInfo("en-GB-LibbyNeural",  "Libby",  "Female", "uk"),
        new VoiceInfo("en-GB-MaisieNeural", "Maisie", "Female", "uk"),
        new VoiceInfo("en-GB-RyanNeural",   "Ryan",   "Male",   "uk"),
        new VoiceInfo("en-GB-ThomasNeural", "Thomas", "Male",   "uk"),
        new VoiceInfo("en-GB-OliverNeural", "Oliver", "Male",   "uk"),
        new VoiceInfo("en-GB-NoahNeural",   "Noah",   "Male",   "uk"),
    };

    /// <summary>返回所有内置语音列表。</summary>
    public static IEnumerable<VoiceInfo> AllVoices =>
        UsVoices.Concat(UkVoices);

    /// <summary>
    /// 根据区域标识（us/en-us 或 uk/en-gb）返回默认语音名称。
    /// </summary>
    public static string GetDefaultVoice(string locale) =>
        NormalizeLocale(locale) switch
        {
            "en-GB" => "en-GB-SoniaNeural",
            _       => "en-US-JennyNeural",
        };

    /// <summary>
    /// 将用户输入的区域标识（us/uk/en-US/en-GB 等）统一规范化为 BCP-47 标签。
    /// </summary>
    public static string NormalizeLocale(string locale) =>
        locale.ToLowerInvariant() switch
        {
            "uk" or "en-gb" or "en_gb" or "gb" => "en-GB",
            _ => "en-US",
        };

    /// <summary>
    /// 验证给定的语音名称是否在已知列表中。
    /// 若不在列表中不会报错，允许用户传入新发布的语音。
    /// </summary>
    public static bool IsKnownVoice(string voiceName) =>
        AllVoices.Any(v => string.Equals(v.ShortName, voiceName, StringComparison.OrdinalIgnoreCase));

    /// <summary>查找与简短名称（如 "Jenny"）匹配的语音。</summary>
    public static VoiceInfo? FindByShortName(string shortName) =>
        AllVoices.FirstOrDefault(v =>
            string.Equals(v.DisplayName, shortName, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(v.ShortName,   shortName, StringComparison.OrdinalIgnoreCase));
}

/// <summary>语音元数据。</summary>
public sealed record VoiceInfo(
    string ShortName,
    string DisplayName,
    string Gender,
    string Locale);
