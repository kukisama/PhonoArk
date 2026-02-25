package com.phonoark.ui.ipachart

import android.speech.tts.TextToSpeech
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.phonoark.data.model.Accent
import com.phonoark.data.model.ExampleWord
import com.phonoark.data.model.Phoneme
import com.phonoark.data.repository.FavoriteRepository
import com.phonoark.data.repository.PhonemeRepository
import com.phonoark.data.repository.SettingsRepository
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch
import java.util.Locale
import javax.inject.Inject

data class IpaChartUiState(
    val vowels: List<Phoneme> = emptyList(),
    val diphthongs: List<Phoneme> = emptyList(),
    val consonants: List<Phoneme> = emptyList(),
    val selectedPhoneme: Phoneme? = null,
    val isFavorite: Boolean = false,
    val currentAccent: Accent = Accent.GEN_AM
)

@HiltViewModel
class IpaChartViewModel @Inject constructor(
    private val phonemeRepository: PhonemeRepository,
    private val favoriteRepository: FavoriteRepository,
    private val settingsRepository: SettingsRepository
) : ViewModel() {

    private val _uiState = MutableStateFlow(IpaChartUiState())
    val uiState: StateFlow<IpaChartUiState> = _uiState.asStateFlow()

    var tts: TextToSpeech? = null
    private var currentVolume: Int = 80

    init {
        loadData()
    }

    private fun loadData() {
        viewModelScope.launch {
            val settings = settingsRepository.getSettings()
            currentVolume = settings.volume
            _uiState.value = _uiState.value.copy(
                vowels = phonemeRepository.vowels,
                diphthongs = phonemeRepository.diphthongs,
                consonants = phonemeRepository.consonants,
                currentAccent = settings.defaultAccent
            )
        }
    }

    fun selectPhoneme(phoneme: Phoneme) {
        viewModelScope.launch {
            val isFav = favoriteRepository.isFavorite(phoneme.symbol)
            _uiState.value = _uiState.value.copy(
                selectedPhoneme = phoneme,
                isFavorite = isFav
            )
        }
    }

    fun toggleFavorite() {
        val phoneme = _uiState.value.selectedPhoneme ?: return
        viewModelScope.launch {
            if (_uiState.value.isFavorite) {
                favoriteRepository.removeFavorite(phoneme.symbol)
            } else {
                favoriteRepository.addFavorite(phoneme.symbol)
            }
            _uiState.value = _uiState.value.copy(isFavorite = !_uiState.value.isFavorite)
        }
    }

    fun switchAccent() {
        val newAccent = if (_uiState.value.currentAccent == Accent.GEN_AM) Accent.RP else Accent.GEN_AM
        _uiState.value = _uiState.value.copy(currentAccent = newAccent)
        applyTtsLocale()
    }

    fun speakWord(word: ExampleWord) {
        applyTtsLocale()
        val params = android.os.Bundle().apply {
            putFloat(TextToSpeech.Engine.KEY_PARAM_VOLUME, currentVolume / 100f)
        }
        tts?.speak(word.word, TextToSpeech.QUEUE_FLUSH, params, "word_${word.word}")
    }

    fun speakPhoneme() {
        val phoneme = _uiState.value.selectedPhoneme ?: return
        val word = phoneme.exampleWords.firstOrNull() ?: return
        applyTtsLocale()
        val params = android.os.Bundle().apply {
            putFloat(TextToSpeech.Engine.KEY_PARAM_VOLUME, currentVolume / 100f)
        }
        tts?.speak(word.word, TextToSpeech.QUEUE_FLUSH, params, "phoneme_${phoneme.symbol}")
    }

    private fun applyTtsLocale() {
        val locale = if (_uiState.value.currentAccent == Accent.RP) Locale.UK else Locale.US
        tts?.language = locale
    }

    override fun onCleared() {
        tts?.shutdown()
        super.onCleared()
    }
}
