using System.Collections.Generic;

namespace PhonoArk.Mobile.Core.Models;

/// <summary>
/// 音标
/// </summary>
public class Phoneme
{
    /// <summary>音标符号（如 iː, æ, θ）</summary>
    public string Symbol { get; set; } = string.Empty;

    /// <summary>音标类别</summary>
    public PhonemeCategory Category { get; set; }

    /// <summary>音标描述</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>示例单词列表（至少 5 个）</summary>
    public List<ExampleWord> ExampleWords { get; set; } = new();
}
