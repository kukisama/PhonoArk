package com.phonoark.data.local

import androidx.room.Dao
import androidx.room.Insert
import androidx.room.Query
import androidx.room.Transaction
import kotlinx.coroutines.flow.Flow

@Dao
interface ExamHistoryDao {
    @Insert
    suspend fun insertResult(result: ExamResultEntity): Long

    @Insert
    suspend fun insertAttempts(attempts: List<ExamAttemptEntity>)

    @Transaction
    suspend fun insertResultWithAttempts(result: ExamResultEntity, attempts: List<ExamAttemptEntity>): Long {
        val resultId = insertResult(result)
        val attemptsWithId = attempts.map { it.copy(examResultId = resultId) }
        insertAttempts(attemptsWithId)
        return resultId
    }

    @Query("SELECT * FROM exam_results ORDER BY examDate DESC")
    fun getAllResults(): Flow<List<ExamResultEntity>>

    @Query("SELECT * FROM exam_attempts WHERE examResultId = :resultId ORDER BY questionOrder")
    suspend fun getAttemptsByResultId(resultId: Long): List<ExamAttemptEntity>

    @Query("SELECT AVG(CAST(correctAnswers AS REAL) / totalQuestions * 100) FROM exam_results")
    suspend fun getAverageScore(): Double?

    @Query("SELECT COUNT(*) FROM exam_attempts")
    suspend fun getTotalAttempts(): Int

    @Query("SELECT COUNT(*) FROM exam_attempts WHERE isCorrect = 0")
    suspend fun getWrongAttempts(): Int

    @Query("SELECT phonemeSymbol, COUNT(*) as errorCount FROM exam_attempts WHERE isCorrect = 0 GROUP BY phonemeSymbol ORDER BY errorCount DESC")
    suspend fun getPhonemeErrorStats(): List<PhonemeErrorStat>

    @Query("DELETE FROM exam_results")
    suspend fun deleteAllResults()

    @Query("DELETE FROM exam_attempts")
    suspend fun deleteAllAttempts()

    @Transaction
    suspend fun clearHistory() {
        deleteAllAttempts()
        deleteAllResults()
    }
}

data class PhonemeErrorStat(
    val phonemeSymbol: String,
    val errorCount: Int
)
