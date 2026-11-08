package com.phonoark.ui.settings

import android.content.Context
import android.content.res.Configuration
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.phonoark.data.model.Accent
import com.phonoark.data.model.AppSettings
import com.phonoark.data.repository.AudioRepository
import com.phonoark.data.repository.SettingsRepository
import dagger.hilt.android.lifecycle.HiltViewModel
import dagger.hilt.android.qualifiers.ApplicationContext
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch
import java.util.Locale
import javax.inject.Inject

data class SettingsUiState(
    val settings: AppSettings = AppSettings(),
    val voiceDiagnostics: String = "",
    val isSaved: Boolean = false
)

@HiltViewModel
class SettingsViewModel @Inject constructor(
    private val settingsRepository: SettingsRepository,
    private val audioRepository: AudioRepository,
    @ApplicationContext private val appContext: Context
) : ViewModel() {

    private val _uiState = MutableStateFlow(SettingsUiState())
    val uiState: StateFlow<SettingsUiState> = _uiState.asStateFlow()

    init {
        loadSettings()
    }

    private fun loadSettings() {
        viewModelScope.launch {
            var settings = settingsRepository.getSettings()

            // 首次启动：如果 DB 里还是默认值，则跟随系统语言。
            // 这里采用最小规则：系统为 zh-* 则用 zh，否则用 en。
            // （避免默认固定英文导致“首次打开”体验与系统不一致）
            if (settings.language == "en") {
                val sys = Locale.getDefault().toLanguageTag()
                val desired = if (sys.startsWith("zh")) "zh" else "en"
                if (desired != settings.language) {
                    settingsRepository.updateLanguage(desired)
                    settings = settings.copy(language = desired)
                    // 也写入 attachBaseContext 读取的 locale 存储，保证下次启动一致。
                    persistLocale(desired)
                }
            }
            _uiState.value = _uiState.value.copy(settings = settings)
            audioRepository.initialize()
        }
    }

    fun updateAccent(accent: Accent) {
        _uiState.value = _uiState.value.copy(
            settings = _uiState.value.settings.copy(defaultAccent = accent),
            isSaved = false
        )
        audioRepository.updateAccent(accent)
    }

    fun updateVolume(volume: Int) {
        _uiState.value = _uiState.value.copy(
            settings = _uiState.value.settings.copy(volume = volume),
            isSaved = false
        )
        audioRepository.updateVolume(volume)
    }

    fun updateQuestionCount(count: Int) {
        _uiState.value = _uiState.value.copy(
            settings = _uiState.value.settings.copy(examQuestionCount = count.coerceIn(5, 50)),
            isSaved = false
        )
    }

    fun updateDarkMode(enabled: Boolean) {
        _uiState.value = _uiState.value.copy(
            settings = _uiState.value.settings.copy(darkMode = enabled),
            isSaved = false
        )
    }

    fun updateReminders(enabled: Boolean) {
        _uiState.value = _uiState.value.copy(
            settings = _uiState.value.settings.copy(remindersEnabled = enabled),
            isSaved = false
        )
    }

    fun updateLanguage(language: String) {
        _uiState.value = _uiState.value.copy(
            settings = _uiState.value.settings.copy(language = language),
            isSaved = false
        )

        // 语言切换需要“实时生效”，不依赖保存按钮。
        applyLocale(language)
    }

    fun saveSettings() {
        viewModelScope.launch {
            val s = _uiState.value.settings
            settingsRepository.updateAccent(s.defaultAccent)
            settingsRepository.updateVolume(s.volume)
            settingsRepository.updateExamQuestionCount(s.examQuestionCount)
            settingsRepository.updateDarkMode(s.darkMode)
            settingsRepository.updateReminders(s.remindersEnabled)
            settingsRepository.updateLanguage(s.language)

            // 这里仍保留一次 apply，保证从“旧版本”升级后的行为一致。
            applyLocale(s.language)

            _uiState.value = _uiState.value.copy(isSaved = true)
        }
    }

    private fun applyLocale(language: String) {
        val locale = when {
            language.startsWith("zh") -> Locale.SIMPLIFIED_CHINESE
            else -> Locale.ENGLISH
        }
        Locale.setDefault(locale)
        val config = Configuration(appContext.resources.configuration)
        config.setLocale(locale)
        @Suppress("DEPRECATION")
        appContext.resources.updateConfiguration(config, appContext.resources.displayMetrics)

        persistLocale(language)
    }

    private fun persistLocale(language: String) {
        appContext.getSharedPreferences("phonoark_locale", Context.MODE_PRIVATE)
            .edit()
            .putString("language", language)
            .apply()
    }

    fun runVoiceDiagnostics() {
        val diagnostics = audioRepository.getDiagnostics()
        _uiState.value = _uiState.value.copy(voiceDiagnostics = diagnostics)
    }

    override fun onCleared() {
        super.onCleared()
    }
}
