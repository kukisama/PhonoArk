package com.phonoark.data.local

import androidx.room.Database
import androidx.room.RoomDatabase

@Database(
    entities = [
        FavoriteEntity::class,
        ExamResultEntity::class,
        ExamAttemptEntity::class,
        SettingsEntity::class
    ],
    version = 1,
    exportSchema = false
)
abstract class AppDatabase : RoomDatabase() {
    abstract fun favoriteDao(): FavoriteDao
    abstract fun examHistoryDao(): ExamHistoryDao
    abstract fun settingsDao(): SettingsDao
}
