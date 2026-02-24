package com.phonoark.ui.exam

import android.speech.tts.TextToSpeech
import androidx.lifecycle.ViewModel
import androidx.lifecycle.viewModelScope
import com.phonoark.data.model.ExamQuestion
import com.phonoark.data.model.ExamQuestionAttempt
import com.phonoark.data.model.ExamResult
import com.phonoark.data.model.ExampleWord
import com.phonoark.data.repository.ExamRepository
import com.phonoark.data.repository.FavoriteRepository
import com.phonoark.data.repository.HistoryRepository
import com.phonoark.data.repository.PhonemeRepository
import com.phonoark.data.repository.SettingsRepository
import dagger.hilt.android.lifecycle.HiltViewModel
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch
import javax.inject.Inject

data class ExamUiState(
    val isExamActive: Boolean = false,
    val questions: List<ExamQuestion> = emptyList(),
    val currentQuestionIndex: Int = 0,
    val correctAnswers: Int = 0,
    val questionCount: Int = 10,
    val examScope: String = "All",
    val feedbackMessage: String = "",
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
    private val settingsRepository: SettingsRepository
) : ViewModel() {

    private val _uiState = MutableStateFlow(ExamUiState())
    val uiState: StateFlow<ExamUiState> = _uiState.asStateFlow()

    var tts: TextToSpeech? = null
    private var examStartTime: Long = 0

    init {
        viewModelScope.launch {
            val settings = settingsRepository.getSettings()
            _uiState.value = _uiState.value.copy(questionCount = settings.examQuestionCount)
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
                _uiState.value = _uiState.value.copy(
                    feedbackMessage = "No questions available for the selected scope."
                )
                return@launch
            }

            examStartTime = System.currentTimeMillis()
            _uiState.value = _uiState.value.copy(
                isExamActive = true,
                questions = questions,
                currentQuestionIndex = 0,
                correctAnswers = 0,
                feedbackMessage = "",
                isAnswered = false,
                examCompleted = false
            )
        }
    }

    fun playCurrentPhoneme() {
        val question = _uiState.value.currentQuestion ?: return
        val word = question.correctAnswer
        tts?.speak(word.word, TextToSpeech.QUEUE_FLUSH, null, "exam_phoneme")
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
            "✓ Correct! The answer is '${question.correctAnswer.word}' ${question.correctAnswer.ipaTranscription}"
        } else {
            "✗ Incorrect. The correct answer is '${question.correctAnswer.word}' ${question.correctAnswer.ipaTranscription}"
        }

        _uiState.value = state.copy(
            questions = updatedQuestions,
            correctAnswers = newCorrectCount,
            feedbackMessage = feedback,
            isAnswered = true
        )
    }

    fun nextQuestion() {
        val state = _uiState.value
        val nextIndex = state.currentQuestionIndex + 1
        if (nextIndex >= state.totalQuestions) {
            endExam()
        } else {
            _uiState.value = state.copy(
                currentQuestionIndex = nextIndex,
                feedbackMessage = "",
                isAnswered = false
            )
        }
    }

    fun endExam() {
        val state = _uiState.value
        val duration = (System.currentTimeMillis() - examStartTime) / 1000
        val score = if (state.totalQuestions > 0) {
            state.correctAnswers.toDouble() / state.totalQuestions * 100
        } else 0.0

        val feedback = "Exam completed! Score: ${state.correctAnswers}/${state.totalQuestions} (${"%.1f".format(score)}%)"

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
            feedbackMessage = feedback,
            examCompleted = true
        )
    }

    fun resetExam() {
        _uiState.value = ExamUiState(questionCount = _uiState.value.questionCount)
    }

    override fun onCleared() {
        tts?.shutdown()
        super.onCleared()
    }
}
