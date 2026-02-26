package com.phonoark.data.local

import androidx.room.Entity
import androidx.room.PrimaryKey

@Entity(tableName = "settings")
data class SettingsEntity(
    @PrimaryKey
    val id: Long = 1,
    val defaultAccent: String = "US_JENNY",
    val volume: Int = 80,
    val examQuestionCount: Int = 10,
    val darkMode: Boolean = false,
    val remindersEnabled: Boolean = false,
    val language: String = "en"
)
