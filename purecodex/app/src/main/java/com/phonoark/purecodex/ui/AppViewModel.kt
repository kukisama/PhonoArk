package com.phonoark.purecodex.ui

import android.app.Application
import androidx.lifecycle.AndroidViewModel
import androidx.lifecycle.viewModelScope
import com.phonoark.purecodex.data.AppRepository
import com.phonoark.purecodex.model.AppSettings
import com.phonoark.purecodex.model.ExamAnswerRecord
import com.phonoark.purecodex.model.ExamHistoryItem
import com.phonoark.purecodex.model.ExamQuestion
import com.phonoark.purecodex.model.ExamQuestionType
import com.phonoark.purecodex.model.Phoneme
import com.phonoark.purecodex.model.PhonemeType
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import kotlinx.coroutines.launch
import kotlin.random.Random

class AppViewModel(application: Application) : AndroidViewModel(application) {
    private val repository = AppRepository(application)

    private val _phonemes = MutableStateFlow<List<Phoneme>>(emptyList())
    val phonemes: StateFlow<List<Phoneme>> = _phonemes.asStateFlow()

    private val _favorites = MutableStateFlow<Set<String>>(emptySet())
    val favorites: StateFlow<Set<String>> = _favorites.asStateFlow()

    private val _history = MutableStateFlow<List<ExamHistoryItem>>(emptyList())
    val history: StateFlow<List<ExamHistoryItem>> = _history.asStateFlow()

    private val _settings = MutableStateFlow(AppSettings())
    val settings: StateFlow<AppSettings> = _settings.asStateFlow()

    private val _selectedType = MutableStateFlow(PhonemeType.VOWEL)
    val selectedType: StateFlow<PhonemeType> = _selectedType.asStateFlow()

    private val _selectedPhoneme = MutableStateFlow<Phoneme?>(null)
    val selectedPhoneme: StateFlow<Phoneme?> = _selectedPhoneme.asStateFlow()

    private val _examQuestions = MutableStateFlow<List<ExamQuestion>>(emptyList())
    private val _examIndex = MutableStateFlow(0)
    private val _examRecords = MutableStateFlow<List<ExamAnswerRecord>>(emptyList())
    private val _selectedOption = MutableStateFlow<Int?>(null)
    private val _feedback = MutableStateFlow<String?>(null)

    val examQuestions: StateFlow<List<ExamQuestion>> = _examQuestions.asStateFlow()
    val examIndex: StateFlow<Int> = _examIndex.asStateFlow()
    val selectedOption: StateFlow<Int?> = _selectedOption.asStateFlow()
    val feedback: StateFlow<String?> = _feedback.asStateFlow()

    init {
        viewModelScope.launch {
            _phonemes.value = repository.loadPhonemes()
            _favorites.value = repository.loadFavorites()
            _history.value = repository.loadHistory().sortedByDescending { it.timestamp }
            _settings.value = repository.loadSettings()
            _selectedPhoneme.value = _phonemes.value.firstOrNull()
        }
    }

    fun switchType(type: PhonemeType) {
        _selectedType.value = type
        _selectedPhoneme.value = _phonemes.value.firstOrNull { it.type == type }
    }

    fun selectPhoneme(phoneme: Phoneme) {
        _selectedPhoneme.value = phoneme
    }

    fun toggleFavorite(symbol: String) {
        val next = _favorites.value.toMutableSet().apply {
            if (contains(symbol)) remove(symbol) else add(symbol)
        }
        _favorites.value = next
        repository.saveFavorites(next)
    }

    fun clearFavorites() {
        _favorites.value = emptySet()
        repository.saveFavorites(emptySet())
    }

    fun updateSettings(settings: AppSettings) {
        _settings.value = settings
        repository.saveSettings(settings)
    }

    fun startExam() {
        val count = settings.value.examQuestionCount.coerceIn(5, 30)
        val generated = buildList {
            repeat(count) { index ->
                generateQuestion(index)?.let(::add)
            }
        }
        _examQuestions.value = generated
        _examIndex.value = 0
        _examRecords.value = emptyList()
        _selectedOption.value = null
        _feedback.value = null
    }

    fun submitAnswer(index: Int) {
        val question = currentQuestion() ?: return
        if (_selectedOption.value != null) return

        _selectedOption.value = index
        _feedback.value = if (index == question.correctIndex) "回答正确" else "回答错误"
        _examRecords.value = _examRecords.value + ExamAnswerRecord(
            prompt = question.prompt,
            options = question.options,
            selectedIndex = index,
            correctIndex = question.correctIndex,
            phonemeSymbol = question.phonemeSymbol,
        )
    }

    fun nextQuestionOrFinish() {
        val isLast = _examIndex.value >= _examQuestions.value.lastIndex
        if (isLast) {
            finishExam()
            return
        }

        _examIndex.value += 1
        _selectedOption.value = null
        _feedback.value = null
    }

    fun resetExam() {
        _examQuestions.value = emptyList()
        _examIndex.value = 0
        _examRecords.value = emptyList()
        _selectedOption.value = null
        _feedback.value = null
    }

    fun clearHistory() {
        _history.value = emptyList()
        repository.saveHistory(emptyList())
    }

    fun currentQuestion(): ExamQuestion? = _examQuestions.value.getOrNull(_examIndex.value)

    fun favoritePhonemes(): List<Phoneme> = _phonemes.value.filter { it.symbol in _favorites.value }

    fun phonemesBySelectedType(): List<Phoneme> = _phonemes.value.filter { it.type == _selectedType.value }

    private fun finishExam() {
        val records = _examRecords.value
        if (records.isEmpty()) {
            resetExam()
            return
        }

        val item = ExamHistoryItem(
            id = System.currentTimeMillis(),
            timestamp = System.currentTimeMillis(),
            questionCount = records.size,
            correctCount = records.count { it.isCorrect },
            records = records,
        )
        val merged = listOf(item) + _history.value
        _history.value = merged
        repository.saveHistory(merged)
        resetExam()
    }

    private fun generateQuestion(index: Int): ExamQuestion? {
        val pool = _phonemes.value
        if (pool.size < 4) return null

        val correct = pool.random()
        if (correct.exampleWords.isEmpty()) return null

        val type = if (index % 2 == 0) {
            ExamQuestionType.CHOOSE_PHONEME_FOR_WORD
        } else {
            ExamQuestionType.CHOOSE_WORD_FOR_PHONEME
        }

        return when (type) {
            ExamQuestionType.CHOOSE_PHONEME_FOR_WORD -> {
                val word = correct.exampleWords.random()
                val options = pool
                    .filter { it.symbol != correct.symbol }
                    .shuffled()
                    .take(3)
                    .map { it.symbol }
                    .toMutableList()
                    .apply { add(correct.symbol) }
                    .shuffled(Random(System.nanoTime()))
                ExamQuestion(
                    type = type,
                    prompt = "单词 \"${word.word}\" 包含哪个音标？",
                    options = options,
                    correctIndex = options.indexOf(correct.symbol),
                    phonemeSymbol = correct.symbol,
                )
            }

            ExamQuestionType.CHOOSE_WORD_FOR_PHONEME -> {
                val correctWord = correct.exampleWords.random().word
                val options = pool
                    .filter { it.symbol != correct.symbol }
                    .flatMap { it.exampleWords }
                    .map { it.word }
                    .distinct()
                    .shuffled()
                    .take(3)
                    .toMutableList()
                    .apply { add(correctWord) }
                    .shuffled(Random(System.nanoTime()))
                ExamQuestion(
                    type = type,
                    prompt = "音标 /${correct.symbol}/ 对应哪个单词？",
                    options = options,
                    correctIndex = options.indexOf(correctWord),
                    phonemeSymbol = correct.symbol,
                )
            }
        }
    }
}
