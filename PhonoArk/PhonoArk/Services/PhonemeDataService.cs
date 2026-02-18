using PhonoArk.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PhonoArk.Services;

public class PhonemeDataService
{
    private readonly List<Phoneme> _phonemes = new();

    public PhonemeDataService()
    {
        InitializePhonemes();
    }

    private void InitializePhonemes()
    {
        // Vowels
        _phonemes.AddRange(new[]
        {
            new Phoneme
            {
                Symbol = "iː",
                Type = PhonemeType.Vowel,
                Description = "Long vowel, as in 'see'",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "see", IpaTranscription = "/siː/" },
                    new() { Word = "sea", IpaTranscription = "/siː/" },
                    new() { Word = "meet", IpaTranscription = "/miːt/" },
                    new() { Word = "key", IpaTranscription = "/kiː/" }
                }
            },
            new Phoneme
            {
                Symbol = "ɪ",
                Type = PhonemeType.Vowel,
                Description = "Short vowel, as in 'sit'",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "sit", IpaTranscription = "/sɪt/" },
                    new() { Word = "hit", IpaTranscription = "/hɪt/" },
                    new() { Word = "big", IpaTranscription = "/bɪɡ/" },
                    new() { Word = "ship", IpaTranscription = "/ʃɪp/" }
                }
            },
            new Phoneme
            {
                Symbol = "e",
                Type = PhonemeType.Vowel,
                Description = "Vowel, as in 'bed'",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "bed", IpaTranscription = "/bed/" },
                    new() { Word = "head", IpaTranscription = "/hed/" },
                    new() { Word = "red", IpaTranscription = "/red/" },
                    new() { Word = "said", IpaTranscription = "/sed/" }
                }
            },
            new Phoneme
            {
                Symbol = "æ",
                Type = PhonemeType.Vowel,
                Description = "Short vowel, as in 'cat'",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "cat", IpaTranscription = "/kæt/" },
                    new() { Word = "hat", IpaTranscription = "/hæt/" },
                    new() { Word = "bad", IpaTranscription = "/bæd/" },
                    new() { Word = "man", IpaTranscription = "/mæn/" }
                }
            },
            new Phoneme
            {
                Symbol = "ɑː",
                Type = PhonemeType.Vowel,
                Description = "Long vowel, as in 'car'",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "car", IpaTranscription = "/kɑːr/" },
                    new() { Word = "far", IpaTranscription = "/fɑːr/" },
                    new() { Word = "palm", IpaTranscription = "/pɑːm/" },
                    new() { Word = "father", IpaTranscription = "/ˈfɑːðər/" }
                }
            },
            new Phoneme
            {
                Symbol = "ɒ",
                Type = PhonemeType.Vowel,
                Description = "Short vowel, as in 'hot' (British)",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "hot", IpaTranscription = "/hɒt/" },
                    new() { Word = "lot", IpaTranscription = "/lɒt/" },
                    new() { Word = "dog", IpaTranscription = "/dɒɡ/" },
                    new() { Word = "box", IpaTranscription = "/bɒks/" }
                }
            },
            new Phoneme
            {
                Symbol = "ɔː",
                Type = PhonemeType.Vowel,
                Description = "Long vowel, as in 'saw'",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "saw", IpaTranscription = "/sɔː/" },
                    new() { Word = "law", IpaTranscription = "/lɔː/" },
                    new() { Word = "four", IpaTranscription = "/fɔːr/" },
                    new() { Word = "door", IpaTranscription = "/dɔːr/" }
                }
            },
            new Phoneme
            {
                Symbol = "ʊ",
                Type = PhonemeType.Vowel,
                Description = "Short vowel, as in 'book'",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "book", IpaTranscription = "/bʊk/" },
                    new() { Word = "look", IpaTranscription = "/lʊk/" },
                    new() { Word = "put", IpaTranscription = "/pʊt/" },
                    new() { Word = "good", IpaTranscription = "/ɡʊd/" }
                }
            },
            new Phoneme
            {
                Symbol = "uː",
                Type = PhonemeType.Vowel,
                Description = "Long vowel, as in 'food'",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "food", IpaTranscription = "/fuːd/" },
                    new() { Word = "moon", IpaTranscription = "/muːn/" },
                    new() { Word = "blue", IpaTranscription = "/bluː/" },
                    new() { Word = "true", IpaTranscription = "/truː/" }
                }
            },
            new Phoneme
            {
                Symbol = "ʌ",
                Type = PhonemeType.Vowel,
                Description = "Short vowel, as in 'cup'",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "cup", IpaTranscription = "/kʌp/" },
                    new() { Word = "love", IpaTranscription = "/lʌv/" },
                    new() { Word = "sun", IpaTranscription = "/sʌn/" },
                    new() { Word = "but", IpaTranscription = "/bʌt/" }
                }
            },
            new Phoneme
            {
                Symbol = "ɜː",
                Type = PhonemeType.Vowel,
                Description = "Long vowel, as in 'bird'",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "bird", IpaTranscription = "/bɜːrd/" },
                    new() { Word = "her", IpaTranscription = "/hɜːr/" },
                    new() { Word = "learn", IpaTranscription = "/lɜːrn/" },
                    new() { Word = "turn", IpaTranscription = "/tɜːrn/" }
                }
            },
            new Phoneme
            {
                Symbol = "ə",
                Type = PhonemeType.Vowel,
                Description = "Schwa, as in 'about'",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "about", IpaTranscription = "/əˈbaʊt/" },
                    new() { Word = "sofa", IpaTranscription = "/ˈsoʊfə/" },
                    new() { Word = "China", IpaTranscription = "/ˈtʃaɪnə/" },
                    new() { Word = "above", IpaTranscription = "/əˈbʌv/" }
                }
            }
        });

        // Diphthongs
        _phonemes.AddRange(new[]
        {
            new Phoneme
            {
                Symbol = "eɪ",
                Type = PhonemeType.Diphthong,
                Description = "Diphthong, as in 'day'",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "day", IpaTranscription = "/deɪ/" },
                    new() { Word = "make", IpaTranscription = "/meɪk/" },
                    new() { Word = "say", IpaTranscription = "/seɪ/" },
                    new() { Word = "rain", IpaTranscription = "/reɪn/" }
                }
            },
            new Phoneme
            {
                Symbol = "aɪ",
                Type = PhonemeType.Diphthong,
                Description = "Diphthong, as in 'my'",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "my", IpaTranscription = "/maɪ/" },
                    new() { Word = "time", IpaTranscription = "/taɪm/" },
                    new() { Word = "high", IpaTranscription = "/haɪ/" },
                    new() { Word = "buy", IpaTranscription = "/baɪ/" }
                }
            },
            new Phoneme
            {
                Symbol = "ɔɪ",
                Type = PhonemeType.Diphthong,
                Description = "Diphthong, as in 'boy'",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "boy", IpaTranscription = "/bɔɪ/" },
                    new() { Word = "toy", IpaTranscription = "/tɔɪ/" },
                    new() { Word = "coin", IpaTranscription = "/kɔɪn/" },
                    new() { Word = "voice", IpaTranscription = "/vɔɪs/" }
                }
            },
            new Phoneme
            {
                Symbol = "aʊ",
                Type = PhonemeType.Diphthong,
                Description = "Diphthong, as in 'now'",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "now", IpaTranscription = "/naʊ/" },
                    new() { Word = "how", IpaTranscription = "/haʊ/" },
                    new() { Word = "house", IpaTranscription = "/haʊs/" },
                    new() { Word = "town", IpaTranscription = "/taʊn/" }
                }
            },
            new Phoneme
            {
                Symbol = "oʊ",
                Type = PhonemeType.Diphthong,
                Description = "Diphthong, as in 'go'",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "go", IpaTranscription = "/ɡoʊ/" },
                    new() { Word = "home", IpaTranscription = "/hoʊm/" },
                    new() { Word = "know", IpaTranscription = "/noʊ/" },
                    new() { Word = "show", IpaTranscription = "/ʃoʊ/" }
                }
            },
            new Phoneme
            {
                Symbol = "ɪə",
                Type = PhonemeType.Diphthong,
                Description = "Diphthong, as in 'here'",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "here", IpaTranscription = "/hɪər/" },
                    new() { Word = "near", IpaTranscription = "/nɪər/" },
                    new() { Word = "ear", IpaTranscription = "/ɪər/" },
                    new() { Word = "deer", IpaTranscription = "/dɪər/" }
                }
            },
            new Phoneme
            {
                Symbol = "eə",
                Type = PhonemeType.Diphthong,
                Description = "Diphthong, as in 'hair'",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "hair", IpaTranscription = "/heər/" },
                    new() { Word = "care", IpaTranscription = "/keər/" },
                    new() { Word = "bear", IpaTranscription = "/beər/" },
                    new() { Word = "where", IpaTranscription = "/weər/" }
                }
            },
            new Phoneme
            {
                Symbol = "ʊə",
                Type = PhonemeType.Diphthong,
                Description = "Diphthong, as in 'poor'",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "poor", IpaTranscription = "/pʊər/" },
                    new() { Word = "sure", IpaTranscription = "/ʃʊər/" },
                    new() { Word = "tour", IpaTranscription = "/tʊər/" },
                    new() { Word = "cure", IpaTranscription = "/kjʊər/" }
                }
            }
        });

        // Consonants
        _phonemes.AddRange(new[]
        {
            new Phoneme
            {
                Symbol = "p",
                Type = PhonemeType.Consonant,
                Description = "Voiceless bilabial plosive, as in 'pen'",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "pen", IpaTranscription = "/pen/" },
                    new() { Word = "apple", IpaTranscription = "/ˈæpəl/" },
                    new() { Word = "top", IpaTranscription = "/tɑːp/" },
                    new() { Word = "happy", IpaTranscription = "/ˈhæpi/" }
                }
            },
            new Phoneme
            {
                Symbol = "b",
                Type = PhonemeType.Consonant,
                Description = "Voiced bilabial plosive, as in 'bad'",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "bad", IpaTranscription = "/bæd/" },
                    new() { Word = "baby", IpaTranscription = "/ˈbeɪbi/" },
                    new() { Word = "job", IpaTranscription = "/dʒɑːb/" },
                    new() { Word = "rabbit", IpaTranscription = "/ˈræbɪt/" }
                }
            },
            new Phoneme
            {
                Symbol = "t",
                Type = PhonemeType.Consonant,
                Description = "Voiceless alveolar plosive, as in 'top'",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "top", IpaTranscription = "/tɑːp/" },
                    new() { Word = "time", IpaTranscription = "/taɪm/" },
                    new() { Word = "cat", IpaTranscription = "/kæt/" },
                    new() { Word = "better", IpaTranscription = "/ˈbetər/" }
                }
            },
            new Phoneme
            {
                Symbol = "d",
                Type = PhonemeType.Consonant,
                Description = "Voiced alveolar plosive, as in 'dog'",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "dog", IpaTranscription = "/dɔːɡ/" },
                    new() { Word = "day", IpaTranscription = "/deɪ/" },
                    new() { Word = "red", IpaTranscription = "/red/" },
                    new() { Word = "middle", IpaTranscription = "/ˈmɪdəl/" }
                }
            },
            new Phoneme
            {
                Symbol = "k",
                Type = PhonemeType.Consonant,
                Description = "Voiceless velar plosive, as in 'cat'",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "cat", IpaTranscription = "/kæt/" },
                    new() { Word = "key", IpaTranscription = "/kiː/" },
                    new() { Word = "back", IpaTranscription = "/bæk/" },
                    new() { Word = "school", IpaTranscription = "/skuːl/" }
                }
            },
            new Phoneme
            {
                Symbol = "ɡ",
                Type = PhonemeType.Consonant,
                Description = "Voiced velar plosive, as in 'go'",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "go", IpaTranscription = "/ɡoʊ/" },
                    new() { Word = "good", IpaTranscription = "/ɡʊd/" },
                    new() { Word = "big", IpaTranscription = "/bɪɡ/" },
                    new() { Word = "dog", IpaTranscription = "/dɔːɡ/" }
                }
            },
            new Phoneme
            {
                Symbol = "f",
                Type = PhonemeType.Consonant,
                Description = "Voiceless labiodental fricative, as in 'fish'",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "fish", IpaTranscription = "/fɪʃ/" },
                    new() { Word = "fun", IpaTranscription = "/fʌn/" },
                    new() { Word = "off", IpaTranscription = "/ɔːf/" },
                    new() { Word = "photo", IpaTranscription = "/ˈfoʊtoʊ/" }
                }
            },
            new Phoneme
            {
                Symbol = "v",
                Type = PhonemeType.Consonant,
                Description = "Voiced labiodental fricative, as in 'van'",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "van", IpaTranscription = "/væn/" },
                    new() { Word = "very", IpaTranscription = "/ˈveri/" },
                    new() { Word = "love", IpaTranscription = "/lʌv/" },
                    new() { Word = "have", IpaTranscription = "/hæv/" }
                }
            },
            new Phoneme
            {
                Symbol = "θ",
                Type = PhonemeType.Consonant,
                Description = "Voiceless dental fricative, as in 'think'",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "think", IpaTranscription = "/θɪŋk/" },
                    new() { Word = "thank", IpaTranscription = "/θæŋk/" },
                    new() { Word = "bath", IpaTranscription = "/bæθ/" },
                    new() { Word = "truth", IpaTranscription = "/truːθ/" }
                }
            },
            new Phoneme
            {
                Symbol = "ð",
                Type = PhonemeType.Consonant,
                Description = "Voiced dental fricative, as in 'this'",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "this", IpaTranscription = "/ðɪs/" },
                    new() { Word = "that", IpaTranscription = "/ðæt/" },
                    new() { Word = "mother", IpaTranscription = "/ˈmʌðər/" },
                    new() { Word = "with", IpaTranscription = "/wɪð/" }
                }
            },
            new Phoneme
            {
                Symbol = "s",
                Type = PhonemeType.Consonant,
                Description = "Voiceless alveolar fricative, as in 'sit'",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "sit", IpaTranscription = "/sɪt/" },
                    new() { Word = "see", IpaTranscription = "/siː/" },
                    new() { Word = "house", IpaTranscription = "/haʊs/" },
                    new() { Word = "pass", IpaTranscription = "/pæs/" }
                }
            },
            new Phoneme
            {
                Symbol = "z",
                Type = PhonemeType.Consonant,
                Description = "Voiced alveolar fricative, as in 'zoo'",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "zoo", IpaTranscription = "/zuː/" },
                    new() { Word = "zero", IpaTranscription = "/ˈzɪəroʊ/" },
                    new() { Word = "has", IpaTranscription = "/hæz/" },
                    new() { Word = "easy", IpaTranscription = "/ˈiːzi/" }
                }
            },
            new Phoneme
            {
                Symbol = "ʃ",
                Type = PhonemeType.Consonant,
                Description = "Voiceless postalveolar fricative, as in 'ship'",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "ship", IpaTranscription = "/ʃɪp/" },
                    new() { Word = "shoe", IpaTranscription = "/ʃuː/" },
                    new() { Word = "wash", IpaTranscription = "/wɑːʃ/" },
                    new() { Word = "fish", IpaTranscription = "/fɪʃ/" }
                }
            },
            new Phoneme
            {
                Symbol = "ʒ",
                Type = PhonemeType.Consonant,
                Description = "Voiced postalveolar fricative, as in 'measure'",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "measure", IpaTranscription = "/ˈmeʒər/" },
                    new() { Word = "vision", IpaTranscription = "/ˈvɪʒən/" },
                    new() { Word = "pleasure", IpaTranscription = "/ˈpleʒər/" },
                    new() { Word = "usual", IpaTranscription = "/ˈjuːʒuəl/" }
                }
            },
            new Phoneme
            {
                Symbol = "h",
                Type = PhonemeType.Consonant,
                Description = "Voiceless glottal fricative, as in 'hat'",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "hat", IpaTranscription = "/hæt/" },
                    new() { Word = "home", IpaTranscription = "/hoʊm/" },
                    new() { Word = "who", IpaTranscription = "/huː/" },
                    new() { Word = "ahead", IpaTranscription = "/əˈhed/" }
                }
            },
            new Phoneme
            {
                Symbol = "m",
                Type = PhonemeType.Consonant,
                Description = "Bilabial nasal, as in 'man'",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "man", IpaTranscription = "/mæn/" },
                    new() { Word = "mother", IpaTranscription = "/ˈmʌðər/" },
                    new() { Word = "time", IpaTranscription = "/taɪm/" },
                    new() { Word = "come", IpaTranscription = "/kʌm/" }
                }
            },
            new Phoneme
            {
                Symbol = "n",
                Type = PhonemeType.Consonant,
                Description = "Alveolar nasal, as in 'no'",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "no", IpaTranscription = "/noʊ/" },
                    new() { Word = "now", IpaTranscription = "/naʊ/" },
                    new() { Word = "sun", IpaTranscription = "/sʌn/" },
                    new() { Word = "man", IpaTranscription = "/mæn/" }
                }
            },
            new Phoneme
            {
                Symbol = "ŋ",
                Type = PhonemeType.Consonant,
                Description = "Velar nasal, as in 'sing'",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "sing", IpaTranscription = "/sɪŋ/" },
                    new() { Word = "song", IpaTranscription = "/sɔːŋ/" },
                    new() { Word = "long", IpaTranscription = "/lɔːŋ/" },
                    new() { Word = "think", IpaTranscription = "/θɪŋk/" }
                }
            },
            new Phoneme
            {
                Symbol = "l",
                Type = PhonemeType.Consonant,
                Description = "Alveolar lateral, as in 'leg'",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "leg", IpaTranscription = "/leɡ/" },
                    new() { Word = "love", IpaTranscription = "/lʌv/" },
                    new() { Word = "all", IpaTranscription = "/ɔːl/" },
                    new() { Word = "feel", IpaTranscription = "/fiːl/" }
                }
            },
            new Phoneme
            {
                Symbol = "r",
                Type = PhonemeType.Consonant,
                Description = "Alveolar approximant, as in 'red'",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "red", IpaTranscription = "/red/" },
                    new() { Word = "run", IpaTranscription = "/rʌn/" },
                    new() { Word = "very", IpaTranscription = "/ˈveri/" },
                    new() { Word = "car", IpaTranscription = "/kɑːr/" }
                }
            },
            new Phoneme
            {
                Symbol = "j",
                Type = PhonemeType.Consonant,
                Description = "Palatal approximant, as in 'yes'",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "yes", IpaTranscription = "/jes/" },
                    new() { Word = "you", IpaTranscription = "/juː/" },
                    new() { Word = "yellow", IpaTranscription = "/ˈjeloʊ/" },
                    new() { Word = "onion", IpaTranscription = "/ˈʌnjən/" }
                }
            },
            new Phoneme
            {
                Symbol = "w",
                Type = PhonemeType.Consonant,
                Description = "Labio-velar approximant, as in 'we'",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "we", IpaTranscription = "/wiː/" },
                    new() { Word = "water", IpaTranscription = "/ˈwɔːtər/" },
                    new() { Word = "away", IpaTranscription = "/əˈweɪ/" },
                    new() { Word = "win", IpaTranscription = "/wɪn/" }
                }
            },
            new Phoneme
            {
                Symbol = "tʃ",
                Type = PhonemeType.Consonant,
                Description = "Voiceless postalveolar affricate, as in 'chair'",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "chair", IpaTranscription = "/tʃeər/" },
                    new() { Word = "church", IpaTranscription = "/tʃɜːrtʃ/" },
                    new() { Word = "watch", IpaTranscription = "/wɑːtʃ/" },
                    new() { Word = "picture", IpaTranscription = "/ˈpɪktʃər/" }
                }
            },
            new Phoneme
            {
                Symbol = "dʒ",
                Type = PhonemeType.Consonant,
                Description = "Voiced postalveolar affricate, as in 'judge'",
                ExampleWords = new List<ExampleWord>
                {
                    new() { Word = "judge", IpaTranscription = "/dʒʌdʒ/" },
                    new() { Word = "job", IpaTranscription = "/dʒɑːb/" },
                    new() { Word = "page", IpaTranscription = "/peɪdʒ/" },
                    new() { Word = "bridge", IpaTranscription = "/brɪdʒ/" }
                }
            }
        });
    }

    public List<Phoneme> GetAllPhonemes() => _phonemes;

    public List<Phoneme> GetPhonemesByType(PhonemeType type) =>
        _phonemes.Where(p => p.Type == type).ToList();

    public Phoneme? GetPhonemeBySymbol(string symbol) =>
        _phonemes.FirstOrDefault(p => p.Symbol == symbol);
}
