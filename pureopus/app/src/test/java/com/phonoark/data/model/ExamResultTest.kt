package com.phonoark.data.model

import org.junit.Assert.assertEquals
import org.junit.Test

class ExamResultTest {

    @Test
    fun `scorePercentage calculates correctly`() {
        val result = ExamResult(totalQuestions = 10, correctAnswers = 7)
        assertEquals(70.0, result.scorePercentage, 0.01)
    }

    @Test
    fun `scorePercentage is 100 for perfect score`() {
        val result = ExamResult(totalQuestions = 5, correctAnswers = 5)
        assertEquals(100.0, result.scorePercentage, 0.01)
    }

    @Test
    fun `scorePercentage is 0 for zero correct`() {
        val result = ExamResult(totalQuestions = 5, correctAnswers = 0)
        assertEquals(0.0, result.scorePercentage, 0.01)
    }

    @Test
    fun `scorePercentage is 0 for zero total questions`() {
        val result = ExamResult(totalQuestions = 0, correctAnswers = 0)
        assertEquals(0.0, result.scorePercentage, 0.01)
    }
}
