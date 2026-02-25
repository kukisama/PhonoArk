package com.phonoark.ui.favorites

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.phonoark.data.model.FavoritePhoneme
import com.phonoark.data.repository.FavoriteRepository
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch
import javax.inject.Inject

data class FavoritesUiState(
    val favorites: List<FavoritePhoneme> = emptyList(),
    val groups: List<String> = emptyList(),
    val selectedGroup: String = "All"
)

@HiltViewModel
class FavoritesViewModel @Inject constructor(
    private val favoriteRepository: FavoriteRepository
) : ViewModel() {

    private val _uiState = MutableStateFlow(FavoritesUiState())
    val uiState: StateFlow<FavoritesUiState> = _uiState.asStateFlow()

    init {
        loadFavorites()
        loadGroups()
    }

    private fun loadFavorites() {
        viewModelScope.launch {
            favoriteRepository.getAll().collect { favorites ->
                val filtered = if (_uiState.value.selectedGroup != "All") {
                    favorites.filter { it.groupName == _uiState.value.selectedGroup }
                } else {
                    favorites
                }
                _uiState.value = _uiState.value.copy(favorites = filtered)
            }
        }
    }

    private fun loadGroups() {
        viewModelScope.launch {
            favoriteRepository.getAllGroups().collect { groups ->
                _uiState.value = _uiState.value.copy(groups = groups)
            }
        }
    }

    fun selectGroup(group: String) {
        _uiState.value = _uiState.value.copy(selectedGroup = group)
        loadFavorites()
    }

    fun removeFavorite(symbol: String) {
        viewModelScope.launch {
            favoriteRepository.removeFavorite(symbol)
        }
    }

    fun clearAll() {
        viewModelScope.launch {
            favoriteRepository.clearAll()
        }
    }
}
