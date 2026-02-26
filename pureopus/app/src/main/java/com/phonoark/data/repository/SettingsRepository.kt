package com.phonoark.data.repository

import com.phonoark.data.local.SettingsDao
import com.phonoark.data.local.SettingsEntity
import com.phonoark.data.model.Accent
import com.phonoark.data.model.AppSettings
import kotlinx.coroutines.flow.MutableSharedFlow
import kotlinx.coroutines.flow.SharedFlow
import kotlinx.coroutines.flow.asSharedFlow
import javax.inject.Inject
import javax.inject.Singleton

@Singleton
class SettingsRepository @Inject constructor(
    private val settingsDao: SettingsDao
) {
    private var cachedSettings: AppSettings? = null

    private val _accentChanged = MutableSharedFlow<Accent>(replay = 1)
    val accentChanged: SharedFlow<Accent> = _accentChanged.asSharedFlow()

    suspend fun getSettings(): AppSettings {
        cachedSettings?.let { return it }
        val entity = settingsDao.getSettings()
        if (entity == null) {
            val defaults = SettingsEntity()
            settingsDao.insertOrUpdate(defaults)
            val settings = defaults.toDomain()
            cachedSettings = settings
            return settings
        }
        val settings = entity.toDomain()
        cachedSettings = settings
        return settings
    }

    suspend fun updateAccent(accent: Accent) {
        ensureExists()
        settingsDao.updateAccent(accent.name)
        cachedSettings = cachedSettings?.copy(defaultAccent = accent)
        _accentChanged.emit(accent)
    }

    suspend fun updateVolume(volume: Int) {
        ensureExists()
        settingsDao.updateVolume(volume)
        cachedSettings = cachedSettings?.copy(volume = volume)
    }

    suspend fun updateExamQuestionCount(count: Int) {
        ensureExists()
        settingsDao.updateExamQuestionCount(count)
        cachedSettings = cachedSettings?.copy(examQuestionCount = count)
    }

    suspend fun updateDarkMode(enabled: Boolean) {
        ensureExists()
        settingsDao.updateDarkMode(enabled)
        cachedSettings = cachedSettings?.copy(darkMode = enabled)
    }

    suspend fun updateReminders(enabled: Boolean) {
        ensureExists()
        settingsDao.updateReminders(enabled)
        cachedSettings = cachedSettings?.copy(remindersEnabled = enabled)
    }

    suspend fun updateLanguage(language: String) {
        ensureExists()
        settingsDao.updateLanguage(language)
        cachedSettings = cachedSettings?.copy(language = language)
    }

    private suspend fun ensureExists() {
        if (settingsDao.getSettings() == null) {
            settingsDao.insertOrUpdate(SettingsEntity())
        }
    }

    private fun SettingsEntity.toDomain() = AppSettings(
        id = id,
        defaultAccent = try { Accent.valueOf(defaultAccent) } catch (_: IllegalArgumentException) { Accent.US_JENNY },
        volume = volume,
        examQuestionCount = examQuestionCount,
        darkMode = darkMode,
        remindersEnabled = remindersEnabled,
        language = language
    )
}
