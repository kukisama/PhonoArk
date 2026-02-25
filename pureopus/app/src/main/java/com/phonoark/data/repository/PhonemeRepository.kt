package com.phonoark.data.repository

import android.content.Context
import com.google.gson.Gson
import com.phonoark.data.model.ExampleWord
import com.phonoark.data.model.Phoneme
import com.phonoark.data.model.PhonemeType
import com.phonoark.data.model.WordBankData
import dagger.hilt.android.qualifiers.ApplicationContext
import javax.inject.Inject
import javax.inject.Singleton

@Singleton
class PhonemeRepository @Inject constructor(
    @ApplicationContext private val context: Context
) {
    private var _phonemes: List<Phoneme> = emptyList()
    val phonemes: List<Phoneme> get() = _phonemes

    val vowels: List<Phoneme> get() = _phonemes.filter { it.type == PhonemeType.VOWEL }
    val diphthongs: List<Phoneme> get() = _phonemes.filter { it.type == PhonemeType.DIPHTHONG }
    val consonants: List<Phoneme> get() = _phonemes.filter { it.type == PhonemeType.CONSONANT }

    var loadError: String? = null
        private set

    init {
        loadPhonemes()
    }

    private fun loadPhonemes() {
        try {
            val json = context.assets.open("phoneme-word-bank.json")
                .bufferedReader().use { it.readText() }
            val data = Gson().fromJson(json, WordBankData::class.java)

            val allGlobalWords = data.globalWords.map { ExampleWord(it.word, it.ipaTranscription) }
            val targetCount = Math.max(4, data.targetWordsPerPhoneme)

            _phonemes = data.phonemes.map { p ->
                val type = when (p.type.lowercase()) {
                    "vowel" -> PhonemeType.VOWEL
                    "diphthong" -> PhonemeType.DIPHTHONG
                    "consonant" -> PhonemeType.CONSONANT
                    else -> PhonemeType.CONSONANT
                }

                // Merge phoneme-specific words with global matching words
                val phonemeSpecificWords = (p.words ?: emptyList()).map {
                    ExampleWord(it.word, it.ipaTranscription)
                }
                val globalMatchingWords = allGlobalWords.filter { word ->
                    containsPhoneme(word.ipaTranscription, p.symbol)
                }

                // Deduplicate by lowercase word (matching original C# logic)
                val mergedWords = (phonemeSpecificWords + globalMatchingWords)
                    .filter { it.word.isNotBlank() && it.ipaTranscription.isNotBlank() }
                    .distinctBy { it.word.trim().lowercase() }

                val selectedWords = if (mergedWords.size > targetCount) {
                    mergedWords.shuffled().take(targetCount)
                } else {
                    mergedWords
                }

                Phoneme(
                    symbol = p.symbol,
                    type = type,
                    description = p.description,
                    exampleWords = selectedWords
                )
            }
        } catch (e: Exception) {
            loadError = e.message
            _phonemes = emptyList()
        }
    }

    private fun containsPhoneme(ipa: String, symbol: String): Boolean {
        return ipa.contains(symbol)
    }

    fun getPhonemeBySymbol(symbol: String): Phoneme? {
        return _phonemes.find { it.symbol == symbol }
    }
}
