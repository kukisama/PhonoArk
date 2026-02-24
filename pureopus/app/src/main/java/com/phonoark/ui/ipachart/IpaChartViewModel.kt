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

    init {
        loadData()
    }

    private fun loadData() {
        viewModelScope.launch {
            val settings = settingsRepository.getSettings()
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
    }

    fun speakWord(word: ExampleWord) {
        tts?.speak(word.word, TextToSpeech.QUEUE_FLUSH, null, "word_${word.word}")
    }

    fun speakPhoneme() {
        val phoneme = _uiState.value.selectedPhoneme ?: return
        val word = phoneme.exampleWords.firstOrNull() ?: return
        tts?.speak(word.word, TextToSpeech.QUEUE_FLUSH, null, "phoneme_${phoneme.symbol}")
    }

    override fun onCleared() {
        tts?.shutdown()
        super.onCleared()
    }
}
