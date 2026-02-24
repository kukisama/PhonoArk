package com.phonoark.data.repository

import com.phonoark.data.local.ExamAttemptEntity
import com.phonoark.data.local.ExamHistoryDao
import com.phonoark.data.local.ExamResultEntity
import com.phonoark.data.model.ExamQuestionAttempt
import com.phonoark.data.model.ExamResult
import kotlinx.coroutines.flow.Flow
import kotlinx.coroutines.flow.map
import java.time.Instant
import java.time.LocalDateTime
import java.time.ZoneId
import javax.inject.Inject
import javax.inject.Singleton

@Singleton
class HistoryRepository @Inject constructor(
    private val examHistoryDao: ExamHistoryDao
) {
    fun getAllResults(): Flow<List<ExamResult>> {
        return examHistoryDao.getAllResults().map { entities ->
            entities.map { it.toDomain() }
        }
    }

    suspend fun saveResult(result: ExamResult): Long {
        val entity = ExamResultEntity(
            totalQuestions = result.totalQuestions,
            correctAnswers = result.correctAnswers,
            examScope = result.examScope,
            durationSeconds = result.durationSeconds
        )
        val attempts = result.attempts.map { attempt ->
            ExamAttemptEntity(
                examResultId = 0,
                questionOrder = attempt.questionOrder,
                phonemeSymbol = attempt.phonemeSymbol,
                correctWord = attempt.correctWord,
                correctWordIpa = attempt.correctWordIpa,
                userAnswerWord = attempt.userAnswerWord,
                userAnswerIpa = attempt.userAnswerIpa,
                isCorrect = attempt.isCorrect
            )
        }
        return examHistoryDao.insertResultWithAttempts(entity, attempts)
    }

    suspend fun getAttempts(resultId: Long): List<ExamQuestionAttempt> {
        return examHistoryDao.getAttemptsByResultId(resultId).map { it.toDomain() }
    }

    suspend fun getAverageScore(): Double = examHistoryDao.getAverageScore() ?: 0.0

    suspend fun getTotalAttempts(): Int = examHistoryDao.getTotalAttempts()

    suspend fun getWrongAttempts(): Int = examHistoryDao.getWrongAttempts()

    suspend fun getPhonemeErrorStats(): List<Pair<String, Int>> {
        return examHistoryDao.getPhonemeErrorStats().map { it.phonemeSymbol to it.errorCount }
    }

    suspend fun clearHistory() {
        examHistoryDao.clearHistory()
    }

    private fun ExamResultEntity.toDomain() = ExamResult(
        id = id,
        examDate = LocalDateTime.ofInstant(Instant.ofEpochMilli(examDate), ZoneId.systemDefault()),
        totalQuestions = totalQuestions,
        correctAnswers = correctAnswers,
        examScope = examScope,
        durationSeconds = durationSeconds
    )

    private fun ExamAttemptEntity.toDomain() = ExamQuestionAttempt(
        id = id,
        examResultId = examResultId,
        questionOrder = questionOrder,
        phonemeSymbol = phonemeSymbol,
        correctWord = correctWord,
        correctWordIpa = correctWordIpa,
        userAnswerWord = userAnswerWord,
        userAnswerIpa = userAnswerIpa,
        isCorrect = isCorrect
    )
}
