package com.phonoark.data.repository

import com.phonoark.data.model.ExamQuestion
import com.phonoark.data.model.ExampleWord
import com.phonoark.data.model.Phoneme
import javax.inject.Inject
import javax.inject.Singleton

@Singleton
class ExamRepository @Inject constructor(
    private val phonemeRepository: PhonemeRepository
) {
    fun generateQuestions(
        count: Int,
        phonemePool: List<Phoneme>? = null
    ): List<ExamQuestion> {
        val pool = phonemePool ?: phonemeRepository.phonemes
        if (pool.isEmpty()) return emptyList()

        val usablePhonemes = pool.filter { it.exampleWords.isNotEmpty() }
        if (usablePhonemes.isEmpty()) return emptyList()

        val questions = mutableListOf<ExamQuestion>()
        val usedWords = mutableSetOf<String>()

        repeat(count) {
            val phoneme = usablePhonemes.random()
            val availableCorrect = phoneme.exampleWords.filter { it.word !in usedWords }
            val correctAnswer = if (availableCorrect.isNotEmpty()) {
                availableCorrect.random()
            } else {
                phoneme.exampleWords.random()
            }
            usedWords.add(correctAnswer.word)

            val wrongOptions = generateWrongOptions(phoneme, correctAnswer, 3)
            val allOptions = (wrongOptions + correctAnswer).shuffled()

            questions.add(
                ExamQuestion(
                    phoneme = phoneme,
                    options = allOptions,
                    correctAnswer = correctAnswer
                )
            )
        }

        return questions
    }

    private fun generateWrongOptions(
        targetPhoneme: Phoneme,
        correctAnswer: ExampleWord,
        count: Int
    ): List<ExampleWord> {
        val otherPhonemes = phonemeRepository.phonemes.filter { it.symbol != targetPhoneme.symbol }
        val wrongPool = otherPhonemes
            .flatMap { it.exampleWords }
            .filter { word ->
                word.word != correctAnswer.word &&
                !containsPhoneme(word.ipaTranscription, targetPhoneme.symbol)
            }
            .distinctBy { it.word }

        return if (wrongPool.size >= count) {
            wrongPool.shuffled().take(count)
        } else {
            val fallback = otherPhonemes
                .flatMap { it.exampleWords }
                .filter { it.word != correctAnswer.word }
                .distinctBy { it.word }
            fallback.shuffled().take(count)
        }
    }

    private fun containsPhoneme(ipa: String, symbol: String): Boolean {
        val cleaned = ipa.replace("/", "").replace("ˈ", "").replace("ˌ", "")
        return cleaned.contains(symbol)
    }
}
