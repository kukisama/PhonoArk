using System;
using System.Collections.Generic;
using System.Linq;
using PhonoArk.Mobile.Core.Models;

namespace PhonoArk.Mobile.Core.Services;

/// <summary>
/// 音标数据服务
/// </summary>
public class PhonemeService : IPhonemeService
{
    private readonly List<Phoneme> _phonemes;

    public PhonemeService()
    {
        _phonemes = InitializePhonemes();
    }

    public IReadOnlyList<Phoneme> GetAllPhonemes() => _phonemes.AsReadOnly();

    public IReadOnlyList<Phoneme> GetPhonemesByCategory(PhonemeCategory category) =>
        _phonemes.Where(p => p.Category == category).ToList().AsReadOnly();

    public Phoneme? GetPhonemeBySymbol(string symbol) =>
        _phonemes.FirstOrDefault(p => p.Symbol == symbol);

    public IReadOnlyList<PhonemeCategory> GetCategories() =>
        Enum.GetValues<PhonemeCategory>().ToList().AsReadOnly();

    private static List<Phoneme> InitializePhonemes()
    {
        return new List<Phoneme>
        {
            // ===== 元音 (Vowels) =====
            new()
            {
                Symbol = "iː", Category = PhonemeCategory.Vowel,
                Description = "长元音，舌位高前，如 see",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "see", IpaTranscription = "/siː/" },
                    new() { Word = "meet", IpaTranscription = "/miːt/" },
                    new() { Word = "key", IpaTranscription = "/kiː/" },
                    new() { Word = "beach", IpaTranscription = "/biːtʃ/" },
                    new() { Word = "sheep", IpaTranscription = "/ʃiːp/" }
                }
            },
            new()
            {
                Symbol = "ɪ", Category = PhonemeCategory.Vowel,
                Description = "短元音，舌位高前偏央，如 sit",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "sit", IpaTranscription = "/sɪt/" },
                    new() { Word = "hit", IpaTranscription = "/hɪt/" },
                    new() { Word = "big", IpaTranscription = "/bɪɡ/" },
                    new() { Word = "ship", IpaTranscription = "/ʃɪp/" },
                    new() { Word = "fish", IpaTranscription = "/fɪʃ/" }
                }
            },
            new()
            {
                Symbol = "e", Category = PhonemeCategory.Vowel,
                Description = "短元音，舌位中前，如 bed",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "bed", IpaTranscription = "/bed/" },
                    new() { Word = "head", IpaTranscription = "/hed/" },
                    new() { Word = "red", IpaTranscription = "/red/" },
                    new() { Word = "said", IpaTranscription = "/sed/" },
                    new() { Word = "ten", IpaTranscription = "/ten/" }
                }
            },
            new()
            {
                Symbol = "æ", Category = PhonemeCategory.Vowel,
                Description = "短元音，舌位低前，如 cat",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "cat", IpaTranscription = "/kæt/" },
                    new() { Word = "hat", IpaTranscription = "/hæt/" },
                    new() { Word = "bad", IpaTranscription = "/bæd/" },
                    new() { Word = "man", IpaTranscription = "/mæn/" },
                    new() { Word = "apple", IpaTranscription = "/ˈæpəl/" }
                }
            },
            new()
            {
                Symbol = "ɑː", Category = PhonemeCategory.Vowel,
                Description = "长元音，舌位低后，如 car",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "car", IpaTranscription = "/kɑːr/" },
                    new() { Word = "far", IpaTranscription = "/fɑːr/" },
                    new() { Word = "palm", IpaTranscription = "/pɑːm/" },
                    new() { Word = "father", IpaTranscription = "/ˈfɑːðər/" },
                    new() { Word = "heart", IpaTranscription = "/hɑːrt/" }
                }
            },
            new()
            {
                Symbol = "ɒ", Category = PhonemeCategory.Vowel,
                Description = "短元音，舌位低后圆唇，如 hot",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "hot", IpaTranscription = "/hɒt/" },
                    new() { Word = "lot", IpaTranscription = "/lɒt/" },
                    new() { Word = "dog", IpaTranscription = "/dɒɡ/" },
                    new() { Word = "box", IpaTranscription = "/bɒks/" },
                    new() { Word = "stop", IpaTranscription = "/stɒp/" }
                }
            },
            new()
            {
                Symbol = "ɔː", Category = PhonemeCategory.Vowel,
                Description = "长元音，舌位中后圆唇，如 door",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "door", IpaTranscription = "/dɔːr/" },
                    new() { Word = "four", IpaTranscription = "/fɔːr/" },
                    new() { Word = "wall", IpaTranscription = "/wɔːl/" },
                    new() { Word = "ball", IpaTranscription = "/bɔːl/" },
                    new() { Word = "talk", IpaTranscription = "/tɔːk/" }
                }
            },
            new()
            {
                Symbol = "ʊ", Category = PhonemeCategory.Vowel,
                Description = "短元音，舌位高后偏央，如 book",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "book", IpaTranscription = "/bʊk/" },
                    new() { Word = "put", IpaTranscription = "/pʊt/" },
                    new() { Word = "good", IpaTranscription = "/ɡʊd/" },
                    new() { Word = "look", IpaTranscription = "/lʊk/" },
                    new() { Word = "foot", IpaTranscription = "/fʊt/" }
                }
            },
            new()
            {
                Symbol = "uː", Category = PhonemeCategory.Vowel,
                Description = "长元音，舌位高后圆唇，如 blue",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "blue", IpaTranscription = "/bluː/" },
                    new() { Word = "food", IpaTranscription = "/fuːd/" },
                    new() { Word = "moon", IpaTranscription = "/muːn/" },
                    new() { Word = "school", IpaTranscription = "/skuːl/" },
                    new() { Word = "true", IpaTranscription = "/truː/" }
                }
            },
            new()
            {
                Symbol = "ʌ", Category = PhonemeCategory.Vowel,
                Description = "短元音，舌位中央偏低，如 cup",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "cup", IpaTranscription = "/kʌp/" },
                    new() { Word = "bus", IpaTranscription = "/bʌs/" },
                    new() { Word = "love", IpaTranscription = "/lʌv/" },
                    new() { Word = "run", IpaTranscription = "/rʌn/" },
                    new() { Word = "sun", IpaTranscription = "/sʌn/" }
                }
            },
            new()
            {
                Symbol = "ɜː", Category = PhonemeCategory.Vowel,
                Description = "长元音，舌位中央，如 bird",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "bird", IpaTranscription = "/bɜːrd/" },
                    new() { Word = "word", IpaTranscription = "/wɜːrd/" },
                    new() { Word = "girl", IpaTranscription = "/ɡɜːrl/" },
                    new() { Word = "learn", IpaTranscription = "/lɜːrn/" },
                    new() { Word = "turn", IpaTranscription = "/tɜːrn/" }
                }
            },
            new()
            {
                Symbol = "ə", Category = PhonemeCategory.Vowel,
                Description = "中央元音（schwa），非重读，如 about",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "about", IpaTranscription = "/əˈbaʊt/" },
                    new() { Word = "banana", IpaTranscription = "/bəˈnænə/" },
                    new() { Word = "sofa", IpaTranscription = "/ˈsoʊfə/" },
                    new() { Word = "open", IpaTranscription = "/ˈoʊpən/" },
                    new() { Word = "common", IpaTranscription = "/ˈkɒmən/" }
                }
            },

            // ===== 双元音 (Diphthongs) =====
            new()
            {
                Symbol = "eɪ", Category = PhonemeCategory.Diphthong,
                Description = "双元音，从 e 滑向 ɪ，如 day",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "day", IpaTranscription = "/deɪ/" },
                    new() { Word = "make", IpaTranscription = "/meɪk/" },
                    new() { Word = "say", IpaTranscription = "/seɪ/" },
                    new() { Word = "late", IpaTranscription = "/leɪt/" },
                    new() { Word = "rain", IpaTranscription = "/reɪn/" }
                }
            },
            new()
            {
                Symbol = "aɪ", Category = PhonemeCategory.Diphthong,
                Description = "双元音，从 a 滑向 ɪ，如 my",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "my", IpaTranscription = "/maɪ/" },
                    new() { Word = "time", IpaTranscription = "/taɪm/" },
                    new() { Word = "buy", IpaTranscription = "/baɪ/" },
                    new() { Word = "fly", IpaTranscription = "/flaɪ/" },
                    new() { Word = "night", IpaTranscription = "/naɪt/" }
                }
            },
            new()
            {
                Symbol = "ɔɪ", Category = PhonemeCategory.Diphthong,
                Description = "双元音，从 ɔ 滑向 ɪ，如 boy",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "boy", IpaTranscription = "/bɔɪ/" },
                    new() { Word = "toy", IpaTranscription = "/tɔɪ/" },
                    new() { Word = "coin", IpaTranscription = "/kɔɪn/" },
                    new() { Word = "join", IpaTranscription = "/dʒɔɪn/" },
                    new() { Word = "oil", IpaTranscription = "/ɔɪl/" }
                }
            },
            new()
            {
                Symbol = "aʊ", Category = PhonemeCategory.Diphthong,
                Description = "双元音，从 a 滑向 ʊ，如 now",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "now", IpaTranscription = "/naʊ/" },
                    new() { Word = "how", IpaTranscription = "/haʊ/" },
                    new() { Word = "out", IpaTranscription = "/aʊt/" },
                    new() { Word = "house", IpaTranscription = "/haʊs/" },
                    new() { Word = "down", IpaTranscription = "/daʊn/" }
                }
            },
            new()
            {
                Symbol = "əʊ", Category = PhonemeCategory.Diphthong,
                Description = "双元音，从 ə 滑向 ʊ，如 go",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "go", IpaTranscription = "/ɡəʊ/" },
                    new() { Word = "home", IpaTranscription = "/həʊm/" },
                    new() { Word = "no", IpaTranscription = "/nəʊ/" },
                    new() { Word = "show", IpaTranscription = "/ʃəʊ/" },
                    new() { Word = "phone", IpaTranscription = "/fəʊn/" }
                }
            },
            new()
            {
                Symbol = "ɪə", Category = PhonemeCategory.Diphthong,
                Description = "双元音，从 ɪ 滑向 ə，如 near",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "near", IpaTranscription = "/nɪər/" },
                    new() { Word = "here", IpaTranscription = "/hɪər/" },
                    new() { Word = "dear", IpaTranscription = "/dɪər/" },
                    new() { Word = "beer", IpaTranscription = "/bɪər/" },
                    new() { Word = "fear", IpaTranscription = "/fɪər/" }
                }
            },
            new()
            {
                Symbol = "eə", Category = PhonemeCategory.Diphthong,
                Description = "双元音，从 e 滑向 ə，如 air",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "air", IpaTranscription = "/eər/" },
                    new() { Word = "care", IpaTranscription = "/keər/" },
                    new() { Word = "pair", IpaTranscription = "/peər/" },
                    new() { Word = "where", IpaTranscription = "/weər/" },
                    new() { Word = "hair", IpaTranscription = "/heər/" }
                }
            },
            new()
            {
                Symbol = "ʊə", Category = PhonemeCategory.Diphthong,
                Description = "双元音，从 ʊ 滑向 ə，如 tour",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "tour", IpaTranscription = "/tʊər/" },
                    new() { Word = "pure", IpaTranscription = "/pjʊər/" },
                    new() { Word = "cure", IpaTranscription = "/kjʊər/" },
                    new() { Word = "sure", IpaTranscription = "/ʃʊər/" },
                    new() { Word = "poor", IpaTranscription = "/pʊər/" }
                }
            },

            // ===== 辅音 (Consonants) =====
            new()
            {
                Symbol = "p", Category = PhonemeCategory.Consonant,
                Description = "清双唇爆破音，如 pen",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "pen", IpaTranscription = "/pen/" },
                    new() { Word = "pop", IpaTranscription = "/pɒp/" },
                    new() { Word = "pig", IpaTranscription = "/pɪɡ/" },
                    new() { Word = "play", IpaTranscription = "/pleɪ/" },
                    new() { Word = "cap", IpaTranscription = "/kæp/" }
                }
            },
            new()
            {
                Symbol = "b", Category = PhonemeCategory.Consonant,
                Description = "浊双唇爆破音，如 bag",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "bag", IpaTranscription = "/bæɡ/" },
                    new() { Word = "bed", IpaTranscription = "/bed/" },
                    new() { Word = "boy", IpaTranscription = "/bɔɪ/" },
                    new() { Word = "ball", IpaTranscription = "/bɔːl/" },
                    new() { Word = "job", IpaTranscription = "/dʒɒb/" }
                }
            },
            new()
            {
                Symbol = "t", Category = PhonemeCategory.Consonant,
                Description = "清齿龈爆破音，如 ten",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "ten", IpaTranscription = "/ten/" },
                    new() { Word = "top", IpaTranscription = "/tɒp/" },
                    new() { Word = "tree", IpaTranscription = "/triː/" },
                    new() { Word = "cat", IpaTranscription = "/kæt/" },
                    new() { Word = "time", IpaTranscription = "/taɪm/" }
                }
            },
            new()
            {
                Symbol = "d", Category = PhonemeCategory.Consonant,
                Description = "浊齿龈爆破音，如 dog",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "dog", IpaTranscription = "/dɒɡ/" },
                    new() { Word = "day", IpaTranscription = "/deɪ/" },
                    new() { Word = "did", IpaTranscription = "/dɪd/" },
                    new() { Word = "red", IpaTranscription = "/red/" },
                    new() { Word = "hand", IpaTranscription = "/hænd/" }
                }
            },
            new()
            {
                Symbol = "k", Category = PhonemeCategory.Consonant,
                Description = "清软腭爆破音，如 key",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "key", IpaTranscription = "/kiː/" },
                    new() { Word = "cat", IpaTranscription = "/kæt/" },
                    new() { Word = "cook", IpaTranscription = "/kʊk/" },
                    new() { Word = "back", IpaTranscription = "/bæk/" },
                    new() { Word = "kind", IpaTranscription = "/kaɪnd/" }
                }
            },
            new()
            {
                Symbol = "ɡ", Category = PhonemeCategory.Consonant,
                Description = "浊软腭爆破音，如 go",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "go", IpaTranscription = "/ɡəʊ/" },
                    new() { Word = "get", IpaTranscription = "/ɡet/" },
                    new() { Word = "give", IpaTranscription = "/ɡɪv/" },
                    new() { Word = "big", IpaTranscription = "/bɪɡ/" },
                    new() { Word = "game", IpaTranscription = "/ɡeɪm/" }
                }
            },
            new()
            {
                Symbol = "f", Category = PhonemeCategory.Consonant,
                Description = "清唇齿摩擦音，如 fish",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "fish", IpaTranscription = "/fɪʃ/" },
                    new() { Word = "five", IpaTranscription = "/faɪv/" },
                    new() { Word = "food", IpaTranscription = "/fuːd/" },
                    new() { Word = "fun", IpaTranscription = "/fʌn/" },
                    new() { Word = "leaf", IpaTranscription = "/liːf/" }
                }
            },
            new()
            {
                Symbol = "v", Category = PhonemeCategory.Consonant,
                Description = "浊唇齿摩擦音，如 van",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "van", IpaTranscription = "/væn/" },
                    new() { Word = "very", IpaTranscription = "/ˈveri/" },
                    new() { Word = "love", IpaTranscription = "/lʌv/" },
                    new() { Word = "five", IpaTranscription = "/faɪv/" },
                    new() { Word = "voice", IpaTranscription = "/vɔɪs/" }
                }
            },
            new()
            {
                Symbol = "θ", Category = PhonemeCategory.Consonant,
                Description = "清齿摩擦音，如 think",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "think", IpaTranscription = "/θɪŋk/" },
                    new() { Word = "three", IpaTranscription = "/θriː/" },
                    new() { Word = "math", IpaTranscription = "/mæθ/" },
                    new() { Word = "bath", IpaTranscription = "/bɑːθ/" },
                    new() { Word = "tooth", IpaTranscription = "/tuːθ/" }
                }
            },
            new()
            {
                Symbol = "ð", Category = PhonemeCategory.Consonant,
                Description = "浊齿摩擦音，如 this",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "this", IpaTranscription = "/ðɪs/" },
                    new() { Word = "that", IpaTranscription = "/ðæt/" },
                    new() { Word = "the", IpaTranscription = "/ðə/" },
                    new() { Word = "mother", IpaTranscription = "/ˈmʌðər/" },
                    new() { Word = "weather", IpaTranscription = "/ˈweðər/" }
                }
            },
            new()
            {
                Symbol = "s", Category = PhonemeCategory.Consonant,
                Description = "清齿龈摩擦音，如 sun",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "sun", IpaTranscription = "/sʌn/" },
                    new() { Word = "see", IpaTranscription = "/siː/" },
                    new() { Word = "six", IpaTranscription = "/sɪks/" },
                    new() { Word = "bus", IpaTranscription = "/bʌs/" },
                    new() { Word = "class", IpaTranscription = "/klɑːs/" }
                }
            },
            new()
            {
                Symbol = "z", Category = PhonemeCategory.Consonant,
                Description = "浊齿龈摩擦音，如 zoo",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "zoo", IpaTranscription = "/zuː/" },
                    new() { Word = "zero", IpaTranscription = "/ˈzɪərəʊ/" },
                    new() { Word = "music", IpaTranscription = "/ˈmjuːzɪk/" },
                    new() { Word = "busy", IpaTranscription = "/ˈbɪzi/" },
                    new() { Word = "nose", IpaTranscription = "/nəʊz/" }
                }
            },
            new()
            {
                Symbol = "ʃ", Category = PhonemeCategory.Consonant,
                Description = "清龈后摩擦音，如 shop",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "shop", IpaTranscription = "/ʃɒp/" },
                    new() { Word = "she", IpaTranscription = "/ʃiː/" },
                    new() { Word = "fish", IpaTranscription = "/fɪʃ/" },
                    new() { Word = "wash", IpaTranscription = "/wɒʃ/" },
                    new() { Word = "ship", IpaTranscription = "/ʃɪp/" }
                }
            },
            new()
            {
                Symbol = "ʒ", Category = PhonemeCategory.Consonant,
                Description = "浊龈后摩擦音，如 measure",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "measure", IpaTranscription = "/ˈmeʒər/" },
                    new() { Word = "vision", IpaTranscription = "/ˈvɪʒən/" },
                    new() { Word = "usual", IpaTranscription = "/ˈjuːʒuəl/" },
                    new() { Word = "pleasure", IpaTranscription = "/ˈpleʒər/" },
                    new() { Word = "garage", IpaTranscription = "/ɡəˈrɑːʒ/" }
                }
            },
            new()
            {
                Symbol = "h", Category = PhonemeCategory.Consonant,
                Description = "清声门摩擦音，如 hat",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "hat", IpaTranscription = "/hæt/" },
                    new() { Word = "hot", IpaTranscription = "/hɒt/" },
                    new() { Word = "hello", IpaTranscription = "/həˈləʊ/" },
                    new() { Word = "house", IpaTranscription = "/haʊs/" },
                    new() { Word = "happy", IpaTranscription = "/ˈhæpi/" }
                }
            },
            new()
            {
                Symbol = "tʃ", Category = PhonemeCategory.Consonant,
                Description = "清龈后塞擦音，如 chair",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "chair", IpaTranscription = "/tʃeər/" },
                    new() { Word = "child", IpaTranscription = "/tʃaɪld/" },
                    new() { Word = "teach", IpaTranscription = "/tiːtʃ/" },
                    new() { Word = "watch", IpaTranscription = "/wɒtʃ/" },
                    new() { Word = "beach", IpaTranscription = "/biːtʃ/" }
                }
            },
            new()
            {
                Symbol = "dʒ", Category = PhonemeCategory.Consonant,
                Description = "浊龈后塞擦音，如 job",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "job", IpaTranscription = "/dʒɒb/" },
                    new() { Word = "jump", IpaTranscription = "/dʒʌmp/" },
                    new() { Word = "age", IpaTranscription = "/eɪdʒ/" },
                    new() { Word = "page", IpaTranscription = "/peɪdʒ/" },
                    new() { Word = "judge", IpaTranscription = "/dʒʌdʒ/" }
                }
            },
            new()
            {
                Symbol = "m", Category = PhonemeCategory.Consonant,
                Description = "双唇鼻音，如 man",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "man", IpaTranscription = "/mæn/" },
                    new() { Word = "moon", IpaTranscription = "/muːn/" },
                    new() { Word = "come", IpaTranscription = "/kʌm/" },
                    new() { Word = "swim", IpaTranscription = "/swɪm/" },
                    new() { Word = "name", IpaTranscription = "/neɪm/" }
                }
            },
            new()
            {
                Symbol = "n", Category = PhonemeCategory.Consonant,
                Description = "齿龈鼻音，如 no",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "no", IpaTranscription = "/nəʊ/" },
                    new() { Word = "nine", IpaTranscription = "/naɪn/" },
                    new() { Word = "run", IpaTranscription = "/rʌn/" },
                    new() { Word = "ten", IpaTranscription = "/ten/" },
                    new() { Word = "pen", IpaTranscription = "/pen/" }
                }
            },
            new()
            {
                Symbol = "ŋ", Category = PhonemeCategory.Consonant,
                Description = "软腭鼻音，如 sing",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "sing", IpaTranscription = "/sɪŋ/" },
                    new() { Word = "ring", IpaTranscription = "/rɪŋ/" },
                    new() { Word = "long", IpaTranscription = "/lɒŋ/" },
                    new() { Word = "king", IpaTranscription = "/kɪŋ/" },
                    new() { Word = "thing", IpaTranscription = "/θɪŋ/" }
                }
            },
            new()
            {
                Symbol = "l", Category = PhonemeCategory.Consonant,
                Description = "齿龈边音，如 love",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "love", IpaTranscription = "/lʌv/" },
                    new() { Word = "let", IpaTranscription = "/let/" },
                    new() { Word = "feel", IpaTranscription = "/fiːl/" },
                    new() { Word = "full", IpaTranscription = "/fʊl/" },
                    new() { Word = "light", IpaTranscription = "/laɪt/" }
                }
            },
            new()
            {
                Symbol = "r", Category = PhonemeCategory.Consonant,
                Description = "齿龈近音，如 red",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "red", IpaTranscription = "/red/" },
                    new() { Word = "run", IpaTranscription = "/rʌn/" },
                    new() { Word = "rain", IpaTranscription = "/reɪn/" },
                    new() { Word = "write", IpaTranscription = "/raɪt/" },
                    new() { Word = "road", IpaTranscription = "/rəʊd/" }
                }
            },
            new()
            {
                Symbol = "w", Category = PhonemeCategory.Consonant,
                Description = "浊唇软腭近音，如 we",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "we", IpaTranscription = "/wiː/" },
                    new() { Word = "want", IpaTranscription = "/wɒnt/" },
                    new() { Word = "well", IpaTranscription = "/wel/" },
                    new() { Word = "win", IpaTranscription = "/wɪn/" },
                    new() { Word = "water", IpaTranscription = "/ˈwɔːtər/" }
                }
            },
            new()
            {
                Symbol = "j", Category = PhonemeCategory.Consonant,
                Description = "浊硬腭近音，如 yes",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "yes", IpaTranscription = "/jes/" },
                    new() { Word = "you", IpaTranscription = "/juː/" },
                    new() { Word = "year", IpaTranscription = "/jɪər/" },
                    new() { Word = "young", IpaTranscription = "/jʌŋ/" },
                    new() { Word = "use", IpaTranscription = "/juːz/" }
                }
            },
        };
    }
}
