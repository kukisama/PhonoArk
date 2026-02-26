package com.phonoark.ui.exam

import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.phonoark.data.model.ExamQuestion
import com.phonoark.data.model.ExamQuestionAttempt
import com.phonoark.data.model.ExamResult
import com.phonoark.data.model.ExampleWord
import com.phonoark.data.repository.AudioRepository
import com.phonoark.data.repository.ExamRepository
import com.phonoark.data.repository.FavoriteRepository
import com.phonoark.data.repository.HistoryRepository
import com.phonoark.data.repository.PhonemeRepository
import com.phonoark.data.repository.SettingsRepository
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.delay
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch
import javax.inject.Inject

sealed class FeedbackType {
    data object None : FeedbackType()
    data class Correct(val word: String, val ipa: String) : FeedbackType()
    data class Incorrect(val correctWord: String, val correctIpa: String) : FeedbackType()
    data class Completed(val correct: Int, val total: Int, val score: Double) : FeedbackType()
    data class NoQuestions(val message: String = "") : FeedbackType()
}

data class ExamUiState(
    val isExamActive: Boolean = false,
    val questions: List<ExamQuestion> = emptyList(),
    val currentQuestionIndex: Int = 0,
    val correctAnswers: Int = 0,
    val questionCount: Int = 10,
    val examScope: String = "All",
    val feedback: FeedbackType = FeedbackType.None,
    val isAnswered: Boolean = false,
    val examCompleted: Boolean = false
) {
    val currentQuestion: ExamQuestion?
        get() = questions.getOrNull(currentQuestionIndex)

    val totalQuestions: Int
        get() = questions.size
}

@HiltViewModel
class ExamViewModel @Inject constructor(
    private val examRepository: ExamRepository,
    private val phonemeRepository: PhonemeRepository,
    private val favoriteRepository: FavoriteRepository,
    private val historyRepository: HistoryRepository,
    private val settingsRepository: SettingsRepository,
    private val audioRepository: AudioRepository
) : ViewModel() {

    private val _uiState = MutableStateFlow(ExamUiState())
    val uiState: StateFlow<ExamUiState> = _uiState.asStateFlow()

    private var examStartTime: Long = 0

    companion object {
        private const val AUTO_ADVANCE_DELAY_MS = 1200L
    }

    init {
        viewModelScope.launch {
            val settings = settingsRepository.getSettings()
            _uiState.value = _uiState.value.copy(questionCount = settings.examQuestionCount)
            audioRepository.initialize()
            audioRepository.updateAccent(settings.defaultAccent)
            audioRepository.updateVolume(settings.volume)
        }
    }

    fun updateQuestionCount(count: Int) {
        _uiState.value = _uiState.value.copy(questionCount = count.coerceIn(5, 50))
    }

    fun updateExamScope(scope: String) {
        _uiState.value = _uiState.value.copy(examScope = scope)
    }

    fun startExam() {
        viewModelScope.launch {
            val pool = if (_uiState.value.examScope == "Favorites") {
                val favSymbols = favoriteRepository.getFavoriteSymbols()
                phonemeRepository.phonemes.filter { it.symbol in favSymbols }
            } else {
                null
            }

            val questions = examRepository.generateQuestions(_uiState.value.questionCount, pool)
            if (questions.isEmpty()) {
                _uiState.value = _uiState.value.copy(feedback = FeedbackType.NoQuestions())
                return@launch
            }

            examStartTime = System.currentTimeMillis()
            _uiState.value = _uiState.value.copy(
                isExamActive = true,
                questions = questions,
                currentQuestionIndex = 0,
                correctAnswers = 0,
                feedback = FeedbackType.None,
                isAnswered = false,
                examCompleted = false
            )
            playCurrentPhoneme()
        }
    }

    fun playCurrentPhoneme() {
        val question = _uiState.value.currentQuestion ?: return
        val phoneme = phonemeRepository.getPhonemeBySymbol(question.phoneme.symbol)
        val word = question.correctAnswer
        audioRepository.playPhoneme(
            phoneme?.voiceAudioPaths ?: emptyMap(),
            word.word
        )
    }

    fun selectAnswer(answer: ExampleWord) {
        if (_uiState.value.isAnswered) return
        val state = _uiState.value
        val question = state.currentQuestion ?: return

        val updatedQuestion = question.copy(userAnswer = answer)
        val updatedQuestions = state.questions.toMutableList()
        updatedQuestions[state.currentQuestionIndex] = updatedQuestion

        val isCorrect = answer.word == question.correctAnswer.word
        val newCorrectCount = if (isCorrect) state.correctAnswers + 1 else state.correctAnswers

        val feedback = if (isCorrect) {
            FeedbackType.Correct(question.correctAnswer.word, question.correctAnswer.ipaTranscription)
        } else {
            FeedbackType.Incorrect(question.correctAnswer.word, question.correctAnswer.ipaTranscription)
        }

        _uiState.value = state.copy(
            questions = updatedQuestions,
            correctAnswers = newCorrectCount,
            feedback = feedback,
            isAnswered = true
        )

        viewModelScope.launch {
            delay(AUTO_ADVANCE_DELAY_MS)
            nextQuestion()
        }
    }

    fun nextQuestion() {
        val state = _uiState.value
        if (!state.isAnswered) return
        val nextIndex = state.currentQuestionIndex + 1
        if (nextIndex >= state.totalQuestions) {
            endExam()
        } else {
            _uiState.value = state.copy(
                currentQuestionIndex = nextIndex,
                feedback = FeedbackType.None,
                isAnswered = false
            )
            playCurrentPhoneme()
        }
    }

    fun endExam() {
        val state = _uiState.value
        val duration = (System.currentTimeMillis() - examStartTime) / 1000
        val score = if (state.totalQuestions > 0) {
            state.correctAnswers.toDouble() / state.totalQuestions * 100
        } else 0.0

        val attempts = state.questions.mapIndexed { index, q ->
            ExamQuestionAttempt(
                questionOrder = index + 1,
                phonemeSymbol = q.phoneme.symbol,
                correctWord = q.correctAnswer.word,
                correctWordIpa = q.correctAnswer.ipaTranscription,
                userAnswerWord = q.userAnswer?.word ?: "",
                userAnswerIpa = q.userAnswer?.ipaTranscription ?: "",
                isCorrect = q.isCorrect
            )
        }

        val result = ExamResult(
            totalQuestions = state.totalQuestions,
            correctAnswers = state.correctAnswers,
            examScope = state.examScope,
            durationSeconds = duration,
            attempts = attempts
        )

        viewModelScope.launch {
            historyRepository.saveResult(result)
        }

        _uiState.value = state.copy(
            isExamActive = false,
            feedback = FeedbackType.Completed(state.correctAnswers, state.totalQuestions, score),
            examCompleted = true
        )
    }

    fun resetExam() {
        _uiState.value = ExamUiState(questionCount = _uiState.value.questionCount)
    }

    override fun onCleared() {
        super.onCleared()
    }
}
