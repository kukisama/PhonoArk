package com.phonoark.data.model

import com.google.gson.annotations.SerializedName

data class WordBankData(
    val targetWordsPerPhoneme: Int = 30,
    val phonemes: List<PhonemeJson> = emptyList(),
    val globalWords: List<WordJson> = emptyList()
)

data class PhonemeJson(
    val symbol: String,
    val type: String,
    val description: String
)

data class WordJson(
    val word: String,
    val ipaTranscription: String
)
