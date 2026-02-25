package com.phonoark.data.repository

import com.phonoark.data.model.ExampleWord
import com.phonoark.data.model.Phoneme
import com.phonoark.data.model.PhonemeType
import org.junit.Assert.assertEquals
import org.junit.Assert.assertNotNull
import org.junit.Assert.assertTrue
import org.junit.Before
import org.junit.Test
import org.mockito.Mockito.mock
import org.mockito.Mockito.`when`

class ExamRepositoryTest {

    private lateinit var phonemeRepository: PhonemeRepository
    private lateinit var examRepository: ExamRepository

    private val testPhonemes = listOf(
        Phoneme("æ", PhonemeType.VOWEL, "Short vowel", listOf(
            ExampleWord("apple", "/ˈæpəl/"),
            ExampleWord("cat", "/kæt/"),
            ExampleWord("bag", "/bæɡ/")
        )),
        Phoneme("iː", PhonemeType.VOWEL, "Long vowel", listOf(
            ExampleWord("beach", "/biːtʃ/"),
            ExampleWord("green", "/ɡriːn/"),
            ExampleWord("dream", "/driːm/")
        )),
        Phoneme("b", PhonemeType.CONSONANT, "Consonant", listOf(
            ExampleWord("ball", "/bɔːl/"),
            ExampleWord("book", "/bʊk/")
        )),
        Phoneme("t", PhonemeType.CONSONANT, "Consonant", listOf(
            ExampleWord("table", "/ˈteɪbəl/"),
            ExampleWord("ticket", "/ˈtɪkɪt/")
        ))
    )

    @Before
    fun setUp() {
        phonemeRepository = mock(PhonemeRepository::class.java)
        `when`(phonemeRepository.phonemes).thenReturn(testPhonemes)
        examRepository = ExamRepository(phonemeRepository)
    }

    @Test
    fun `generateQuestions returns requested number of questions`() {
        val questions = examRepository.generateQuestions(3)
        assertEquals(3, questions.size)
    }

    @Test
    fun `generateQuestions returns empty list when pool is empty`() {
        val questions = examRepository.generateQuestions(5, emptyList())
        assertTrue(questions.isEmpty())
    }

    @Test
    fun `each question has 4 options`() {
        val questions = examRepository.generateQuestions(2)
        questions.forEach { question ->
            assertEquals(4, question.options.size)
        }
    }

    @Test
    fun `correct answer is in options`() {
        val questions = examRepository.generateQuestions(3)
        questions.forEach { question ->
            assertTrue(question.options.contains(question.correctAnswer))
        }
    }

    @Test
    fun `correct answer belongs to the question phoneme`() {
        val questions = examRepository.generateQuestions(3)
        questions.forEach { question ->
            val phoneme = question.phoneme
            assertTrue(phoneme.exampleWords.any { it.word == question.correctAnswer.word })
        }
    }

    @Test
    fun `generateQuestions with custom pool uses only provided phonemes`() {
        val pool = listOf(testPhonemes[0]) // Only "æ"
        val questions = examRepository.generateQuestions(2, pool)
        questions.forEach { question ->
            assertEquals("æ", question.phoneme.symbol)
        }
    }
}
