package com.phonoark.ui.history

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.phonoark.data.model.ExamQuestionAttempt
import com.phonoark.data.model.ExamResult
import com.phonoark.data.repository.HistoryRepository
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch
import javax.inject.Inject

data class HistoryUiState(
    val sessions: List<ExamResult> = emptyList(),
    val averageScore: Double = 0.0,
    val totalAttempts: Int = 0,
    val wrongAttempts: Int = 0,
    val errorStats: List<Pair<String, Int>> = emptyList(),
    val showWrongOnly: Boolean = false,
    val selectedSessionAttempts: List<ExamQuestionAttempt>? = null,
    val selectedSessionId: Long? = null
) {
    val wrongRate: Double
        get() = if (totalAttempts > 0) wrongAttempts.toDouble() / totalAttempts * 100 else 0.0

    val hasErrorStats: Boolean
        get() = errorStats.isNotEmpty()
}

@HiltViewModel
class HistoryViewModel @Inject constructor(
    private val historyRepository: HistoryRepository
) : ViewModel() {

    private val _uiState = MutableStateFlow(HistoryUiState())
    val uiState: StateFlow<HistoryUiState> = _uiState.asStateFlow()

    init {
        loadHistory()
    }

    private fun loadHistory() {
        viewModelScope.launch {
            historyRepository.getAllResults().collect { results ->
                val avgScore = historyRepository.getAverageScore()
                val totalAttempts = historyRepository.getTotalAttempts()
                val wrongAttempts = historyRepository.getWrongAttempts()
                val errorStats = historyRepository.getPhonemeErrorStats()

                _uiState.value = _uiState.value.copy(
                    sessions = results,
                    averageScore = avgScore,
                    totalAttempts = totalAttempts,
                    wrongAttempts = wrongAttempts,
                    errorStats = errorStats
                )
            }
        }
    }

    fun toggleShowWrongOnly() {
        _uiState.value = _uiState.value.copy(showWrongOnly = !_uiState.value.showWrongOnly)
    }

    fun openAttemptDetails(resultId: Long) {
        viewModelScope.launch {
            val attempts = historyRepository.getAttempts(resultId)
            val filtered = if (_uiState.value.showWrongOnly) {
                attempts.filter { !it.isCorrect }
            } else {
                attempts
            }
            _uiState.value = _uiState.value.copy(
                selectedSessionAttempts = filtered,
                selectedSessionId = resultId
            )
        }
    }

    fun closeAttemptDetails() {
        _uiState.value = _uiState.value.copy(
            selectedSessionAttempts = null,
            selectedSessionId = null
        )
    }

    fun clearHistory() {
        viewModelScope.launch {
            historyRepository.clearHistory()
        }
    }
}
