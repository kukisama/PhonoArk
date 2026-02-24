package com.phonoark.data.model

data class ExamQuestionAttempt(
    val id: Long = 0,
    val examResultId: Long = 0,
    val questionOrder: Int,
    val phonemeSymbol: String,
    val correctWord: String,
    val correctWordIpa: String,
    val userAnswerWord: String,
    val userAnswerIpa: String,
    val isCorrect: Boolean
)
