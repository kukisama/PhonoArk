package com.phonoark.ui.ipachart

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.phonoark.data.model.Accent
import com.phonoark.data.model.ExampleWord
import com.phonoark.data.model.Phoneme
import com.phonoark.data.model.PhonemeType
import com.phonoark.data.repository.AudioRepository
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
    val currentAccent: Accent = Accent.US_JENNY,
    val favoriteSymbols: Set<String> = emptySet(),
    val playingWordId: String? = null,
    val loadError: String? = null
) {
    val allVowelsFavorited: Boolean
        get() = vowels.isNotEmpty() && vowels.all { it.symbol in favoriteSymbols }
    val allDiphthongsFavorited: Boolean
        get() = diphthongs.isNotEmpty() && diphthongs.all { it.symbol in favoriteSymbols }
    val allConsonantsFavorited: Boolean
        get() = consonants.isNotEmpty() && consonants.all { it.symbol in favoriteSymbols }
}

@HiltViewModel
class IpaChartViewModel @Inject constructor(
    private val phonemeRepository: PhonemeRepository,
    private val favoriteRepository: FavoriteRepository,
    private val settingsRepository: SettingsRepository,
    private val audioRepository: AudioRepository
) : ViewModel() {

    private val _uiState = MutableStateFlow(IpaChartUiState())
    val uiState: StateFlow<IpaChartUiState> = _uiState.asStateFlow()

    init {
        loadData()
        collectAccentChanges()
    }

    private fun loadData() {
        viewModelScope.launch {
            val settings = settingsRepository.getSettings()
            audioRepository.initialize()
            audioRepository.updateAccent(settings.defaultAccent)
            audioRepository.updateVolume(settings.volume)
            val favSymbols = favoriteRepository.getFavoriteSymbols().toSet()
            _uiState.value = _uiState.value.copy(
                vowels = phonemeRepository.vowels,
                diphthongs = phonemeRepository.diphthongs,
                consonants = phonemeRepository.consonants,
                currentAccent = settings.defaultAccent,
                favoriteSymbols = favSymbols,
                loadError = phonemeRepository.loadError
            )
        }
    }

    private fun collectAccentChanges() {
        viewModelScope.launch {
            audioRepository.currentAccent.collect { accent ->
                _uiState.value = _uiState.value.copy(currentAccent = accent)
            }
        }
    }

    fun selectPhoneme(phoneme: Phoneme) {
        viewModelScope.launch {
            val isFav = favoriteRepository.isFavorite(phoneme.symbol)
            _uiState.value = _uiState.value.copy(
                selectedPhoneme = phoneme,
                isFavorite = isFav
            )
            speakPhoneme()
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
            val newFavSymbols = favoriteRepository.getFavoriteSymbols().toSet()
            _uiState.value = _uiState.value.copy(
                isFavorite = !_uiState.value.isFavorite,
                favoriteSymbols = newFavSymbols
            )
        }
    }

    fun toggleFavoriteBatch(type: PhonemeType) {
        viewModelScope.launch {
            val phonemes = when (type) {
                PhonemeType.VOWEL -> _uiState.value.vowels
                PhonemeType.DIPHTHONG -> _uiState.value.diphthongs
                PhonemeType.CONSONANT -> _uiState.value.consonants
            }
            val allFavorited = phonemes.all { it.symbol in _uiState.value.favoriteSymbols }
            if (allFavorited) {
                phonemes.forEach { favoriteRepository.removeFavorite(it.symbol) }
            } else {
                phonemes.forEach { favoriteRepository.addFavorite(it.symbol) }
            }
            val newFavSymbols = favoriteRepository.getFavoriteSymbols().toSet()
            val selectedPhoneme = _uiState.value.selectedPhoneme
            _uiState.value = _uiState.value.copy(
                favoriteSymbols = newFavSymbols,
                isFavorite = selectedPhoneme?.let { it.symbol in newFavSymbols } ?: false
            )
        }
    }

    fun clearAllFavorites() {
        viewModelScope.launch {
            favoriteRepository.clearAll()
            _uiState.value = _uiState.value.copy(
                favoriteSymbols = emptySet(),
                isFavorite = false
            )
        }
    }

    fun switchAccent() {
        val newAccent = when (_uiState.value.currentAccent) {
            Accent.US_JENNY -> Accent.GEN_AM
            Accent.GEN_AM -> Accent.RP
            Accent.RP -> Accent.US_JENNY
        }
        audioRepository.updateAccent(newAccent)
    }

    fun speakWord(word: ExampleWord) {
        _uiState.value = _uiState.value.copy(playingWordId = word.word)
        audioRepository.playWord(word.word, word.voiceAudioPaths)
        viewModelScope.launch {
            kotlinx.coroutines.delay(1500)
            _uiState.value = _uiState.value.copy(playingWordId = null)
        }
    }

    fun speakPhoneme() {
        val phoneme = _uiState.value.selectedPhoneme ?: return
        val word = phoneme.exampleWords.firstOrNull() ?: return
        audioRepository.playPhoneme(phoneme.voiceAudioPaths, word.word)
    }

    override fun onCleared() {
        super.onCleared()
    }
}
