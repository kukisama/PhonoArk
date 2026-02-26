package com.phonoark.data.model

data class Phoneme(
    val symbol: String,
    val type: PhonemeType,
    val description: String,
    val exampleWords: List<ExampleWord> = emptyList(),
    val voiceAudioPaths: Map<String, String> = emptyMap()
)
