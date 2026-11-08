package com.phonoark

import android.content.res.Configuration
import android.util.Log
import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.activity.enableEdgeToEdge
import androidx.compose.runtime.getValue
import androidx.compose.runtime.mutableStateOf
import androidx.compose.runtime.remember
import androidx.compose.runtime.setValue
import com.phonoark.ui.navigation.AppNavigation
import com.phonoark.ui.theme.PhonoArkTheme
import dagger.hilt.android.AndroidEntryPoint
import java.util.Locale

@AndroidEntryPoint
class MainActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        Log.i("PhonoArk", "MainActivity.onCreate")
        enableEdgeToEdge()
        setContent {
            Log.i("PhonoArk", "MainActivity.setContent: composing")
            var darkTheme by remember { mutableStateOf(false) }
            PhonoArkTheme(darkTheme = darkTheme) {
                AppNavigation(
                    darkTheme = darkTheme,
                    onDarkThemeChange = { darkTheme = it }
                )
            }
        }
    }

    override fun attachBaseContext(newBase: android.content.Context) {
        // Apply saved locale before activity creation
        val prefs = newBase.getSharedPreferences("phonoark_locale", MODE_PRIVATE)
        val lang = prefs.getString("language", null)
        if (lang != null) {
            val locale = when {
                lang.startsWith("zh") -> Locale.SIMPLIFIED_CHINESE
                else -> Locale.US
            }
            Locale.setDefault(locale)
            val config = Configuration(newBase.resources.configuration)
            config.setLocale(locale)
            super.attachBaseContext(newBase.createConfigurationContext(config))
        } else {
            super.attachBaseContext(newBase)
        }
    }
}
