using System.Collections.Generic;
using PhonoArk.Mobile.Core.Models;

namespace PhonoArk.Mobile.Core.Services;

/// <summary>
/// 音标数据服务接口
/// </summary>
public interface IPhonemeService
{
    /// <summary>获取所有音标</summary>
    IReadOnlyList<Phoneme> GetAllPhonemes();

    /// <summary>按类别获取音标</summary>
    IReadOnlyList<Phoneme> GetPhonemesByCategory(PhonemeCategory category);

    /// <summary>根据符号获取音标详情</summary>
    Phoneme? GetPhonemeBySymbol(string symbol);

    /// <summary>获取所有类别</summary>
    IReadOnlyList<PhonemeCategory> GetCategories();
}
