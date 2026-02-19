namespace PhonoArk.Mobile.Core.Models;

/// <summary>
/// 示例单词
/// </summary>
public class ExampleWord
{
    /// <summary>单词文本</summary>
    public string Word { get; set; } = string.Empty;

    /// <summary>音标标注（含斜杠）</summary>
    public string IpaTranscription { get; set; } = string.Empty;
}
