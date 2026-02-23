namespace MsSpeechCmd.Models;

/// <summary>语音合成请求参数。</summary>
public sealed class SynthesisRequest
{
    /// <summary>要合成的文本内容（与 SsmlFile 二选一）。</summary>
    public string? Text { get; set; }

    /// <summary>SSML 文件路径（与 Text 二选一）。若指定则直接使用 SSML 合成。</summary>
    public string? SsmlFile { get; set; }

    /// <summary>发音区域：us（美式，默认）或 uk（英式）。</summary>
    public string Locale { get; set; } = "us";

    /// <summary>
    /// 发音人名称。若为空则使用区域默认语音。
    /// 美式默认：en-US-JennyNeural；英式默认：en-GB-SoniaNeural。
    /// </summary>
    public string? Voice { get; set; }

    /// <summary>
    /// 输出文件名（含扩展名 .wav 或 .mp3）。
    /// 若为空则自动取文本前 3 个单词生成文件名。
    /// </summary>
    public string? OutputFile { get; set; }

    /// <summary>
    /// 输出目录路径。若为空则输出到当前工作目录。
    /// </summary>
    public string? OutputPath { get; set; }

    /// <summary>语速调节，范围 0.5 ~ 2.0，默认 1.0。</summary>
    public double Rate { get; set; } = 1.0;

    /// <summary>音量调节，范围 0 ~ 100，默认 100。</summary>
    public int Volume { get; set; } = 100;
}
