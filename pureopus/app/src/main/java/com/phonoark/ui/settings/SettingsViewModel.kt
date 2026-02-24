package com.phonoark.ui.settings

import android.speech.tts.TextToSpeech
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.phonoark.data.model.Accent
import com.phonoark.data.model.AppSettings
import com.phonoark.data.repository.SettingsRepository
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch
import javax.inject.Inject

data class SettingsUiState(
    val settings: AppSettings = AppSettings(),
    val voiceDiagnostics: String = "",
    val isSaved: Boolean = false
)

@HiltViewModel
class SettingsViewModel @Inject constructor(
    private val settingsRepository: SettingsRepository
) : ViewModel() {

    private val _uiState = MutableStateFlow(SettingsUiState())
    val uiState: StateFlow<SettingsUiState> = _uiState.asStateFlow()

    var tts: TextToSpeech? = null

    init {
        loadSettings()
    }

    private fun loadSettings() {
        viewModelScope.launch {
            val settings = settingsRepository.getSettings()
            _uiState.value = _uiState.value.copy(settings = settings)
        }
    }

    fun updateAccent(accent: Accent) {
        _uiState.value = _uiState.value.copy(
            settings = _uiState.value.settings.copy(defaultAccent = accent),
            isSaved = false
        )
    }

    fun updateVolume(volume: Int) {
        _uiState.value = _uiState.value.copy(
            settings = _uiState.value.settings.copy(volume = volume),
            isSaved = false
        )
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
            _uiState.value = _uiState.value.copy(isSaved = true)
        }
    }

    fun runVoiceDiagnostics() {
        val ttsEngine = tts
        if (ttsEngine == null) {
            _uiState.value = _uiState.value.copy(voiceDiagnostics = "TTS not initialized")
            return
        }

        val sb = StringBuilder()
        sb.appendLine("TTS Engine: ${ttsEngine.defaultEngine}")
        sb.appendLine("Language: ${ttsEngine.defaultVoice?.locale ?: "Unknown"}")

        val voices = ttsEngine.voices
        if (voices != null) {
            sb.appendLine("Available voices: ${voices.size}")
            voices.take(5).forEach { voice ->
                sb.appendLine("  - ${voice.name} (${voice.locale})")
            }
            if (voices.size > 5) {
                sb.appendLine("  ... and ${voices.size - 5} more")
            }
        }

        _uiState.value = _uiState.value.copy(voiceDiagnostics = sb.toString())
    }

    override fun onCleared() {
        tts?.shutdown()
        super.onCleared()
    }
}
