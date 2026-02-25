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

        // Round-robin through shuffled phonemes (matching original C# logic)
        val shuffledPhonemes = usablePhonemes.shuffled()

        repeat(count) { i ->
            val phoneme = shuffledPhonemes[i % shuffledPhonemes.size]
            val availableCorrect = phoneme.exampleWords.filter { it.word !in usedWords }
            val correctAnswer = if (availableCorrect.isNotEmpty()) {
                availableCorrect.random()
            } else {
                phoneme.exampleWords.random()
            }
            usedWords.add(correctAnswer.word)

            val wrongOptions = generateWrongOptions(phoneme, correctAnswer, usedWords, 3)
            wrongOptions.forEach { usedWords.add(it.word) }
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
        usedWords: Set<String>,
        count: Int
    ): List<ExampleWord> {
        val otherPhonemes = phonemeRepository.phonemes.filter { it.symbol != targetPhoneme.symbol }

        // First try: strict exclusion of words containing target phoneme
        val strictWrongPool = otherPhonemes
            .flatMap { it.exampleWords }
            .filter { word ->
                word.word != correctAnswer.word &&
                !usedWords.contains(word.word) &&
                !containsPhoneme(word.ipaTranscription, targetPhoneme.symbol)
            }
            .distinctBy { it.word.lowercase() }

        if (strictWrongPool.size >= count) {
            return strictWrongPool.shuffled().take(count)
        }

        // Fallback: allow words that may contain the target phoneme
        val result = strictWrongPool.toMutableList()
        val fallback = otherPhonemes
            .flatMap { it.exampleWords }
            .filter { word ->
                word.word != correctAnswer.word &&
                !usedWords.contains(word.word) &&
                result.none { it.word.equals(word.word, ignoreCase = true) }
            }
            .distinctBy { it.word.lowercase() }
            .shuffled()

        for (w in fallback) {
            if (result.size >= count) break
            result.add(w)
        }
        return result.take(count)
    }

    private fun containsPhoneme(ipa: String, symbol: String): Boolean {
        return ipa.contains(symbol)
    }
}
