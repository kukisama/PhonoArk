using MsSpeechCmd.Models;

namespace MsSpeechCmd.Helpers;

/// <summary>输出文件名生成帮助类。</summary>
public static class FileNameHelper
{
    /// <summary>
    /// 根据文本自动生成文件名（不含目录）。
    /// 取文本前最多 3 个单词，保留单词间空格，去除标点符号，附加指定扩展名。
    /// </summary>
    /// <param name="text">输入文本。</param>
    /// <param name="extension">文件扩展名，例如 ".wav" 或 ".mp3"。</param>
    /// <returns>生成的文件名，例如 "Hello World Today.wav"。</returns>
    public static string GenerateFromText(string text, string extension)
    {
        if (string.IsNullOrWhiteSpace(text))
            return $"output{extension}";

        // 按空白拆分，过滤空项，取前 3 个
        var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                        .Take(3)
                        .Select(StripPunctuation)
                        .Where(w => w.Length > 0)
                        .ToArray();

        string name = words.Length > 0 ? string.Join(" ", words) : "output";

        // 替换文件系统非法字符
        foreach (char c in Path.GetInvalidFileNameChars())
            name = name.Replace(c, '_');

        return name + extension;
    }

    /// <summary>
    /// 根据请求参数解析最终的完整输出路径。
    /// 如果 OutputFile 已指定则直接使用，否则从文本自动生成文件名。
    /// </summary>
    public static string ResolveOutputPath(SynthesisRequest request)
    {
        string extension = ResolveExtension(request.OutputFile);

        string fileName = !string.IsNullOrWhiteSpace(request.OutputFile)
            ? Path.GetFileName(request.OutputFile)
            : GenerateFromText(request.Text ?? "output", extension);

        string directory = !string.IsNullOrWhiteSpace(request.OutputPath)
            ? request.OutputPath
            : Directory.GetCurrentDirectory();

        return Path.Combine(directory, fileName);
    }

    /// <summary>从文件名中提取扩展名，默认为 .wav。</summary>
    public static string ResolveExtension(string? fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return ".wav";

        string ext = Path.GetExtension(fileName).ToLowerInvariant();
        return ext is ".wav" or ".mp3" ? ext : ".wav";
    }

    private static string StripPunctuation(string word)
    {
        // 仅保留字母、数字和撇号（apostrophe）
        var chars = word.Where(c => char.IsLetterOrDigit(c) || c == '\'').ToArray();
        return new string(chars);
    }
}
