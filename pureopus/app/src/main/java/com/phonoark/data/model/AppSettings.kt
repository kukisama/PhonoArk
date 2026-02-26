package com.phonoark.data.model

data class AppSettings(
    val id: Long = 1,
    val defaultAccent: Accent = Accent.US_JENNY,
    val volume: Int = 80,
    val examQuestionCount: Int = 10,
    val darkMode: Boolean = false,
    val remindersEnabled: Boolean = false,
    val language: String = "en"
)
