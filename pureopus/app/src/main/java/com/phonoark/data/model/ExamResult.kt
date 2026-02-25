package com.phonoark.data.model

import java.time.LocalDateTime

data class ExamResult(
    val id: Long = 0,
    val examDate: LocalDateTime = LocalDateTime.now(),
    val totalQuestions: Int,
    val correctAnswers: Int,
    val examScope: String = "All",
    val durationSeconds: Long = 0,
    val attempts: List<ExamQuestionAttempt> = emptyList()
) {
    val scorePercentage: Double
        get() = if (totalQuestions > 0) correctAnswers.toDouble() / totalQuestions * 100 else 0.0
}
