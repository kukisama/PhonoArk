package com.phonoark.data.local

import androidx.room.Entity
import androidx.room.ForeignKey
import androidx.room.Index
import androidx.room.PrimaryKey

@Entity(
    tableName = "exam_attempts",
    foreignKeys = [
        ForeignKey(
            entity = ExamResultEntity::class,
            parentColumns = ["id"],
            childColumns = ["examResultId"],
            onDelete = ForeignKey.CASCADE
        )
    ],
    indices = [Index("examResultId")]
)
data class ExamAttemptEntity(
    @PrimaryKey(autoGenerate = true)
    val id: Long = 0,
    val examResultId: Long,
    val questionOrder: Int,
    val phonemeSymbol: String,
    val correctWord: String,
    val correctWordIpa: String,
    val userAnswerWord: String,
    val userAnswerIpa: String,
    val isCorrect: Boolean
)
