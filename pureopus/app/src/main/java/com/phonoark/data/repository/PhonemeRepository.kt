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

            val allWords = data.globalWords.map { ExampleWord(it.word, it.ipaTranscription) }
            val targetCount = data.targetWordsPerPhoneme

            _phonemes = data.phonemes.map { p ->
                val type = when (p.type.lowercase()) {
                    "vowel" -> PhonemeType.VOWEL
                    "diphthong" -> PhonemeType.DIPHTHONG
                    "consonant" -> PhonemeType.CONSONANT
                    else -> PhonemeType.CONSONANT
                }

                val matchingWords = allWords.filter { word ->
                    containsPhoneme(word.ipaTranscription, p.symbol)
                }

                val selectedWords = if (matchingWords.size > targetCount) {
                    matchingWords.shuffled().take(targetCount)
                } else {
                    matchingWords
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
        val cleaned = ipa.replace("/", "").replace("ˈ", "").replace("ˌ", "")
        return cleaned.contains(symbol)
    }

    fun getPhonemeBySymbol(symbol: String): Phoneme? {
        return _phonemes.find { it.symbol == symbol }
    }
}
