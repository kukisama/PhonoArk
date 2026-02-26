package com.phonoark.data.model

import org.junit.Assert.assertEquals
import org.junit.Assert.assertFalse
import org.junit.Test

class AppSettingsTest {

    @Test
    fun `default settings have USJenny accent`() {
        val settings = AppSettings()
        assertEquals(Accent.US_JENNY, settings.defaultAccent)
    }

    @Test
    fun `default volume is 80`() {
        val settings = AppSettings()
        assertEquals(80, settings.volume)
    }

    @Test
    fun `default exam question count is 10`() {
        val settings = AppSettings()
        assertEquals(10, settings.examQuestionCount)
    }

    @Test
    fun `dark mode defaults to false`() {
        val settings = AppSettings()
        assertFalse(settings.darkMode)
    }

    @Test
    fun `reminders default to false`() {
        val settings = AppSettings()
        assertFalse(settings.remindersEnabled)
    }

    @Test
    fun `default language is English`() {
        val settings = AppSettings()
        assertEquals("en", settings.language)
    }
}
