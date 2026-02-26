package com.phonoark.data.model

data class ExampleWord(
    val word: String,
    val ipaTranscription: String,
    val voiceAudioPaths: Map<String, String> = emptyMap()
)
