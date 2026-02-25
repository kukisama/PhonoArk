package com.phonoark.purecodex

import android.os.Bundle
import android.speech.tts.TextToSpeech
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.compose.material3.MaterialTheme
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.collectAsState
import androidx.lifecycle.viewmodel.compose.viewModel
import com.phonoark.purecodex.ui.PureCodexApp
import java.util.Locale

class MainActivity : ComponentActivity() {
    private var textToSpeech: TextToSpeech? = null

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        textToSpeech = TextToSpeech(this) { status ->
            if (status == TextToSpeech.SUCCESS) {
                textToSpeech?.language = Locale.US
            }
        }

        setContent {
            val appViewModel = viewModel<com.phonoark.purecodex.ui.AppViewModel>()
            val settings by appViewModel.settings.collectAsState()
            val ttsReady = remember { mutableStateOf(false) }

            LaunchedEffect(Unit) {
                ttsReady.value = textToSpeech != null
            }

            MaterialTheme {
                PureCodexApp(
                    viewModel = appViewModel,
                    onSpeak = { text ->
                        if (ttsReady.value) {
                            textToSpeech?.speak(text, TextToSpeech.QUEUE_FLUSH, null, "purecodex-$text")
                        }
                    },
                    isDark = settings.darkMode,
                )
            }
        }
    }

    override fun onDestroy() {
        textToSpeech?.stop()
        textToSpeech?.shutdown()
        super.onDestroy()
    }
}
