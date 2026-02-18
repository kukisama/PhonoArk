using System;

namespace PhonoArk.Models;

public class FavoritePhoneme
{
    public int Id { get; set; }
    public string PhonemeSymbol { get; set; } = string.Empty;
    public string GroupName { get; set; } = "Default";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

