package com.phonoark.purecodex.model

enum class PhonemeType(val title: String) {
    VOWEL("元音"),
    DIPHTHONG("双元音"),
    CONSONANT("辅音"),
}

data class ExampleWord(
    val word: String,
    val ipaTranscription: String,
)

data class Phoneme(
    val symbol: String,
    val type: PhonemeType,
    val description: String,
    val exampleWords: List<ExampleWord>,
)

enum class ExamQuestionType {
    CHOOSE_PHONEME_FOR_WORD,
    CHOOSE_WORD_FOR_PHONEME,
}

data class ExamQuestion(
    val type: ExamQuestionType,
    val prompt: String,
    val options: List<String>,
    val correctIndex: Int,
    val phonemeSymbol: String,
)

data class ExamAnswerRecord(
    val prompt: String,
    val options: List<String>,
    val selectedIndex: Int,
    val correctIndex: Int,
    val phonemeSymbol: String,
) {
    val isCorrect: Boolean get() = selectedIndex == correctIndex
}

data class ExamHistoryItem(
    val id: Long,
    val timestamp: Long,
    val questionCount: Int,
    val correctCount: Int,
    val records: List<ExamAnswerRecord>,
)

data class AppSettings(
    val accent: String = "GenAm",
    val volume: Float = 1.0f,
    val examQuestionCount: Int = 10,
    val darkMode: Boolean = false,
    val language: String = "zh-CN",
)
