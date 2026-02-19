// tests/phonemes.test.js
// 音律方舟 - 音标数据完整性回归测试

const {
  vowels,
  diphthongs,
  consonants,
  getAllPhonemes,
  getPhonemesByType,
  getPhonemeBySymbol,
  getPhonemeStats
} = require('../miniprogram/data/phonemes');

describe('音标数据完整性', () => {

  test('应包含 12 个元音', () => {
    expect(vowels).toHaveLength(12);
  });

  test('应包含 8 个双元音', () => {
    expect(diphthongs).toHaveLength(8);
  });

  test('应包含 24 个辅音', () => {
    expect(consonants).toHaveLength(24);
  });

  test('总计应有 44 个音标', () => {
    const all = getAllPhonemes();
    expect(all).toHaveLength(44);
  });

  test('getPhonemeStats 应返回正确的统计数据', () => {
    const stats = getPhonemeStats();
    expect(stats.vowelCount).toBe(12);
    expect(stats.diphthongCount).toBe(8);
    expect(stats.consonantCount).toBe(24);
    expect(stats.totalCount).toBe(44);
  });
});

describe('音标数据格式', () => {

  const allPhonemes = getAllPhonemes();

  test.each(allPhonemes)('音标 $symbol 应有必填字段', (phoneme) => {
    expect(phoneme.symbol).toBeTruthy();
    expect(typeof phoneme.symbol).toBe('string');
    expect(phoneme.type).toBeTruthy();
    expect(['vowel', 'diphthong', 'consonant']).toContain(phoneme.type);
    expect(phoneme.name).toBeTruthy();
    expect(phoneme.description).toBeTruthy();
  });

  test.each(allPhonemes)('音标 $symbol 应至少包含 5 个示例单词', (phoneme) => {
    expect(phoneme.examples).toBeDefined();
    expect(Array.isArray(phoneme.examples)).toBe(true);
    expect(phoneme.examples.length).toBeGreaterThanOrEqual(5);
  });

  test.each(allPhonemes)('音标 $symbol 的每个示例单词应有 word 和 ipa 字段', (phoneme) => {
    phoneme.examples.forEach((example) => {
      expect(example.word).toBeTruthy();
      expect(typeof example.word).toBe('string');
      expect(example.ipa).toBeTruthy();
      expect(typeof example.ipa).toBe('string');
      // IPA 标注应以 / 开头和结尾
      expect(example.ipa.startsWith('/')).toBe(true);
      expect(example.ipa.endsWith('/')).toBe(true);
    });
  });
});

describe('音标符号唯一性', () => {

  test('所有音标符号应唯一', () => {
    const all = getAllPhonemes();
    const symbols = all.map(p => p.symbol);
    const uniqueSymbols = new Set(symbols);
    expect(uniqueSymbols.size).toBe(symbols.length);
  });
});

describe('音标分类查询', () => {

  test('getPhonemesByType("vowel") 应返回所有元音', () => {
    const result = getPhonemesByType('vowel');
    expect(result).toHaveLength(12);
    result.forEach(p => expect(p.type).toBe('vowel'));
  });

  test('getPhonemesByType("diphthong") 应返回所有双元音', () => {
    const result = getPhonemesByType('diphthong');
    expect(result).toHaveLength(8);
    result.forEach(p => expect(p.type).toBe('diphthong'));
  });

  test('getPhonemesByType("consonant") 应返回所有辅音', () => {
    const result = getPhonemesByType('consonant');
    expect(result).toHaveLength(24);
    result.forEach(p => expect(p.type).toBe('consonant'));
  });

  test('getPhonemesByType 传入无效类型应返回空数组', () => {
    const result = getPhonemesByType('invalid');
    expect(result).toHaveLength(0);
  });
});

describe('音标符号查找', () => {

  test('getPhonemeBySymbol 能找到存在的音标', () => {
    const result = getPhonemeBySymbol('iː');
    expect(result).toBeDefined();
    expect(result.symbol).toBe('iː');
    expect(result.type).toBe('vowel');
  });

  test('getPhonemeBySymbol 找不到不存在的音标应返回 undefined', () => {
    const result = getPhonemeBySymbol('xyz');
    expect(result).toBeUndefined();
  });

  test('能通过符号找到所有 44 个音标', () => {
    const all = getAllPhonemes();
    all.forEach(p => {
      const found = getPhonemeBySymbol(p.symbol);
      expect(found).toBeDefined();
      expect(found.symbol).toBe(p.symbol);
    });
  });
});

describe('元音列表覆盖', () => {
  const expectedVowels = ['iː', 'ɪ', 'e', 'æ', 'ɑː', 'ɒ', 'ɔː', 'ʊ', 'uː', 'ʌ', 'ɜː', 'ə'];

  test.each(expectedVowels)('应包含元音 %s', (symbol) => {
    const found = vowels.find(v => v.symbol === symbol);
    expect(found).toBeDefined();
  });
});

describe('双元音列表覆盖', () => {
  const expectedDiphthongs = ['eɪ', 'aɪ', 'ɔɪ', 'aʊ', 'oʊ', 'ɪə', 'eə', 'ʊə'];

  test.each(expectedDiphthongs)('应包含双元音 %s', (symbol) => {
    const found = diphthongs.find(d => d.symbol === symbol);
    expect(found).toBeDefined();
  });
});

describe('辅音列表覆盖', () => {
  const expectedConsonants = [
    'p', 'b', 't', 'd', 'k', 'ɡ', 'f', 'v', 'θ', 'ð', 's', 'z',
    'ʃ', 'ʒ', 'h', 'm', 'n', 'ŋ', 'l', 'r', 'j', 'w', 'tʃ', 'dʒ'
  ];

  test.each(expectedConsonants)('应包含辅音 %s', (symbol) => {
    const found = consonants.find(c => c.symbol === symbol);
    expect(found).toBeDefined();
  });
});
