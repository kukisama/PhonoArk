namespace MsSpeechCmd.Models;

/// <summary>
/// Azure Speech Service 连接配置，对应 speechconfig.json 文件结构。
/// </summary>
public sealed class SpeechServiceConfig
{
    /// <summary>Azure 区域，例如 eastus、eastasia。</summary>
    public string Region { get; set; } = string.Empty;

    /// <summary>Azure Speech 订阅密钥。</summary>
    public string Key { get; set; } = string.Empty;
}
