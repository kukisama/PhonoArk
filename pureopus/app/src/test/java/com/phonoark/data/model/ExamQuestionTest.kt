package com.phonoark.data.model

import org.junit.Assert.assertEquals
import org.junit.Assert.assertFalse
import org.junit.Assert.assertTrue
import org.junit.Test

class ExamQuestionTest {

    private val correctWord = ExampleWord("apple", "/ˈæpəl/")
    private val wrongWord = ExampleWord("ball", "/bɔːl/")
    private val phoneme = Phoneme("æ", PhonemeType.VOWEL, "Short vowel", listOf(correctWord))

    @Test
    fun `isCorrect returns true when user answer matches correct answer`() {
        val question = ExamQuestion(
            phoneme = phoneme,
            options = listOf(correctWord, wrongWord),
            correctAnswer = correctWord,
            userAnswer = correctWord
        )
        assertTrue(question.isCorrect)
    }

    @Test
    fun `isCorrect returns false when user answer differs from correct answer`() {
        val question = ExamQuestion(
            phoneme = phoneme,
            options = listOf(correctWord, wrongWord),
            correctAnswer = correctWord,
            userAnswer = wrongWord
        )
        assertFalse(question.isCorrect)
    }

    @Test
    fun `isAnswered returns false when no user answer`() {
        val question = ExamQuestion(
            phoneme = phoneme,
            options = listOf(correctWord, wrongWord),
            correctAnswer = correctWord
        )
        assertFalse(question.isAnswered)
    }

    @Test
    fun `isAnswered returns true when user has answered`() {
        val question = ExamQuestion(
            phoneme = phoneme,
            options = listOf(correctWord, wrongWord),
            correctAnswer = correctWord,
            userAnswer = wrongWord
        )
        assertTrue(question.isAnswered)
    }
}
