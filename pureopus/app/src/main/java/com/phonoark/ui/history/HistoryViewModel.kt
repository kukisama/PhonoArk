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
    val selectedSessionId: Long? = null,
    val hasMore: Boolean = true,
    val isLoading: Boolean = false
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

    private var allSessions: List<ExamResult> = emptyList()
    private var displayedCount: Int = 0

    companion object {
        private const val INITIAL_LOAD = 2
        private const val PAGE_SIZE = 3
    }

    init {
        loadHistory()
    }

    private fun loadHistory() {
        viewModelScope.launch {
            historyRepository.getAllResults().collect { results ->
                allSessions = results
                displayedCount = minOf(INITIAL_LOAD, results.size)
                val avgScore = historyRepository.getAverageScore()
                val totalAttempts = historyRepository.getTotalAttempts()
                val wrongAttempts = historyRepository.getWrongAttempts()
                val errorStats = historyRepository.getPhonemeErrorStats()

                _uiState.value = _uiState.value.copy(
                    sessions = allSessions.take(displayedCount),
                    averageScore = avgScore,
                    totalAttempts = totalAttempts,
                    wrongAttempts = wrongAttempts,
                    errorStats = errorStats,
                    hasMore = displayedCount < allSessions.size
                )
            }
        }
    }

    fun loadMore() {
        if (!_uiState.value.hasMore || _uiState.value.isLoading) return
        _uiState.value = _uiState.value.copy(isLoading = true)
        displayedCount = minOf(displayedCount + PAGE_SIZE, allSessions.size)
        _uiState.value = _uiState.value.copy(
            sessions = allSessions.take(displayedCount),
            hasMore = displayedCount < allSessions.size,
            isLoading = false
        )
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
