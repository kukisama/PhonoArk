package com.phonoark.data.local

import androidx.room.Dao
import androidx.room.Insert
import androidx.room.OnConflictStrategy
import androidx.room.Query
import androidx.room.Update

@Dao
interface SettingsDao {
    @Query("SELECT * FROM settings WHERE id = 1")
    suspend fun getSettings(): SettingsEntity?

    @Insert(onConflict = OnConflictStrategy.REPLACE)
    suspend fun insertOrUpdate(settings: SettingsEntity)

    @Query("UPDATE settings SET defaultAccent = :accent WHERE id = 1")
    suspend fun updateAccent(accent: String)

    @Query("UPDATE settings SET volume = :volume WHERE id = 1")
    suspend fun updateVolume(volume: Int)

    @Query("UPDATE settings SET examQuestionCount = :count WHERE id = 1")
    suspend fun updateExamQuestionCount(count: Int)

    @Query("UPDATE settings SET darkMode = :enabled WHERE id = 1")
    suspend fun updateDarkMode(enabled: Boolean)

    @Query("UPDATE settings SET remindersEnabled = :enabled WHERE id = 1")
    suspend fun updateReminders(enabled: Boolean)

    @Query("UPDATE settings SET language = :language WHERE id = 1")
    suspend fun updateLanguage(language: String)
}
