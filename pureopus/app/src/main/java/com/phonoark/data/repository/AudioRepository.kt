package com.phonoark.data.repository

import android.content.Context
import android.content.res.AssetFileDescriptor
import android.media.MediaPlayer
import android.speech.tts.TextToSpeech
import com.phonoark.data.model.Accent
import dagger.hilt.android.qualifiers.ApplicationContext
import kotlinx.coroutines.flow.MutableStateFlow
import kotlinx.coroutines.flow.StateFlow
import kotlinx.coroutines.flow.asStateFlow
import java.util.Locale
import javax.inject.Inject
import javax.inject.Singleton

@Singleton
class AudioRepository @Inject constructor(
    @ApplicationContext private val context: Context
) {
    private var tts: TextToSpeech? = null
    private var mediaPlayer: MediaPlayer? = null
    private var ttsReady = false

    private val _currentAccent = MutableStateFlow(Accent.US_JENNY)
    val currentAccent: StateFlow<Accent> = _currentAccent.asStateFlow()

    private var _volume: Int = 80
    val volume: Int get() = _volume

    fun initialize(onReady: (() -> Unit)? = null) {
        if (tts != null) {
            onReady?.invoke()
            return
        }
        tts = TextToSpeech(context) { status ->
            ttsReady = status == TextToSpeech.SUCCESS
            if (ttsReady) {
                applyTtsLocale()
            }
            onReady?.invoke()
        }
    }

    fun updateAccent(accent: Accent) {
        _currentAccent.value = accent
        applyTtsLocale()
    }

    fun updateVolume(volume: Int) {
        _volume = volume.coerceIn(0, 100)
    }

    fun playPhoneme(voiceAudioPaths: Map<String, String>, fallbackWord: String) {
        val accentKey = _currentAccent.value.name
        val wavPath = voiceAudioPaths[accentKey]
        if (wavPath != null && playAssetWav(wavPath)) {
            return
        }
        speakTts(fallbackWord)
    }

    fun playWord(word: String, voiceAudioPaths: Map<String, String> = emptyMap()) {
        val accentKey = _currentAccent.value.name
        val wavPath = voiceAudioPaths[accentKey]
        if (wavPath != null && playAssetWav(wavPath)) {
            return
        }
        speakTts(word)
    }

    fun stop() {
        tts?.stop()
        mediaPlayer?.let {
            if (it.isPlaying) it.stop()
            it.reset()
        }
    }

    fun shutdown() {
        stop()
        tts?.shutdown()
        tts = null
        ttsReady = false
        mediaPlayer?.release()
        mediaPlayer = null
    }

    fun getDiagnostics(): String {
        val engine = tts ?: return "TTS not initialized"
        val sb = StringBuilder()
        sb.appendLine("TTS Engine: ${engine.defaultEngine}")
        sb.appendLine("Language: ${engine.defaultVoice?.locale ?: "Unknown"}")
        val voices = engine.voices
        if (voices != null) {
            val enVoices = voices.filter { it.locale.language == "en" }
            sb.appendLine("English voices: ${enVoices.size}")
            enVoices.take(10).forEach { voice ->
                sb.appendLine("  - ${voice.name} (${voice.locale})")
            }
            if (enVoices.size > 10) {
                sb.appendLine("  ... and ${enVoices.size - 10} more")
            }
            sb.appendLine("Total available voices: ${voices.size}")
        }
        // Report US-Jenny asset availability
        sb.appendLine("US-Jenny assets: ${hasUsJennyAssets()}")
        return sb.toString()
    }

    private fun hasUsJennyAssets(): Boolean {
        return try {
            val files = context.assets.list("Exportfile/US-Jenny/phonemes")
            files != null && files.isNotEmpty()
        } catch (_: Exception) {
            false
        }
    }

    private fun playAssetWav(assetPath: String): Boolean {
        return try {
            val afd: AssetFileDescriptor = context.assets.openFd(assetPath)
            stop()
            if (mediaPlayer == null) {
                mediaPlayer = MediaPlayer()
            }
            mediaPlayer?.apply {
                reset()
                setDataSource(afd.fileDescriptor, afd.startOffset, afd.length)
                setVolume(_volume / 100f, _volume / 100f)
                prepare()
                start()
            }
            afd.close()
            true
        } catch (_: Exception) {
            false
        }
    }

    private fun speakTts(text: String) {
        if (!ttsReady) return
        applyTtsLocale()
        val params = android.os.Bundle().apply {
            putFloat(TextToSpeech.Engine.KEY_PARAM_VOLUME, _volume / 100f)
        }
        tts?.speak(text, TextToSpeech.QUEUE_FLUSH, params, "audio_${System.nanoTime()}")
    }

    private fun applyTtsLocale() {
        val locale = when (_currentAccent.value) {
            Accent.RP -> Locale.UK
            else -> Locale.US
        }
        tts?.language = locale
    }
}
