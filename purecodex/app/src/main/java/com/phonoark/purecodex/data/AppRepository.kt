package com.phonoark.purecodex.data

import android.content.Context
import com.phonoark.purecodex.model.AppSettings
import com.phonoark.purecodex.model.ExamAnswerRecord
import com.phonoark.purecodex.model.ExamHistoryItem
import com.phonoark.purecodex.model.ExampleWord
import com.phonoark.purecodex.model.Phoneme
import com.phonoark.purecodex.model.PhonemeType
import org.json.JSONArray
import org.json.JSONObject

class AppRepository(private val context: Context) {
    private val prefs = context.getSharedPreferences("purecodex_prefs", Context.MODE_PRIVATE)

    fun loadPhonemes(): List<Phoneme> {
        val json = context.assets.open("phoneme-word-bank.json").bufferedReader().use { it.readText() }
        val root = JSONObject(json)
        val words = root.getJSONArray("globalWords").toWordList()
        return root.getJSONArray("phonemes").toPhonemeList(words)
    }

    fun loadFavorites(): Set<String> =
        prefs.getStringSet(KEY_FAVORITES, emptySet()) ?: emptySet()

    fun saveFavorites(favorites: Set<String>) {
        prefs.edit().putStringSet(KEY_FAVORITES, favorites).apply()
    }

    fun loadSettings(): AppSettings = AppSettings(
        accent = prefs.getString(KEY_ACCENT, "GenAm") ?: "GenAm",
        volume = prefs.getFloat(KEY_VOLUME, 1f),
        examQuestionCount = prefs.getInt(KEY_EXAM_COUNT, 10),
        darkMode = prefs.getBoolean(KEY_DARK_MODE, false),
        language = prefs.getString(KEY_LANGUAGE, "zh-CN") ?: "zh-CN",
    )

    fun saveSettings(settings: AppSettings) {
        prefs.edit()
            .putString(KEY_ACCENT, settings.accent)
            .putFloat(KEY_VOLUME, settings.volume)
            .putInt(KEY_EXAM_COUNT, settings.examQuestionCount)
            .putBoolean(KEY_DARK_MODE, settings.darkMode)
            .putString(KEY_LANGUAGE, settings.language)
            .apply()
    }

    fun loadHistory(): List<ExamHistoryItem> {
        val raw = prefs.getString(KEY_HISTORY, "[]") ?: "[]"
        val array = JSONArray(raw)
        return buildList {
            for (i in 0 until array.length()) {
                val item = array.getJSONObject(i)
                val records = item.getJSONArray("records")
                add(
                    ExamHistoryItem(
                        id = item.getLong("id"),
                        timestamp = item.getLong("timestamp"),
                        questionCount = item.getInt("questionCount"),
                        correctCount = item.getInt("correctCount"),
                        records = buildList {
                            for (r in 0 until records.length()) {
                                val record = records.getJSONObject(r)
                                add(
                                    ExamAnswerRecord(
                                        prompt = record.getString("prompt"),
                                        options = record.getJSONArray("options").toStringList(),
                                        selectedIndex = record.getInt("selectedIndex"),
                                        correctIndex = record.getInt("correctIndex"),
                                        phonemeSymbol = record.getString("phonemeSymbol"),
                                    ),
                                )
                            }
                        },
                    ),
                )
            }
        }
    }

    fun saveHistory(items: List<ExamHistoryItem>) {
        val array = JSONArray()
        items.forEach { item ->
            val records = JSONArray()
            item.records.forEach { record ->
                records.put(
                    JSONObject()
                        .put("prompt", record.prompt)
                        .put("options", JSONArray(record.options))
                        .put("selectedIndex", record.selectedIndex)
                        .put("correctIndex", record.correctIndex)
                        .put("phonemeSymbol", record.phonemeSymbol),
                )
            }
            array.put(
                JSONObject()
                    .put("id", item.id)
                    .put("timestamp", item.timestamp)
                    .put("questionCount", item.questionCount)
                    .put("correctCount", item.correctCount)
                    .put("records", records),
            )
        }
        prefs.edit().putString(KEY_HISTORY, array.toString()).apply()
    }

    private fun JSONArray.toWordList(): List<ExampleWord> = buildList {
        for (i in 0 until length()) {
            val word = getJSONObject(i)
            add(
                ExampleWord(
                    word = word.getString("word"),
                    ipaTranscription = word.getString("ipaTranscription"),
                ),
            )
        }
    }

    private fun JSONArray.toPhonemeList(words: List<ExampleWord>): List<Phoneme> = buildList {
        for (i in 0 until length()) {
            val item = getJSONObject(i)
            val symbol = item.getString("symbol")
            val examples = words
                .filter { it.ipaTranscription.contains(symbol, ignoreCase = true) }
                .take(5)
                .ifEmpty { words.shuffled().take(5) }
            add(
                Phoneme(
                    symbol = symbol,
                    type = item.getString("type").toPhonemeType(),
                    description = item.getString("description"),
                    exampleWords = examples,
                ),
            )
        }
    }

    private fun String.toPhonemeType(): PhonemeType = when (this.lowercase()) {
        "vowel" -> PhonemeType.VOWEL
        "diphthong" -> PhonemeType.DIPHTHONG
        else -> PhonemeType.CONSONANT
    }

    private fun JSONArray.toStringList(): List<String> = buildList {
        for (i in 0 until length()) {
            add(getString(i))
        }
    }

    companion object {
        private const val KEY_FAVORITES = "favorites"
        private const val KEY_HISTORY = "history"
        private const val KEY_ACCENT = "accent"
        private const val KEY_VOLUME = "volume"
        private const val KEY_EXAM_COUNT = "examQuestionCount"
        private const val KEY_DARK_MODE = "darkMode"
        private const val KEY_LANGUAGE = "language"
    }
}
